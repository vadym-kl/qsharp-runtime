using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Quantum.Simulation.Core;

namespace Microsoft.Quantum.Simulation.Circuitizer
{
    /// <summary>
    /// An example of ICircuitizer that throws <see cref="NotImplementedException"/> for every call.
    /// </summary>
    class EmptyCircuitizer : ICircuitizer
    {
        public void Assert(IQArray<Pauli> bases, IQArray<Qubit> qubits, Result result, string msg)
        {
            throw new NotImplementedException();
        }

        public void AssertProb(IQArray<Pauli> bases, IQArray<Qubit> qubits, double probabilityOfZero, string msg, double tol)
        {
            throw new NotImplementedException();
        }

        public void ClassicallyControlled(Result measurementResult, Action onZero, Action onOne)
        {
            throw new NotImplementedException();
        }

        public void ControlledExp(IQArray<Qubit> controls, IQArray<Pauli> paulis, double theta, IQArray<Qubit> qubits)
        {
            throw new NotImplementedException();
        }

        public void ControlledExpFrac(IQArray<Qubit> controls, IQArray<Pauli> paulis, long numerator, long power, IQArray<Qubit> qubits)
        {
            throw new NotImplementedException();
        }

        public void ControlledH(IQArray<Qubit> controls, Qubit qubit)
        {
            throw new NotImplementedException();
        }

        public void ControlledR(IQArray<Qubit> controls, Pauli axis, double theta, Qubit qubit)
        {
            throw new NotImplementedException();
        }

        public void ControlledR1(IQArray<Qubit> controls, double theta, Qubit qubit)
        {
            throw new NotImplementedException();
        }

        public void ControlledR1Frac(IQArray<Qubit> controls, long numerator, long power, Qubit qubit)
        {
            throw new NotImplementedException();
        }

        public void ControlledRFrac(IQArray<Qubit> controls, Pauli axis, long numerator, long power, Qubit qubit)
        {
            throw new NotImplementedException();
        }

        public void ControlledS(IQArray<Qubit> controls, Qubit qubit)
        {
            throw new NotImplementedException();
        }

        public void ControlledSAdj(IQArray<Qubit> controls, Qubit qubit)
        {
            throw new NotImplementedException();
        }

        public void ControlledSWAP(IQArray<Qubit> controls, Qubit qubit1, Qubit qubit2)
        {
            throw new NotImplementedException();
        }

        public void ControlledT(IQArray<Qubit> controls, Qubit qubit)
        {
            throw new NotImplementedException();
        }

        public void ControlledTAdj(IQArray<Qubit> controls, Qubit qubit)
        {
            throw new NotImplementedException();
        }

        public void ControlledX(IQArray<Qubit> controls, Qubit qubit)
        {
            throw new NotImplementedException();
        }

        public void ControlledY(IQArray<Qubit> controls, Qubit qubit)
        {
            throw new NotImplementedException();
        }

        public void ControlledZ(IQArray<Qubit> controls, Qubit qubit)
        {
            throw new NotImplementedException();
        }

        public void Exp(IQArray<Pauli> paulis, double theta, IQArray<Qubit> qubits)
        {
            throw new NotImplementedException();
        }

        public void ExpFrac(IQArray<Pauli> paulis, long numerator, long power, IQArray<Qubit> qubits)
        {
            throw new NotImplementedException();
        }

        public void H(Qubit qubit)
        {
            throw new NotImplementedException();
        }

        public Result M(Qubit qubit)
        {
            throw new NotImplementedException();
        }

        public Result Measure(IQArray<Pauli> bases, IQArray<Qubit> qubits)
        {
            throw new NotImplementedException();
        }

        public void OnAllocateQubits(IQArray<Qubit> qubits)
        {
            throw new NotImplementedException();
        }

        public void OnBorrowQubits(IQArray<Qubit> qubits)
        {
            throw new NotImplementedException();
        }

        public void OnDump<T>(T location, IQArray<Qubit> qubits = null)
        {
            throw new NotImplementedException();
        }

        public void OnMessage(string msg)
        {
            throw new NotImplementedException();
        }

        public void OnOperationEnd(ICallable operation, IApplyData arguments)
        {
            throw new NotImplementedException();
        }

        public void OnOperationStart(ICallable operation, IApplyData arguments)
        {
            throw new NotImplementedException();
        }

        public void OnReleaseQubits(IQArray<Qubit> qubits)
        {
            throw new NotImplementedException();
        }

        public void OnReturnQubits(IQArray<Qubit> qubits)
        {
            throw new NotImplementedException();
        }

        public void R(Pauli axis, double theta, Qubit qubit)
        {
            throw new NotImplementedException();
        }

        public void R1(double theta, Qubit qubit)
        {
            throw new NotImplementedException();
        }

        public void R1Frac(long numerator, long power, Qubit qubit)
        {
            throw new NotImplementedException();
        }

        public void Reset(Qubit qubit)
        {
            throw new NotImplementedException();
        }

        public void RFrac(Pauli axis, long numerator, long power, Qubit qubit)
        {
            throw new NotImplementedException();
        }

        public void S(Qubit qubit)
        {
            throw new NotImplementedException();
        }

        public void SAdj(Qubit qubit)
        {
            throw new NotImplementedException();
        }

        public void SWAP(Qubit qubit1, Qubit qubit2)
        {
            throw new NotImplementedException();
        }

        public void T(Qubit qubit)
        {
            throw new NotImplementedException();
        }

        public void TAdj(Qubit qubit)
        {
            throw new NotImplementedException();
        }

        public void X(Qubit qubit)
        {
            throw new NotImplementedException();
        }

        public void Y(Qubit qubit)
        {
            throw new NotImplementedException();
        }

        public void Z(Qubit qubit)
        {
            throw new NotImplementedException();
        }
    }
}
