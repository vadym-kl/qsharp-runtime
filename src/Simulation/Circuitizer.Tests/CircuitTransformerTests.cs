using System;
using System.IO;
using Microsoft.Quantum.Simulation.Circuitizer;
using Xunit;
using Circuitizer.Tests.Extensions;
using Microsoft.Quantum.QsCompiler.SyntaxTree;
using Microsoft.Quantum.QsCompiler.Diagnostics;
using Microsoft.VisualStudio.LanguageServer.Protocol;
using Microsoft.Quantum.QsCompiler;
using Microsoft.Quantum.Intrinsic;
using System.Linq;
using System.Collections.Generic;

namespace Circuitizer.Tests
{
    public class Logger : LogTracker
    {
        protected override void Print(Diagnostic msg)
            => Console.WriteLine(Formatting.MsBuildFormat(msg));
    }

    public class CircuitTransformerTests
    {
        public static QsNamespace[] LoadSyntaxTree(params string[] filenames)
        {
            var baseFolder = "RawInputs";
            var sources = filenames.Select(f => Path.Combine(baseFolder, f));
            var logger = new Logger();
            var loadOptions =
                new CompilationLoader.Configuration()
                {
                    GenerateFunctorSupport = true,
                };

            // How do we not hard code these paths?

            var references = new string[] { typeof(X).Assembly.Location };
            var loader = new CompilationLoader(sources, references, loadOptions, logger);
            var result = loader.GeneratedSyntaxTree.ToArray();

            return result;
        }

        [Fact]
        public void TestQRNG()
        {
            var transformation = new ClassicallyControlledConditions();

            var original = LoadSyntaxTree("QRNG.qs");
            var final = original.Select(transformation.Transform).ToArray();
        }
    }
}