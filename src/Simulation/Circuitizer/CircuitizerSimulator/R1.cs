﻿using System;
using System.Runtime.InteropServices;
using Microsoft.Quantum.Simulation.Core;

namespace Microsoft.Quantum.Simulation.Circuitizer
{

    public partial class CircuitizerSimulator
    {
        public class CircuitizerSimR1 : Quantum.Intrinsic.R1
        {

            private CircuitizerSimulator Simulator { get; }


            public CircuitizerSimR1(CircuitizerSimulator m) : base(m)
            {
                this.Simulator = m;
            }

            public override Func<(double, Qubit), QVoid> Body => (_args) =>
            {

                var (angle, q1) = _args;
                Simulator.Circuitizer.R1(angle, q1);
                return QVoid.Instance;
            };

            public override Func<(double, Qubit), QVoid> AdjointBody => (_args) =>
            {
                var (angle, q1) = _args;
                return this.Body.Invoke((-angle, q1));
            };

            public override Func<(IQArray<Qubit>, ( double, Qubit)), QVoid> ControlledBody => (_args) =>
            {

                var (ctrls, (angle, q1)) = _args;
                Simulator.Circuitizer.ControlledR1(ctrls, angle, q1);
                return QVoid.Instance;
            };


            public override Func<(IQArray<Qubit>, (double, Qubit)), QVoid> ControlledAdjointBody => (_args) =>
            {
                var (ctrls, (angle, q1)) = _args;
                return this.ControlledBody.Invoke((ctrls, (-angle, q1)));
            };
        }
    }
}
