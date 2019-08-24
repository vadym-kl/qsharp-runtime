using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CommandLine;
using Microsoft.Quantum.QsCompiler;
using Microsoft.Quantum.QsCompiler.CsharpGeneration;
using Microsoft.Quantum.QsCompiler.DataTypes;
using Microsoft.Quantum.QsCompiler.Diagnostics;
using Microsoft.Quantum.QsCompiler.SyntaxTree;
using Microsoft.Quantum.QsCompiler.Transformations.BasicTransformations;
using Microsoft.Quantum.Simulation.Circuitizer;
using Microsoft.VisualStudio.LanguageServer.Protocol;

namespace Microsoft.Quantum.CircuitPrinter
{
    public class Logger : LogTracker
    {
        protected override void Print(Diagnostic msg)
            => Console.WriteLine(Formatting.MsBuildFormat(msg));
    }

    public class Program
    {
        Logger logger = new Logger();

        public enum OutputMode
        {
            ASCII = 0,
            QPIC = 1,
            PNG = 2,
            PDF = 3
        }

        class Options
        {
            [Option('i', "inputFile", Required = true, HelpText = "Input qsharp file to be processed.")]
            public string InputFile { get; set; }

            [Option('m', "method", Required = true, HelpText = "Fully qualified method/operation name.")]
            public string MethodName { get; set; }

            [Option('o', "outputFile", Required = true, HelpText = "Output file where result should be written.")]
            public string OutputFile { get; set; }

            [Option('f', "outputFormat", Required = true, HelpText = "Output result Format. Possible values are : ")]
            public OutputMode OutputFormat { get; set; }
        }

        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(opts => new Program().GenerateOutput(new List<string> {opts.InputFile}, opts.MethodName, opts.OutputFile, opts.OutputFormat))
                .WithNotParsed((errs) =>
                    Console.WriteLine(
                        @"A sample command is : dotnet run -- -i C:\q-circuit2\src\Simulation\CircuitTransformer.Tests\operation.qs -m Microsoft.Hack.Foo -o C:\q-circuit2\src\Simulation\CircuitTransformer.Tests\operation.qs.output -f QPIC"));
        }

        public void GenerateOutput(List<string> inputFiles, string methodName, string outputFileLocation, OutputMode outputMode)
        {
            Console.WriteLine("Starting processing.");

            var references = new List<string>()
            {
                @"C:\q-circuit\qsharp-runtime\src\Simulation\Common\bin\Debug\netstandard2.0\Microsoft.Quantum.Intrinsic.dll"
            };
            var loadOptions =
                new CompilationLoader.Configuration()
                {
                    GenerateFunctorSupport = true,
                    DocumentationOutputFolder = "output\\"
                };

            var loaded = new CompilationLoader(inputFiles, references, loadOptions, logger);
            var syntaxTree = loaded.GeneratedSyntaxTree.ToArray();
            // filter to keep only Q# files.
            var allSources = GetSourceFiles.Apply(syntaxTree).Where(x => x.Value.EndsWith(".qs"));
            foreach (var source in allSources)
            {
                GenerateOutputFromTree(syntaxTree, source, outputMode, methodName, outputFileLocation);
            }
        }

        public void GenerateOutputFromTree(QsNamespace[] syntaxTree, NonNullable<string> source, OutputMode outputMode, string methodName, string outputFileLocation)
        {
            try
            {
                Console.WriteLine("marker 12");

                // Apply transformation
                var transformer = new ClassicallyControlledConditions();
                var transformedTree = syntaxTree.Select(transformer.Transform).ToArray();
                var transformedCode = SimulationCode.generate(source, transformedTree);
                var messages = new List<string>();
                Console.WriteLine("marker 13");

                switch (outputMode)
                {
                    case OutputMode.ASCII:
                        var asciiCircuitizer = new AsciiCircuitizer();
                        var asciiSimulator = new CircuitizerSimulator(asciiCircuitizer);
                        asciiSimulator.OnLog += messages.Add;
                        Runner.Run(transformedCode, methodName, asciiSimulator);
                        Console.WriteLine(asciiCircuitizer);
                        File.WriteAllText(outputFileLocation, asciiCircuitizer.ToString());
                        break;
                    default:
                        // We currently only have qpic directive option.
                        var qpicCircuitizer = new QPicCircuitizer();
                        var qpicCircuitSimulator = new CircuitizerSimulator(qpicCircuitizer);
                        qpicCircuitSimulator.OnLog += messages.Add;
                        Runner.Run(transformedCode, methodName, qpicCircuitSimulator);
                        qpicCircuitizer.WriteToFile(outputFileLocation);
                        break;
                }

                foreach (var message in messages)
                {
                    Console.WriteLine(message);
                }

                Console.WriteLine("Finished processing.");
            }
            catch (Exception e)
            {
                logger.Log(e);
            }
        }
    }
}