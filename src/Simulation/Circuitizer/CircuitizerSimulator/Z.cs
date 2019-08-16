using System;
using System.Runtime.InteropServices;
using Microsoft.Quantum.Intrinsic;
using Microsoft.Quantum.Simulation.Core;

namespace Microsoft.Quantum.Simulation.Circuitizer
{
    public partial class CircuitizerSimulator
    {
        public class CircuitizerSimZ : Quantum.Intrinsic.Z
        {
            private CircuitizerSimulator Simulator { get; }

            public CircuitizerSimZ(CircuitizerSimulator m) : base(m)
            {
                this.Simulator = m;
            }

            public override Func<Qubit, QVoid> Body => (q1) =>
            {

                Simulator.Circuitizer.Z(q1);
                return QVoid.Instance;
            };

            public override Func<(IQArray<Qubit>, Qubit), QVoid> ControlledBody => (_args) =>
            {

                (IQArray<Qubit> ctrls, Qubit q1) = _args;
                Simulator.Circuitizer.ControlledZ(ctrls, q1);
                return QVoid.Instance;
            };
        }
    }
}
