using System;
using System.Runtime.InteropServices;
using Microsoft.Quantum.Simulation.Core;

namespace Microsoft.Quantum.Simulation.Circuitizer
{

    public partial class CircuitizerSimulator
    {
        public class CircuitizerSimR : Quantum.Intrinsic.R
        {

            private CircuitizerSimulator Simulator { get; }


            public CircuitizerSimR(CircuitizerSimulator m) : base(m)
            {
                this.Simulator = m;
            }

            public override Func<(Pauli, double, Qubit), QVoid> Body => (_args) =>
            {

                var (basis, angle, q1) = _args;
                if (basis != Pauli.PauliI)
                {
                    Simulator.Circuitizer.R(basis, angle,q1);
                }
                return QVoid.Instance;
            };

            public override Func<(Pauli, double, Qubit), QVoid> AdjointBody => (_args) =>
            {
                var (basis, angle, q1) = _args;
                return this.Body.Invoke((basis, -angle, q1));
            };

            public override Func<(IQArray<Qubit>, (Pauli, double, Qubit)), QVoid> ControlledBody => (_args) =>
            {

                var (ctrls, (basis, angle, q1)) = _args;
                Simulator.Circuitizer.ControlledR(ctrls, basis, angle, q1);
                return QVoid.Instance;
            };


            public override Func<(IQArray<Qubit>, (Pauli, double, Qubit)), QVoid> ControlledAdjointBody => (_args) =>
            {
                var (ctrls, (basis, angle, q1)) = _args;
                return this.ControlledBody.Invoke((ctrls, (basis, -angle, q1)));
            };
        }
    }
}
