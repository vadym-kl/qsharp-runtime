using Microsoft.Quantum.Simulation.Common;
using Microsoft.Quantum.Simulation.Core;
using System;
using System.Collections.Generic;

namespace Microsoft.Quantum.Simulation.Circuitizer
{
    public partial class CircuitizerSimulator
    {
        public class CircuitizerSimExpFrac : Quantum.Intrinsic.ExpFrac
        {
            private CircuitizerSimulator Simulator { get; }

            public CircuitizerSimExpFrac(CircuitizerSimulator m) : base(m) { this.Simulator = m; }

            public override Func<(IQArray<Pauli>, long, long, IQArray<Qubit>), QVoid> Body => (_args) =>
            {

                var (paulis, nom, den, qubits) = _args;

                if (paulis.Length != qubits.Length)
                {
                    throw new InvalidOperationException(
                        $"Both input arrays for {this.GetType().Name} (paulis,qubits), must be of same size.");
                }

                CommonUtils.PruneObservable(paulis, qubits, out QArray<Pauli> newPaulis, out QArray<Qubit> newQubits);

                Simulator.Circuitizer.ExpFrac(newPaulis, nom, den, newQubits);
                return QVoid.Instance;
            };

            public override Func<(IQArray<Pauli>, long, long, IQArray<Qubit>), QVoid> AdjointBody => (_args) =>
            {
                var (paulis, nom, den, qubits) = _args;
                return this.Body.Invoke((paulis, -nom, den, qubits));
            };

            public override Func<(IQArray<Qubit>, (IQArray<Pauli>, long, long, IQArray<Qubit>)), QVoid>
                ControlledBody => (_args) =>
                {
    
                    var (ctrls, (paulis, nom, den, qubits)) = _args;

                    if (paulis.Length != qubits.Length)
                    {
                        throw new InvalidOperationException(
                      $"Both input arrays for {this.GetType().Name} (paulis,qubits), must be of same size.");
                    }
                    CommonUtils.PruneObservable(paulis, qubits, out QArray<Pauli> newPaulis, out QArray<Qubit> newQubits);
                    Simulator.Circuitizer.ControlledExpFrac(ctrls, newPaulis, nom, den, newQubits);

                    return QVoid.Instance;
                };

            public override Func<(IQArray<Qubit>, (IQArray<Pauli>, long, long, IQArray<Qubit>)), QVoid>
                ControlledAdjointBody => (_args) =>
                {
                    var (ctrls, (paulis, nom, den, qubits)) = _args;

                    return this.ControlledBody.Invoke((ctrls, (paulis, -nom, den, qubits)));
                };
        }
    }
}
