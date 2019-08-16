using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.Quantum.Simulation.Common;
using Microsoft.Quantum.Simulation.Core;

namespace Microsoft.Quantum.Simulation.Circuitizer
{
    public partial class CircuitizerSimulator
    {
        public class CircuitizerSimMeasure : Quantum.Intrinsic.Measure
        {
            private CircuitizerSimulator Simulator { get; }


            public CircuitizerSimMeasure(CircuitizerSimulator m) : base(m)
            {
                this.Simulator = m;
            }

            public override Func<(IQArray<Pauli>, IQArray<Qubit>), Result> Body => (_args) =>
            {
                var (paulis, qubits) = _args;

                if (paulis.Length != qubits.Length)
                {
                    throw new InvalidOperationException($"Both input arrays for {this.GetType().Name} (paulis,qubits), must be of same size");
                }

                CommonUtils.PruneObservable(paulis, qubits, out QArray<Pauli> newPaulis, out QArray<Qubit> newQubits);
                return Simulator.Circuitizer.Measure( newPaulis, newQubits);
            };
        }
    }
}
