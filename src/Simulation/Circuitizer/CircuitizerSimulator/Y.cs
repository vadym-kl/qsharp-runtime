using System;
using System.Runtime.InteropServices;
using Microsoft.Quantum.Simulation.Core;

namespace Microsoft.Quantum.Simulation.Circuitizer
{
    public partial class CircuitizerSimulator
    {
        public class CircuitizerSimY : Quantum.Intrinsic.Y
        {
            private CircuitizerSimulator Simulator { get; }


            public CircuitizerSimY(CircuitizerSimulator m) : base(m)
            {
                this.Simulator = m;
            }

            public override Func<Qubit, QVoid> Body => (q1) =>
            {

                Simulator.Circuitizer.Y(q1);
                return QVoid.Instance;
            };

            public override Func<(IQArray<Qubit>, Qubit), QVoid> ControlledBody => (_args) =>
            {

                (IQArray<Qubit> ctrls, Qubit q1) = _args;
                Simulator.Circuitizer.ControlledY(ctrls, q1);
                return QVoid.Instance;
            };            
        }
    }
}
