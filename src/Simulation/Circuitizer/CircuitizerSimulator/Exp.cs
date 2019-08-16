﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.Quantum.Simulation.Common;
using Microsoft.Quantum.Simulation.Core;

namespace Microsoft.Quantum.Simulation.Circuitizer
{
    public partial class CircuitizerSimulator
    {
        public class CircuitizerSimExp : Quantum.Intrinsic.Exp
        {
            private CircuitizerSimulator Simulator { get; }

            public CircuitizerSimExp(CircuitizerSimulator m) : base(m)
            {
                this.Simulator = m;
            }

            public override Func<(IQArray<Pauli>, double, IQArray<Qubit>), QVoid> Body => (_args) =>
            {

                var (paulis, theta, qubits) = _args;

                if (paulis.Length != qubits.Length)
                {
                    throw new InvalidOperationException($"Both input arrays for {this.GetType().Name} (paulis,qubits), must be of same size.");
                }

                CommonUtils.PruneObservable(paulis, qubits, out QArray<Pauli> newPaulis, out QArray<Qubit> newQubits);
                Simulator.Circuitizer.Exp(newPaulis, theta, newQubits);

                return QVoid.Instance;
            };

            public override Func<(IQArray<Pauli>, double, IQArray<Qubit>), QVoid> AdjointBody => (_args) =>
            {
                var (paulis, angle, qubits) = _args;
                return this.Body.Invoke((paulis, -angle, qubits));
            };

            public override Func<(IQArray<Qubit>, (IQArray<Pauli>, double, IQArray<Qubit>)), QVoid> ControlledBody => (_args) =>
            {

                var (ctrls, (paulis, theta, qubits)) = _args;

                CommonUtils.PruneObservable(paulis, qubits, out QArray<Pauli> newPaulis, out QArray<Qubit> newQubits);
                Simulator.Circuitizer.ControlledExp(ctrls, newPaulis, theta, newQubits);

                return QVoid.Instance;
            };

            public override Func<(IQArray<Qubit>, (IQArray<Pauli>, double, IQArray<Qubit>)), QVoid> ControlledAdjointBody => (_args) =>
            {
                var (ctrls, (paulis, angle, qubits)) = _args;

                return this.ControlledBody.Invoke((ctrls, (paulis, -angle, qubits)));
            };
        }
    }
}
