using System;
using System.Runtime.InteropServices;
using Microsoft.Quantum.Simulation.Core;

namespace Microsoft.Quantum.Simulation.Circuitizer
{
    public partial class CircuitizerSimulator
    {
        public class CircuitizerSimS : Quantum.Intrinsic.S
        {
            private CircuitizerSimulator Simulator { get; }

            public CircuitizerSimS(CircuitizerSimulator m) : base(m)
            {
                this.Simulator = m;
            }

            public override Func<Qubit, QVoid> Body => (q1) =>
            {

                Simulator.Circuitizer.S(q1);
                return QVoid.Instance;
            };

            public override Func<(IQArray<Qubit>, Qubit), QVoid> ControlledBody => (_args) =>
            {

                (IQArray<Qubit> ctrls, Qubit q1) = _args;
                Simulator.Circuitizer.ControlledS(ctrls, q1);
                return QVoid.Instance;
            };

            public override Func<Qubit, QVoid> AdjointBody => (q1) =>
            {

                Simulator.Circuitizer.SAdj(q1);
                return QVoid.Instance;
            };

            public override Func<(IQArray<Qubit>, Qubit), QVoid> ControlledAdjointBody => (_args) =>
            {

                (IQArray<Qubit> ctrls, Qubit q1) = _args;
                Simulator.Circuitizer.ControlledSAdj(ctrls, q1);
                return QVoid.Instance;
            };
        }
    }
}
