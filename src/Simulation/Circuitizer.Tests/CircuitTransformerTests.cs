// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.IO;
using System.Linq;

using Microsoft.FSharp.Core;
using Microsoft.Quantum.QsCompiler;
using Microsoft.Quantum.QsCompiler.CsharpGeneration;
using Microsoft.Quantum.QsCompiler.Diagnostics;
using Microsoft.Quantum.QsCompiler.SyntaxTokens;
using Microsoft.Quantum.QsCompiler.SyntaxTree;
using Microsoft.Quantum.QsCompiler.Transformations;
using Microsoft.Quantum.Simulation.Circuitizer;
using Microsoft.VisualStudio.LanguageServer.Protocol;

using Xunit;

using Assert = Xunit.Assert;

namespace Circuitizer.Tests
{
    public class Logger : LogTracker
    {
        private readonly Xunit.Abstractions.ITestOutputHelper output;

        public Logger(Xunit.Abstractions.ITestOutputHelper output)
        {
            this.output = output;
        }

        protected override void Print(Diagnostic msg)
            => output.WriteLine(Formatting.MsBuildFormat(msg));
    }

    public class OperationBodyFinder
        : SyntaxTreeTransformation<NoScopeTransformations>
    {
        private readonly string _operation;

        public QsScope Body { get; private set; }

        public OperationBodyFinder(string operationName) : base((new NoScopeTransformations()))
        {
            _operation = operationName;
        }

        public override QsCallable onCallableImplementation(QsCallable c)
        {
            /// Only continue on the actual operation we're trying to find.
            if (c.FullName.Name.Value == _operation)
            {
                return base.onCallableImplementation(c);
            }
            else
            {
                return c;
            }
        }

        public override QsSpecialization beforeSpecialization(QsSpecialization spec)
        {
            /// Only continue on the body of the operation.
            if (spec.Kind == QsSpecializationKind.QsBody)
            {
                return base.beforeSpecialization(spec);
            }

            return spec;
        }

        public override Tuple<QsTuple<LocalVariableDeclaration<QsLocalSymbol>>, QsScope> onProvidedImplementation(QsTuple<LocalVariableDeclaration<QsLocalSymbol>> argTuple, QsScope body)
        {
            /// This is only called once we get to the body of the implementation we're looking for, no need to continue
            this.Body = body;
            return new Tuple<QsTuple<LocalVariableDeclaration<QsLocalSymbol>>, QsScope>(argTuple, body);
        }

        public static QsScope Find(string operationName, QsNamespace ns)
        {
            var finder = new OperationBodyFinder(operationName);
            finder.Transform(ns);

            return finder.Body;
        }
    }

    public class CircuitTransformerTests
    {
        private Logger Logger { get; }

        public CircuitTransformerTests(Xunit.Abstractions.ITestOutputHelper output)
        {
            this.Logger = new Logger(output);
        }

        private QsNamespace[] LoadSyntaxTree(params string[] filenames)
        {
            var baseFolder = "RawInputs";
            var sources = filenames.Select(f => Path.Combine(baseFolder, f));
            var loadOptions =
                new CompilationLoader.Configuration()
                {
                    GenerateFunctorSupport = true,
                };

            var references = new string[] { typeof(Microsoft.Quantum.Intrinsic.X).Assembly.Location };
            var loader = new CompilationLoader(sources, references, loadOptions, this.Logger);
            
            var result = loader.GeneratedSyntaxTree.ToArray();
            return result;
        }

        // Assume the operation lives in a file with the same name as the operation we're testing.
        public QsScope FindOperationBody(string operationName) =>
            FindOperationBody(operationName, LoadSyntaxTree($"{operationName}.qs"));

        public QsScope FindOperationBody(string operationName, QsNamespace[] syntaxTree)
        {
            var testsNS = syntaxTree.First(ns => ns.Name.Value == "Circuitizer.Tests");
            return OperationBodyFinder.Find(operationName, testsNS);
        }

        private ClassicallyControlledTransformation.ScopeTransformation CreateTransformation() =>
            (new ClassicallyControlledTransformation()).Scope as ClassicallyControlledTransformation.ScopeTransformation;

        private string[] ToCSharp(QsNamespace[] syntaxTreee, QsScope body)
        {
            var context = SimulationCode.createContext(FSharpOption<string>.None, syntaxTreee);
            var builder = new SimulationCode.StatementBlockBuilder(context);
            builder.Transform(body);

            return builder.Statements.Select(s => s.ToString()).ToArray();
        }

        [Fact]
        public void TestIsSimpleCallStatement()
        {
            var transformation = CreateTransformation();
            var body = FindOperationBody("ToyOperation");
            var stmts = body.Statements;

            var expected = new bool[] {
                true,       // H(q);
                true,       // Adjoint Rx(1.2, q);
                true,       // Controlled Wrapper([ctrl], (q, false));
                true,       // (Wrapper(q,_))(false);
                false,      // let r = M(q);
                false,      // if (r == Zero) { ...
                false,      // if (One == M(q)) {
                false       // return r;
            };

            var actual = (
                from s in stmts
                select transformation.IsSimpleCallStatement(s.Statement).Item1
            ).ToArray();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestIsConditionedOnResultLiteralStatement()
        {
            var transformation = CreateTransformation();
            var body = FindOperationBody("ToyOperation");
            var stmts = body.Statements;

            var expected = new (bool, QsResult)[] {
                (false, null),               // H(q);
                (false, null),               // Adjoint Rx(1.2, q);
                (false, null),               // Controlled Wrapper([ctrl], (q, false));
                (false, null),               // (Wrapper(q,_))(false);
                (false, null),               // let r = M(q);
                (true, QsResult.Zero),       // if (r == Zero) { ...
                (true, QsResult.One),        // if (One == M(q)) {
                (false, null)                // return r;
            };

            var actual = stmts
                .Select(s => transformation.IsConditionedOnResultLiteralStatement(s.Statement))
                .Select(r => (r.Item1, r.Item2))
                .ToArray();

            Assert.Equal(expected, actual);
        }


        [Fact]
        public void TestTransformToyOperation()
        {
            var transformation = CreateTransformation();
            var syntaxTree = LoadSyntaxTree($"ToyOperation.qs");
            var all = FindOperationBody("ToyOperation", syntaxTree);

            string removeWhitespace(string str) => string.Concat(str.Where(c => !char.IsWhiteSpace(c)));

            var expected = new string[]
            {
                "MicrosoftQuantumIntrinsicH.Apply(q);",
                "MicrosoftQuantumIntrinsicRx.Adjoint.Apply((1.2D,q));",
                "CircuitizerTestsWrapper.Controlled.Apply((newQArray<Qubit>(ctrl),(q,false)));",
                "CircuitizerTestsWrapper.Partial(new Func<Boolean, (Qubit,Boolean)>((__arg1__) => (q,__arg1__))).Apply(false);",

                "var r = MicrosoftQuantumIntrinsicM.Apply(q);",

                "MicrosoftQuantumSimulationCircuitizerExtensionsApplyIfZero.Apply((r,(CircuitizerTestsWrapper.Partial(new Func<Boolean,(Qubit,Boolean)>((__arg2__) => (q,__arg2__))), true)));",
                "MicrosoftQuantumSimulationCircuitizerExtensionsApplyIfZero.Apply((r,(MicrosoftQuantumIntrinsicCNOT,(ctrl,q))));",
                "MicrosoftQuantumSimulationCircuitizerExtensionsApplyIfZero.Apply((r,(MicrosoftQuantumIntrinsicRx.Adjoint,(1.2D,q))));",

                "var __classic_ctrl1__ = MicrosoftQuantumIntrinsicM.Apply(q);",
                "MicrosoftQuantumSimulationCircuitizerExtensionsApplyIfOne.Apply((__classic_ctrl1__,(MicrosoftQuantumIntrinsicY,q)));",
                "MicrosoftQuantumSimulationCircuitizerExtensionsApplyIfOne.Apply((__classic_ctrl1__,(MicrosoftQuantumIntrinsicY.Controlled,(newQArray<Qubit>(ctrl),q))));",
                "MicrosoftQuantumSimulationCircuitizerExtensionsApplyIfOne.Apply((__classic_ctrl1__,(MicrosoftQuantumIntrinsicR,(Pauli.PauliY,1.2D,q))));",

                "return r;"
            }.Select(removeWhitespace).ToArray();

            var actual = ToCSharp(syntaxTree, transformation.Transform(all));

            Assert.Equal(expected.Length, actual.Length);
            for (var i = 0; i < expected.Length; i++)
            {
                Assert.Equal(expected[i], actual[i]);
            }
        }
    }
}

