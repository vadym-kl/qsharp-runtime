// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.Quantum.Intrinsic;
using Microsoft.Quantum.QsCompiler.DataTypes;
using Microsoft.Quantum.QsCompiler.SyntaxTokens;
using Microsoft.Quantum.QsCompiler.SyntaxTree;
using Microsoft.Quantum.QsCompiler.Transformations;
using Microsoft.Quantum.Simulation.Circuitizer.Extensions;

using ExpressionKind = Microsoft.Quantum.QsCompiler.SyntaxTokens.QsExpressionKind<Microsoft.Quantum.QsCompiler.SyntaxTree.TypedExpression, Microsoft.Quantum.QsCompiler.SyntaxTree.Identifier, Microsoft.Quantum.QsCompiler.SyntaxTree.ResolvedType>;

namespace Microsoft.Quantum.Simulation.Circuitizer
{
    public class ClassicallyControlledConditions
        : SyntaxTreeTransformation<ClassicallyControlledConditions.ScopeTransformation>
    {
        public ClassicallyControlledConditions() 
            : base(new ScopeTransformation())
        {
        }

        public class ScopeTransformation 
            : ScopeTransformation<StatementsTransformation, NoExpressionTransformations>
        {
            public ScopeTransformation() : 
                base (
                    s => new StatementsTransformation(s as ScopeTransformation), 
                    new NoExpressionTransformations()
                )
            { }


            public (bool, QsResult, TypedExpression, QsScope) IsConditionedOnResultLiteralStatement(QsStatementKind statement)
            {
                if (statement is QsStatementKind.QsConditionalStatement cond)
                {
                    if (cond.Item.ConditionalBlocks.Length == 1 && (cond.Item.ConditionalBlocks[0].Item1.Expression is ExpressionKind.EQ expression))
                    {
                        var scope = cond.Item.ConditionalBlocks[0].Item2.Body;

                        if (expression.Item1.Expression is ExpressionKind.ResultLiteral exp1)
                        {
                            return (true, exp1.Item, expression.Item2, scope);
                        }
                        else if (expression.Item2.Expression is ExpressionKind.ResultLiteral exp2)
                        {
                            return (true, exp2.Item, expression.Item1, scope);
                        }
                    }
                }

                return (false, null, null, null);
            }

            public bool AreSimpleCallStatements(IEnumerable<QsStatement> stmts) =>
                stmts.Select(s => IsSimpleCallStatement(s.Statement).Item1).All(b => b);

            public (bool, TypedExpression, TypedExpression) IsSimpleCallStatement(QsStatementKind statement)
            {
                if (statement is QsStatementKind.QsExpressionStatement expr)
                {
                    var returnType = expr.Item.ResolvedType;

                    if (returnType.Resolution.IsUnitType && expr.Item.Expression is ExpressionKind.CallLikeExpression call)
                    {
                        return (true, call.Item1, call.Item2);
                    }
                }

                return (false, null, null);
            }

            public TypedExpression CreateTypedExpression(ExpressionKind expression) =>
                CreateTypedExpression(expression, ResolvedType.New(QsTypeKind<ResolvedType, UserDefinedType, QsTypeParameter, CallableInformation>.UnitType));
            
            public TypedExpression CreateTypedExpression(ExpressionKind expression, ResolvedType returnType)
            {
                var inferredInfo = new InferredExpressionInformation(isMutable: false, hasLocalQuantumDependency: true);
                var nullRange = QsNullable<Tuple<QsPositionInfo, QsPositionInfo>>.Null;
                var emptyTypes = ImmutableDictionary<QsTypeParameter, ResolvedType>.Empty;

                return new TypedExpression(expression, emptyTypes, returnType, inferredInfo, nullRange);
            }

            public QsStatement CreateApplyIfStatement(QsResult result, TypedExpression conditionExpression, QsStatement s)
            {
                var (_, op, originalArgs) = IsSimpleCallStatement(s.Statement);

                var nullTypes = QsNullable<ImmutableArray<ResolvedType>>.Null;

                var originalCall = CreateTypedExpression(ExpressionKind.NewValueTuple(new TypedExpression[] { op, originalArgs }.ToImmutableArray()));

                var opType = (result == QsResult.One)
                    ? typeof(ApplyIfOne<>)
                    : typeof(ApplyIfZero<>);
                var applyIfOp = Identifier.NewGlobalCallable(new QsQualifiedName(NonNullable<string>.New(opType.Namespace), NonNullable<string>.New(opType.Name.Remove(opType.Name.IndexOf('`'))))) as Identifier.GlobalCallable;
                var id = CreateTypedExpression(ExpressionKind.NewIdentifier(applyIfOp, nullTypes));

                var args = CreateTypedExpression(ExpressionKind.NewValueTuple(new TypedExpression[] { conditionExpression, originalCall }.ToImmutableArray()));
                var call = CreateTypedExpression(ExpressionKind.NewCallLikeExpression(id, args));

                return new QsStatement(QsStatementKind.NewQsExpressionStatement(call), s.SymbolDeclarations, s.Location, s.Comments);
            }

            private static int _varCount = 0;

            public (QsStatement, TypedExpression) CreateNewConditionVariable(TypedExpression value, QsStatement condStatement)
            {
                _varCount++;
                var name = $"__classic_ctrl{_varCount}__";

                // The typed expression with the identifier of the variable we just created:
                var idExpression = CreateTypedExpression(ExpressionKind.NewIdentifier(Identifier.NewLocalVariable(NonNullable<string>.New(name)), QsNullable<ImmutableArray<ResolvedType>>.Null));

                // The actual binding statement:
                var binding = new QsBinding<TypedExpression>(QsBindingKind.ImmutableBinding, SymbolTuple.NewVariableName(NonNullable<string>.New(name)), value);                
                var stmt = new QsStatement(QsStatementKind.NewQsVariableDeclaration(binding), condStatement.SymbolDeclarations, condStatement.Location, condStatement.Comments);

                return (stmt, idExpression);
            }

            public override QsScope Transform(QsScope scope)
            {
                var statements = ImmutableArray.CreateBuilder<QsStatement>();
                foreach (var statement in scope.Statements)
                {
                    var (isCondition, result, conditionExpression, conditionScope) = IsConditionedOnResultLiteralStatement(statement.Statement);

                    if (isCondition && AreSimpleCallStatements(conditionScope.Statements))
                    {
                        // The condition must be an identifier, otherwise we'll call it multiple times.
                        // If not, create a new variable and use that:
                        if (!(conditionExpression.Expression is ExpressionKind.Identifier))
                        {
                            var (letStmt, idExpression) = CreateNewConditionVariable(conditionExpression, statement);
                            statements.Add(letStmt);
                            conditionExpression = idExpression;
                        }

                        foreach (var stmt in conditionScope.Statements)
                        {
                            statements.Add(CreateApplyIfStatement(result, conditionExpression, stmt));
                        }
                    }
                    else
                    {
                        statements.Add(this.onStatement(statement));
                    }
                }

                return new QsScope(statements.ToImmutableArray(), scope.KnownSymbols);
            }
        }

        public class StatementsTransformation
            : StatementKindTransformation<ScopeTransformation>
        {
            public StatementsTransformation(ScopeTransformation scope) : base(scope)
            { }
        }
    }
}
