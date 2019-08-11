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
            var circuitizer1 = new AsciiCircuitizer();
            var circuitizerSimulato1 = new CircuitizerSimulator(circuitizer1);

            // Single character gate
            var res = CircuitizerTests.Z.Run(circuitizerSimulator1).Result;
            Assert.Equal(File.Open("output/Z.txt", FileMode.Open), circuitizer1);

            // multi character gate
            var circuitizer2 = new AsciiCircuitizer();
            var circuitizerSimulato2 = new CircuitizerSimulator(circuitizer2);
            res = CircuitizerTests.RY.Run(circuitizerSimulator2).Result;
            Assert.Equal(File.Open("output/Ry.txt", FileMode.Open), circuitizer2);
        }

        [Fact]
        public void Connectedgates()
        {
            var circuitizer = new AsciiCircuitizer();
            var circuitizerSimulato2 = new CircuitizerSimulator(circuitizer);
            var res = CircuitizerTests.ConnectedExp.Run(circuitizerSimulator2).Result;
            Assert.Equal(File.Open("output/Exp.txt", FileMode.Open), circuitizer);
        }

        [Fact]
        public void TestSwap()
        {

        }

        [Fact]
        public void TestSimpleControlled()
        {

        }

        [Fact]
        public void TestControlledSwap()
        {

        }

        [Fact]
        public void TestMultipleControls()
        {

        }

        [Fact]
        public void TestMeasure()
        {
            // single 

            //multiple
        }

        [Fact]
        public void TestClassicallyControlled()
        {

        }
    }
}