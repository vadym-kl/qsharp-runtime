﻿using System;
using System.Runtime.InteropServices;
using Microsoft.Quantum.Simulation.Common;
using Microsoft.Quantum.Simulation.Core;

namespace Microsoft.Quantum.Simulation.Circuitizer
{

    public partial class CircuitizerSimulator
    {
        public class CircuitizerSimRFrac : Quantum.Intrinsic.RFrac
        {

            private CircuitizerSimulator Simulator { get; }


            public CircuitizerSimRFrac(CircuitizerSimulator m) : base(m)
            {
                this.Simulator = m;
            }

            public override Func<(Pauli, long, long, Qubit), QVoid> Body => (_args) =>
            {

                var (basis, num, denom , q1) = _args;
                if (basis != Pauli.PauliI)
                {
                    var (numNew, denomNew) = CommonUtils.Reduce(num, denom);
                    Simulator.Circuitizer.RFrac(basis, numNew, denomNew, q1);
                }
                return QVoid.Instance;
            };

            public override Func<(Pauli, long, long, Qubit), QVoid> AdjointBody => (_args) =>
            {
                var (basis, num, denom, q1) = _args;
                return this.Body.Invoke((basis, -num, denom, q1));
            };

            public override Func<(IQArray<Qubit>, (Pauli, long, long, Qubit)), QVoid> ControlledBody => (_args) =>
            {

                var (ctrls, (basis, num, denom, q1)) = _args;
                var (numNew, denomNew) = CommonUtils.Reduce(num, denom);
                Simulator.Circuitizer.ControlledRFrac(ctrls, basis, numNew, denomNew, q1);
                return QVoid.Instance;
            };

            public override Func<(IQArray<Qubit>, (Pauli, long, long, Qubit)), QVoid> ControlledAdjointBody => (_args) =>
            {
                var (ctrls, (basis, num, denom, q1)) = _args;
                return this.ControlledBody.Invoke((ctrls, (basis, -num, denom, q1)));
            };
        }
    }
}
