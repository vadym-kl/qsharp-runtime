using System;
using System.Runtime.InteropServices;
using Microsoft.Quantum.Simulation.Core;

namespace Microsoft.Quantum.Simulation.Circuitizer
{
    public partial class CircuitizerSimulator
    {
        public class CircuitizerSimX : Quantum.Intrinsic.X
        {
            private CircuitizerSimulator Simulator { get; }


            public CircuitizerSimX(CircuitizerSimulator m) : base(m)
            {
                this.Simulator = m;
            }

            public override Func<Qubit, QVoid> Body => (q1) =>
            {

                Simulator.Circuitizer.X(q1);
                return QVoid.Instance;
            };

            public override Func<(IQArray<Qubit>, Qubit), QVoid> ControlledBody => (args) =>
            {

                var (ctrls, q1) = args;
                Simulator.Circuitizer.ControlledX(ctrls, q1);
                return QVoid.Instance;
            };
        }
    }
}
