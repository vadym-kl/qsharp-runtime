using System;
using System.Collections.Generic;
using Microsoft.Quantum.Simulation.Common;
using Microsoft.Quantum.Simulation.Core;

namespace Microsoft.Quantum.Simulation.Circuitizer
{
    public partial class CircuitizerSimulator
    {
        public class CircuitizerSimAssertProb : Quantum.Intrinsic.AssertProb
        {

            private CircuitizerSimulator Simulator { get; }


            public CircuitizerSimAssertProb(CircuitizerSimulator m) : base(m)
            {
                this.Simulator = m;
            }

            public override Func<(IQArray<Pauli>, IQArray<Qubit>, Result, double, string, double), QVoid> Body => (_args) =>
            {
                var (paulis, qubits, result, expectedPr, msg, tol) = _args;
                if (paulis.Length != qubits.Length)
                {
                    IgnorableAssert.Assert((paulis.Length != qubits.Length), "Arrays length mismatch");
                    throw new InvalidOperationException($"Both input arrays for {this.GetType().Name} (paulis,qubits), must be of same size.");
                }
                
                double probabilityOfZero = result == Result.Zero ? expectedPr : 1.0 - expectedPr;
                CommonUtils.PruneObservable(paulis, qubits, out QArray<Pauli> newPaulis, out QArray<Qubit> newQubits);
                Simulator.Circuitizer.AssertProb(newPaulis, newQubits, probabilityOfZero, msg, tol );
                return QVoid.Instance;
            };

            public override Func<(IQArray<Pauli>, IQArray<Qubit>, Result, double, string, double), QVoid> AdjointBody => (_args) => { return QVoid.Instance; };

            public override Func<(IQArray<Qubit>, (IQArray<Pauli>, IQArray<Qubit>, Result, double, string, double)), QVoid> ControlledBody => (_args) => { return QVoid.Instance; };

            public override Func<(IQArray<Qubit>, (IQArray<Pauli>, IQArray<Qubit>, Result, double, string, double)), QVoid> ControlledAdjointBody => (_args) => { return QVoid.Instance; };
        }
    }
}
