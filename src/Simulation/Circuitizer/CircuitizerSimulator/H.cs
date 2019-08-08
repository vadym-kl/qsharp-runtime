using System;
using System.Runtime.InteropServices;
using Microsoft.Quantum.Simulation.Core;

namespace Microsoft.Quantum.Simulation.Circuitizer
{
    public partial class CircuitizerSimulator
    {
        public class CircuitizerSimH : Quantum.Intrinsic.H
        {
            private CircuitizerSimulator Simulator { get; }


            public CircuitizerSimH(CircuitizerSimulator m) : base(m)
            {
                this.Simulator = m;
            }

            public override Func<Qubit, QVoid> Body => (q1) =>
            {

                Simulator.Circuitizer.H(q1);
                return QVoid.Instance;
            };


            public override Func<(IQArray<Qubit>, Qubit), QVoid> ControlledBody => (args) =>
            {

                var (ctrls, q1) = args;
                Simulator.Circuitizer.ControlledH(ctrls,q1);
                return QVoid.Instance;
            };
        }
    }
}
