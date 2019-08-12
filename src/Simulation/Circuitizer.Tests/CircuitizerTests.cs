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
            // Single character gate
            var circuitizer1 = new AsciiCircuitizer();
            var circuitizerSimulato1 = new CircuitizerSimulator(circuitizer1);
            var res = CircuitizerTests.Z.Run(circuitizerSimulator1).Result;
            Assert.Equal(File.ReadAllText("output/Z.txt"), circuitizer1);

            // multi character gate
            var circuitizer2 = new AsciiCircuitizer();
            var circuitizerSimulator2 = new CircuitizerSimulator(circuitizer2);
            res = CircuitizerTests.RY.Run(circuitizerSimulator2).Result;
            Assert.Equal(File.ReadAllText("output/Ry.txt"), circuitizer2);
        }

        [Fact]
        public void Connectedgates()
        {
            var circuitizer = new AsciiCircuitizer();
            var circuitizerSimulator = new CircuitizerSimulator(circuitizer);
            var res = CircuitizerTests.ConnectedExp.Run(circuitizerSimulator).Result;
            Assert.Equal(File.ReadAllText("output/Exp.txt"), circuitizer);
        }

        [Fact]
        public void TestSimpleSwap()
        {
            var circuitizer = new AsciiCircuitizer();
            var circuitizerSimulator = new CircuitizerSimulator(circuitizer);
            var res = CircuitizerTests.Swap.Run(circuitizerSimulator).Result;
            Assert.Equal(File.ReadAllText("output/Swap.txt"), circuitizer);
        }

        [Fact]
        public void TestSimpleControlled()
        {
            var circuitizer = new AsciiCircuitizer();
            var circuitizerSimulator = new CircuitizerSimulator(circuitizer);
            var res = CircuitizerTests.ControlledX.Run(circuitizerSimulator).Result;
            Assert.Equal(File.ReadAllText("output/ControlledX.txt"), circuitizer);
        }

        [Fact]
        public void TestControlledSwap()
        {
            var circuitizer = new AsciiCircuitizer();
            var circuitizerSimulator = new CircuitizerSimulator(circuitizer);
            var res = CircuitizerTests.ControlledSwap.Run(circuitizerSimulator).Result;
            Assert.Equal(File.ReadAllText("output/ControlledSwap.txt"), circuitizer);
        }

        [Fact]
        public void TestMeasure()
        {
            //single
            var circuitizer1 = new AsciiCircuitizer();
            var circuitizerSimulator1 = new CircuitizerSimulator(circuitizer1);
            var res = CircuitizerTests.M.Run(circuitizerSimulator1).Result;
            Assert.Equal(File.ReadAllText("output/M.txt"), circuitizer1);

            // multiple
            var circuitizer2 = new AsciiCircuitizer();
            var circuitizerSimulator2 = new CircuitizerSimulator(circuitizer2);
            res = CircuitizerTests.Measure.Run(circuitizerSimulator2).Result;
            Assert.Equal(File.ReadAllText("output/Measure.txt"), circuitizer2);
        }

        [Fact]
        public void TestTeleport()
        {
            var circuitizer = new AsciiCircuitizer();
            var circuitizerSimulator = new CircuitizerSimulator(circuitizer);
            var res = CircuitizerTests.TeleportCircuit.Run(circuitizerSimulator).Result;
            Assert.Equal(File.ReadAllText("output/Teleport.txt"), circuitizer);
        }

        [Fact]
        public void TestIntergration()
        {
            var circuitizer = new AsciiCircuitizer();
            var circuitizerSimulator = new CircuitizerSimulator(circuitizer);
            var res = CircuitizerTests.IntergrateCircuit.Run(circuitizerSimulator).Result;
            Assert.Equal(File.ReadAllText("output/Intergration.txt"), circuitizer);
        }
    }
}