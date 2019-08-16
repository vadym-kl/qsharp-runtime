using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.Quantum.Simulation.Core;

namespace Microsoft.Quantum.Simulation.Circuitizer
{
    public partial class CircuitizerSimulator
    {
        public class CircuitizerSimM : Quantum.Intrinsic.M
        {
            private CircuitizerSimulator Simulator { get; }

            public CircuitizerSimM(CircuitizerSimulator m) : base(m)
            {
                this.Simulator = m;
            }

            public override Func<Qubit, Result> Body => (q) =>
            {
                return Simulator.Circuitizer.M(q);
            };
        }
    }
}
