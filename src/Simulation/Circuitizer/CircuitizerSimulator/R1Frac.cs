using System;
using System.Runtime.InteropServices;
using Microsoft.Quantum.Simulation.Common;
using Microsoft.Quantum.Simulation.Core;

namespace Microsoft.Quantum.Simulation.Circuitizer
{

    public partial class CircuitizerSimulator
    {
        public class CircuitizerSimR1Frac : Quantum.Intrinsic.R1Frac
        {

            private CircuitizerSimulator Simulator { get; }


            public CircuitizerSimR1Frac(CircuitizerSimulator m) : base(m)
            {
                this.Simulator = m;
            }

            public override Func<(long, long, Qubit), QVoid> Body => (_args) =>
            {

                var (num, denom , q1) = _args;
                var (numNew, denomNew) = CommonUtils.Reduce(num, denom);
                Simulator.Circuitizer.R1Frac(numNew, denomNew, q1);
                return QVoid.Instance;
            };

            public override Func<(long, long, Qubit), QVoid> AdjointBody => (_args) =>
            {
                var (num, denom, q1) = _args;
                return this.Body.Invoke((-num, denom, q1));
            };

            public override Func<(IQArray<Qubit>, (long, long, Qubit)), QVoid> ControlledBody => (_args) =>
            {

                var (ctrls, (num, denom, q1)) = _args;
                var (numNew, denomNew) = CommonUtils.Reduce(num, denom);
                Simulator.Circuitizer.ControlledR1Frac(ctrls, numNew, denomNew, q1);
                return QVoid.Instance;
            };

            public override Func<(IQArray<Qubit>, (long, long, Qubit)), QVoid> ControlledAdjointBody => (_args) =>
            {
                var (ctrls, (num, denom, q1)) = _args;
                return this.ControlledBody.Invoke((ctrls, (-num, denom, q1)));
            };
        }
    }
}
