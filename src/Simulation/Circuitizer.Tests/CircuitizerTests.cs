using System;
using System.IO;
using Microsoft.Quantum.Simulation.Circuitizer;
using Xunit;
using Circuitizer.Tests.Extensions;
using Microsoft.Quantum.Simulation.Core;
using Xunit.Abstractions;

namespace Circuitizer.Tests
{
    public class CircuitizerTests
    {
        private readonly ITestOutputHelper output;

        public CircuitizerTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        private void AssertCircuit<Operation>(string filename)
            where Operation : Operation<QVoid, QVoid>
        {
            var ascii = File.ReadAllText(Path.Combine("ExpectedOutputs", "ASCII", filename));
            var qpic = File.ReadAllText(Path.Combine("ExpectedOutputs", "qpic", filename));

            var actual_ascii = AsciiCircuitizer.Print<Operation>();
            var actual_qpic = QPicCircuitizer.Print<Operation>();

            char[] trimChars = { ' ', '\n' };
            ascii = ascii.Replace("\r\n", "\n").TrimEnd(trimChars);
            actual_ascii = actual_ascii.Replace("\r\n", "\n").TrimEnd(trimChars);

            this.output.WriteLine($"Actual ASCII:\n{actual_ascii}\n\n");
            this.output.WriteLine($"Expected ASCII:\n{ascii}\n\n");
            // this.output.WriteLine($"Actual QPIC:\n{actual_qpic}");

            Assert.Equal(ascii, actual_ascii);
            // todo: Assert.Equal(qpic, actual_qpic);
        }

        private static void AssertCircuit(string filename, string actual)
        {
            var expected = File.ReadAllText(Path.Combine("ExpectedOutputs", filename));
            expected = expected.Replace("\r\n", "\n");
            actual = actual.Replace("\r\n", "\n");
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestBasicGate()
        {
            AssertCircuit<ZTest>("Z.txt");
        }

        [Fact]
        public void TestMulticharacterGate()
        {
            AssertCircuit<RyTest>("Ry.txt");
        }

        [Fact]
        public void Connectedgates()
        {
            AssertCircuit<ConnectedExpTest>("Exp.txt");
        }

        [Fact]
        public void TestSimpleSwap()
        {
            AssertCircuit<SwapTest>("Swap.txt");
        }

        [Fact]
        public void TestSimpleControlled()
        {
            AssertCircuit<ControlledXTest>("ControlledX.txt");
        }

        [Fact]
        public void TestControlledSwap()
        {
            AssertCircuit<ControlledSwapTest>("ControlledSwap.txt");
        }

        [Fact]
        public void TestM()
        {
            AssertCircuit<MTest>("M.txt");
        }

        [Fact]
        public void TestMeasure()
        {
            AssertCircuit<MeasureTest>("Measure.txt");
        }

        [Fact]
        public void TestTeleport()
        {
            AssertCircuit<TeleportCircuitTest>("Teleport.txt");
        }

        [Fact]
        public void TestIntergration()
        {
            AssertCircuit<IntegrateCircuitTest>("Intergration.txt");
        }
    }
}