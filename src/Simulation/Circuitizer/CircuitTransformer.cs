using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.FSharp.Core;
using Microsoft.Quantum.QsCompiler.DataTypes;
using Microsoft.Quantum.QsCompiler.SyntaxTokens;
using Microsoft.Quantum.QsCompiler.SyntaxTree;
using Microsoft.Quantum.QsCompiler.Transformations;
using Microsoft.Quantum.QsCompiler.Transformations.Core;
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


            public (bool, TypedExpression, QsScope) IsConditionedOnOneStatement(QsStatementKind statement)
            {

                bool IsConstantOne(TypedExpression expression)
                {
                    if (expression.Expression is ExpressionKind.ResultLiteral result)
                    {
                        return (result.Item == QsResult.One);
                    }

                    return false;
                }


                if (statement is QsStatementKind.QsConditionalStatement cond)
                {
                    if (cond.Item.ConditionalBlocks.Length == 1 && (cond.Item.ConditionalBlocks[0].Item1.Expression is ExpressionKind.EQ expression))
                    {
                        if (IsConstantOne(expression.Item1))
                        {
                            return (true, expression.Item2, cond.Item.ConditionalBlocks[0].Item2.Body);
                        }
                        else if (IsConstantOne(expression.Item2))
                        {
                            return (true, expression.Item1, cond.Item.ConditionalBlocks[0].Item2.Body);
                        }
                    }
                }

                return (false, null, null);
            }

            public (bool, ExpressionKind.Identifier, TypedExpression) IsSimpleCallStatement(QsStatementKind statement)
            {
                if (statement is QsStatementKind.QsExpressionStatement expr)
                {
                    var returnType = expr.Item.ResolvedType;

                    if (returnType.Resolution.IsUnitType
                        && expr.Item.Expression is ExpressionKind.CallLikeExpression call
                        && call.Item1.Expression is ExpressionKind.Identifier id)
                    {
                        return (true, id, call.Item2);
                    }
                }

                return (false, null, null);
            }

            public TypedExpression CreateTypedExpression(ExpressionKind expression)
            {
                var inferredInfo = new InferredExpressionInformation(isMutable: false, hasLocalQuantumDependency: true);
                var nullRange = QsNullable<Tuple<QsPositionInfo, QsPositionInfo>>.Null;
                var emptyTypes = ImmutableDictionary<QsTypeParameter, ResolvedType>.Empty;
                var unit = ResolvedType.New(QsTypeKind<ResolvedType, UserDefinedType, QsTypeParameter, CallableInformation>.UnitType);

                return new TypedExpression(expression, emptyTypes, unit, inferredInfo, nullRange);
            }

            public QsStatement CreateApplyIfStatement(TypedExpression conditionExpression, QsStatement s)
            {
                var (_, identifier, originalArgs) = IsSimpleCallStatement(s.Statement);

                var nullTypes = QsNullable<ImmutableArray<ResolvedType>>.Null;

                var one = CreateTypedExpression(ExpressionKind.NewResultLiteral(QsResult.One));
                var original = CreateTypedExpression(ExpressionKind.NewValueTuple(new TypedExpression[] { CreateTypedExpression(identifier), originalArgs }.ToImmutableArray()));

                var applyIfOne = Identifier.NewGlobalCallable(new QsQualifiedName(NonNullable<string>.New(typeof(ApplyIfOne<>).Namespace), NonNullable<string>.New("ApplyIfOne"))) as Identifier.GlobalCallable;
                var id = CreateTypedExpression(ExpressionKind.NewIdentifier(applyIfOne, nullTypes));

                var args = CreateTypedExpression(ExpressionKind.NewValueTuple(new TypedExpression[] { one, original }.ToImmutableArray()));
                var call = CreateTypedExpression(ExpressionKind.NewCallLikeExpression(id, args));

                return new QsStatement(QsStatementKind.NewQsExpressionStatement(call), s.SymbolDeclarations, s.Location, s.Comments);
            }

            public override QsScope Transform(QsScope scope)
            {
                var statements = ImmutableArray.CreateBuilder<QsStatement>();
                foreach (var statement in scope.Statements)
                {
                    var (isCondition, conditionExpression, conditionScope) = IsConditionedOnOneStatement(statement.Statement);

                    if (isCondition && conditionScope.Statements.Select(s => IsSimpleCallStatement(s.Statement).Item1).All(b => b))
                    {
                        foreach (var stmt in conditionScope.Statements)
                        {
                            statements.Add(CreateApplyIfStatement(conditionExpression, stmt));
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
