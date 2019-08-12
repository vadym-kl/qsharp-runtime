using System;
using System.IO;
using Microsoft.Quantum.Simulation.Circuitizer;
using Xunit;
using Circuitizer.Tests.Extensions;

namespace Circuitizer.Tests
{
    public class CircuitizerTests
    {
        [Fact]
        public void TestOneGate()
        {
            // Single character gate
            var circuitizer1 = new AsciiCircuitizer();
            var circuitizerSimulator1 = new CircuitizerSimulator(circuitizer1);
            var res = ZTest.Run(circuitizerSimulator1).Result;
            Assert.Equal(File.ReadAllText("output/Z.txt"), circuitizer1.ToString());

            // multi character gate
            var circuitizer2 = new AsciiCircuitizer();
            var circuitizerSimulator2 = new CircuitizerSimulator(circuitizer2);
            res = RyTest.Run(circuitizerSimulator2).Result;
            Assert.Equal(File.ReadAllText("output/Ry.txt"), circuitizer2.ToString());
        }

        [Fact]
        public void Connectedgates()
        {
            var circuitizer = new AsciiCircuitizer();
            var circuitizerSimulator = new CircuitizerSimulator(circuitizer);
            var res = ConnectedExpTest.Run(circuitizerSimulator).Result;
            Assert.Equal(File.ReadAllText("output/Exp.txt"), circuitizer.ToString());
        }

        [Fact]
        public void TestSimpleSwap()
        {
            var circuitizer = new AsciiCircuitizer();
            var circuitizerSimulator = new CircuitizerSimulator(circuitizer);
            var res = SwapTest.Run(circuitizerSimulator).Result;
            Assert.Equal(File.ReadAllText("output/Swap.txt"), circuitizer.ToString());
        }

        [Fact]
        public void TestSimpleControlled()
        {
            var circuitizer = new AsciiCircuitizer();
            var circuitizerSimulator = new CircuitizerSimulator(circuitizer);
            var res = ControlledXTest.Run(circuitizerSimulator).Result;
            Assert.Equal(File.ReadAllText("output/ControlledX.txt"), circuitizer.ToString());
        }

        [Fact]
        public void TestControlledSwap()
        {
            var circuitizer = new AsciiCircuitizer();
            var circuitizerSimulator = new CircuitizerSimulator(circuitizer);
            var res = ControlledSwapTest.Run(circuitizerSimulator).Result;
            Assert.Equal(File.ReadAllText("output/ControlledSwap.txt"), circuitizer.ToString());
        }

        [Fact]
        public void TestMeasure()
        {
            //single
            var circuitizer1 = new AsciiCircuitizer();
            var circuitizerSimulator1 = new CircuitizerSimulator(circuitizer1);
            var res = MTest.Run(circuitizerSimulator1).Result;
            Assert.Equal(File.ReadAllText("output/M.txt"), circuitizer1.ToString());

            // multiple
            var circuitizer2 = new AsciiCircuitizer();
            var circuitizerSimulator2 = new CircuitizerSimulator(circuitizer2);
            res = MeasureTest.Run(circuitizerSimulator2).Result;
            Assert.Equal(File.ReadAllText("output/Measure.txt"), circuitizer2.ToString());
        }

        [Fact]
        public void TestTeleport()
        {
            var circuitizer = new AsciiCircuitizer();
            var circuitizerSimulator = new CircuitizerSimulator(circuitizer);
            var res = TeleportCircuitTest.Run(circuitizerSimulator).Result;
            Assert.Equal(File.ReadAllText("output/Teleport.txt"), circuitizer.ToString());
        }

        [Fact]
        public void TestIntergration()
        {
            var circuitizer = new AsciiCircuitizer();
            var circuitizerSimulator = new CircuitizerSimulator(circuitizer);
            var res = IntegrateCircuitTest.Run(circuitizerSimulator).Result;
            Assert.Equal(File.ReadAllText("output/Intergration.txt"), circuitizer.ToString());
        }
    }
}