using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Quantum.Simulation.Circuitizer;
using Microsoft.Quantum.Simulation.Simulators;
using Xunit;

namespace Circuitizer.Tests
{
    public class RunnerTests
    {
        [Fact]
        public void TestRun()
        {
            var messages = new List<string>();
            var code = File.ReadAllText("Operations.txt");
            using (var simulator = new QuantumSimulator())
            {
                simulator.OnLog += messages.Add;
                Runner.Run(code, "Quantum.QSharpLibrary1.HelloQLib", simulator);
            }
            Assert.Single(messages);
            Assert.Equal("Hello quantum world library!", messages[0]);
        }

        [Fact]
        public void TestRun2()
        {
            var messages = new List<string>();
            var code = File.ReadAllText("Operation.2.txt");
            using (var simulator = new QuantumSimulator())
            {
                simulator.OnLog += messages.Add;
                Runner.Run(code, "Microsoft.Hack.Foo", simulator);
            }
            Assert.Single(messages);
            Assert.Equal("Hello quantum world library!", messages[0]);
        }

        [Fact]
        public void TestInvalidCode()
        {
            var code = "Some Invalid c# code.";

            using (var simulator = new QuantumSimulator())
            {
                Assert.Throws<InvalidOperationException>(() => Runner.Run(code, "Foo.Bar", simulator));
            }
        }

        [Fact]
        public void TestInvalidOperation()
        {
            var code = File.ReadAllText("Operations.txt");
            using (var simulator = new QuantumSimulator())
            {
                Assert.Throws<InvalidOperationException>(() => Runner.Run(code, "Foo.Bar", simulator));
            }
        }


        [Fact]
        public void TestOperationArguments()
        {
            var code = File.ReadAllText("Operations.txt");
            using (var simulator = new QuantumSimulator())
            {
                Assert.Throws<InvalidOperationException>(() => Runner.Run(code, "Quantum.QSharpLibrary1.ExpectAguments", simulator));
            }
        }
    }
}
