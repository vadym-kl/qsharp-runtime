using System;
using System.IO;
using Microsoft.Quantum.Simulation.Circuitizer;
using Microsoft.Quantum.Simulation.Core;
using Microsoft.Quantum.Simulation.Simulators;
using Xunit;

namespace Circuitizer.Tests
{
    public class CircuitizerTests
    {
        [Fact]
        public void TestOneGate()
        {
            var circuitizer = new AsciiCircuitizer();
            var circuitizerSimulator = new CircuitizerSimulator(circuitizer);

            CircuitizerTests.Z.Run(circuitizerSimulator).Result;
            Assert.Equal(File.Open("output/Z.txt", FileMode.Open), circuitizer);
        }

    }
}