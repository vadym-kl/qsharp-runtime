using System;
using System.Runtime.InteropServices;
using Microsoft.Quantum.Simulation.Core;

namespace Microsoft.Quantum.Simulation.Circuitizer
{
    public partial class CircuitizerSimulator
    {
        public class CircuitizerSimSWAP : Quantum.Intrinsic.SWAP
        {
            private CircuitizerSimulator Simulator { get; }


            public CircuitizerSimSWAP(CircuitizerSimulator m) : base(m)
            {
                this.Simulator = m;
            }

            public override Func<(Qubit,Qubit), QVoid> Body => (q1) =>
            {

                Simulator.Circuitizer.SWAP(q1.Item1, q1.Item2);
                return QVoid.Instance;
            };

            public override Func<(IQArray<Qubit>, (Qubit, Qubit)), QVoid> ControlledBody => (args) =>
            {

                var (ctrls, q1) = args;
                Simulator.Circuitizer.ControlledSWAP(ctrls, q1.Item1, q1.Item2);
                return QVoid.Instance;
            };
        }
    }
}
