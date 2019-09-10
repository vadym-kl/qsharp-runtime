using System;
using System.Runtime.InteropServices;
using Microsoft.Quantum.Simulation.Core;

namespace Microsoft.Quantum.Simulation.Circuitizer
{
    public partial class CircuitizerSimulator
    {
        public class CircuitizerSimReset : Quantum.Intrinsic.Reset
        {
            private CircuitizerSimulator Simulator { get; }


            public CircuitizerSimReset(CircuitizerSimulator m) : base(m)
            {
                this.Simulator = m;
            }

            public override Func<Qubit, QVoid> Body => (q1) =>
            {

                Simulator.Circuitizer.Reset(q1);
                return QVoid.Instance;
            };
        }
    }
}
