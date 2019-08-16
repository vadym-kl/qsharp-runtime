using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.Quantum.Simulation.Core;

namespace Microsoft.Quantum.Simulation.Circuitizer
{
    public static class Runner
    {
        public static Assembly CreateAssembly(string code)
        {
            var targetDll = "circuitizer";
            var references = References.Load();

            // Generate C# syntax tree:
            var tree = CSharpSyntaxTree.ParseText(code, encoding: UTF8Encoding.UTF8);
            var trees = new SyntaxTree[] { tree };

            // Compile the C# syntax trees:
            var options = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);

            var compilation = CSharpCompilation.Create(
                targetDll,
                trees,
                references,
                options);

            // Generate the assembly from the C# compilation:
            using (var ms = new MemoryStream())
            {
                EmitResult result = compilation.Emit(ms);

                if (!result.Success)
                {
                    throw new InvalidOperationException("Could not compile Roslyn dll from given C# code.");
                }
                else
                {
                    var data = ms.ToArray();
                    return Assembly.Load(data);
                }
            }
        }

        public static Type FindOperation(string fullName, Assembly assembly)
        {
            var op = assembly.GetType(fullName);

            if (op == null)
            {
                throw new InvalidOperationException($"Operation {fullName} was not found in assembly.");
            }

            return op;
        }

        public static MethodInfo FindRunMethod(Type op)
        {
            var run = op.GetMethod("Run");

            // TODO: Allow parameters.
            if (run.GetParameters().Length > 1)
            {
                throw new InvalidOperationException($"Only operation that receives no arguments can be circuitized.");
            }

            return run;
        }

        public static void Run(string code, string fullName, IOperationFactory machine)
        {
            var assembly = CreateAssembly(code);
            var op = FindOperation(fullName, assembly);
            var run = FindRunMethod(op);

            var args = new object[] { machine };
            var t = run.Invoke(null, args) as Task;

            t.Wait();
        }
    }
}
