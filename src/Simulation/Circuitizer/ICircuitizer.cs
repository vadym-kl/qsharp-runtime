using System;
using Microsoft.Quantum.Simulation.Core;

namespace Microsoft.Quantum.Simulation.Circuitizer
{
    /// <summary>
    /// An interface for implementing QDK target machines which work 
    /// on a quantum circuit level. 
    /// It is intended to be used with ....
    /// </summary>
    public interface ICircuitizer
    {
        /// <summary>
        /// Called when <a href="https://docs.microsoft.com/en-gb/qsharp/api/qsharp/microsoft.quantum.intrinsic.r">Microsoft.Quantum.Intrinsic.R</a> is called in Q#.
        /// In Q# the operation applies 𝑒𝑥𝑝(-𝑖⋅<paramref name="theta"/>⋅<paramref name="axis"/>/2) to <paramref name="qubit"/>.  
        /// </summary>
        /// <remarks>
        /// When adjoint of R is called in Q#, <see cref="R(Pauli, double, Qubit)"/> is called with <paramref name="theta"/> replaced by -<paramref name="theta"/>.
        /// The names and the order of the parameters is the same as for the corresponding Q# operation.
        /// </remarks>
        /// <param name="axis">Pauli operator to be exponentiated to form the rotation.</param>
        /// <param name="theta">Angle about which the qubit is to be rotated.</param>
        /// <param name="qubit">Qubit to which the gate should be applied.</param>
        void R(Pauli axis, double theta, Qubit qubit);

        /// <summary>
        /// Called when controlled <a href="https://docs.microsoft.com/en-gb/qsharp/api/qsharp/microsoft.quantum.intrinsic.r">Microsoft.Quantum.Intrinsic.R</a> is called in Q#.
        /// In Q# the operation applies 𝑒𝑥𝑝(-𝑖⋅<paramref name="theta"/>⋅<paramref name="axis"/>/2) to <paramref name="qubit"/> controlled on <paramref name="controls"/>.  
        /// </summary>
        /// <remarks>
        /// When adjoint of Controlled R is called in Q#, <see cref="ControlledR(IQArray{Qubit}, Pauli, double, Qubit)"/> is called with <paramref name="theta"/> replaced by -<paramref name="theta"/>.
        /// The names and the order of the parameters is the same as for the corresponding Q# operation.
        /// </remarks>
        /// <param name="controls">The array of qubits on which the operation is controlled.</param>
        /// <param name="axis">Pauli operator to be exponentiated to form the rotation.</param>
        /// <param name="theta">Angle about which the qubit is to be rotated.</param>
        /// <param name="qubit">Qubit to which the gate should be applied.</param>
        void ControlledR(IQArray<Qubit> controls, Pauli axis, double theta, Qubit qubit);

        /// <summary>
        /// Called when <a href="https://docs.microsoft.com/en-gb/qsharp/api/qsharp/microsoft.quantum.intrinsic.rfrac">Microsoft.Quantum.Intrinsic.RFrac</a> is called in Q#.
        /// In Q# the operation applies 𝑒𝑥𝑝(𝑖⋅π⋅<paramref name="numerator"/>⋅<paramref name="axis"/>/2^<paramref name="power"/>) to <paramref name="qubit"/>.  
        /// </summary>
        /// <remarks>
        /// When adjoint of RFrac is called in Q#, <see cref="RFrac(Pauli, long, long, Qubit)"/> is called with <paramref name="numerator"/> replaced by -<paramref name="numerator"/>.
        /// The names and the order of the parameters is the same as for the corresponding Q# operation.
        /// </remarks>
        /// <param name="axis">Pauli operator to be exponentiated to form the rotation.</param>
        /// <param name="numerator">Numerator in the dyadic fraction representation of the angle by which the qubit is to be rotated.</param>
        /// <param name="power">Power of two specifying the denominator of the angle by which the qubit is to be rotated.</param>
        /// <param name="qubit">Qubit to which the gate should be applied.</param>
        void RFrac(Pauli axis, long numerator, long power, Qubit qubit);

        /// <summary>
        /// Called when a controlled <a href="https://docs.microsoft.com/en-gb/qsharp/api/qsharp/microsoft.quantum.intrinsic.rfrac">Microsoft.Quantum.Intrinsic.RFrac</a> is called in Q#.
        /// In Q# the operation applies 𝑒𝑥𝑝(𝑖⋅π⋅<paramref name="numerator"/>⋅<paramref name="axis"/>/2^<paramref name="power"/>) to <paramref name="qubit"/> controlled on <paramref name="controls"/>.
        /// </summary>
        /// <remarks>
        /// When adjoint of Controlled RFrac is called in Q#, <see cref="ControlledRFrac(IQArray{Qubit}, Pauli, long, long, Qubit)"/> is called with <paramref name="numerator"/> replaced by -<paramref name="numerator"/>.
        /// The names and the order of the parameters is the same as for the corresponding Q# operation.
        /// </remarks>
        /// <param name="controls">The array of qubits on which the operation is controlled.</param>
        /// <param name="axis">Pauli operator to be exponentiated to form the rotation.</param>
        /// <param name="numerator">Numerator in the dyadic fraction representation of the angle by which the qubit is to be rotated.</param>
        /// <param name="power">Power of two specifying the denominator of the angle by which the qubit is to be rotated.</param>
        /// <param name="qubit">Qubit to which the gate should be applied.</param>
        void ControlledRFrac(IQArray<Qubit> controls, Pauli axis, long numerator, long power, Qubit qubit);

        /// <summary>
        /// Called when <a href="https://docs.microsoft.com/en-gb/qsharp/api/qsharp/microsoft.quantum.intrinsic.exp">Microsoft.Quantum.Intrinsic.Exp</a> is called in Q#.
        /// In Q# the operation applies 𝑒𝑥𝑝(𝑖⋅<paramref name="angle"/>⋅<paramref name="pauli"/>) to <paramref name="target"/> qubits.  
        /// </summary>
        /// <remarks>
        /// When adjoint of Exp is called in Q#, <see cref="Exp(IQArray{Pauli}, double, IQArray{Qubit})"/> is called with <paramref name="angle"/> replaced by -<paramref name="angle"/>.
        /// The names and the order of the parameters is the same as for the corresponding Q# operation.
        /// </remarks>
        /// <param name="pauli">Array of single-qubit Pauli values representing a multi-qubit Pauli to be applied.</param>
        /// <param name="angle">Angle about the given multi-qubit Pauli operator by which the target register is to be rotated.</param>
        /// <param name="target">Register to apply the exponent to.</param>
        void Exp(IQArray<Pauli> pauli, double angle, IQArray<Qubit> target);

        /// <summary>
        /// Called when a controlled <a href="https://docs.microsoft.com/en-gb/qsharp/api/qsharp/microsoft.quantum.intrinsic.exp">Microsoft.Quantum.Intrinsic.Exp</a> is called in Q#.
        /// In Q# the operation applies 𝑒𝑥𝑝(𝑖⋅<paramref name="angle"/>⋅<paramref name="pauli"/>) to <paramref name="target"/> qubits controlled on <paramref name="controls"/>.  
        /// </summary>
        /// <remarks>
        /// When adjoint of Controlled Exp is called in Q#, <see cref="Exp(IQArray{Pauli}, double, IQArray{Qubit})"/> is called with <paramref name="angle"/> replaced by -<paramref name="angle"/>.
        /// The names and the order of the parameters is the same as for the corresponding Q# operation.
        /// </remarks>
        /// <param name="controls">The array of qubits on which the operation is controlled.</param>
        /// <param name="pauli">Array of single-qubit Pauli values representing a multi-qubit Pauli to be applied.</param>
        /// <param name="angle">Angle about the given multi-qubit Pauli operator by which the target register is to be rotated.</param>
        /// <param name="target">Register to apply the exponent to.</param>
        void ControlledExp(IQArray<Qubit> controls, IQArray<Pauli> pauli, double angle, IQArray<Qubit> target);

        void ExpFrac(IQArray<Pauli> pauli, long numerator, long denominator, IQArray<Qubit> target);
        void ControlledExpFrac(IQArray<Qubit> controls, IQArray<Pauli> pauli, long numerator, long denominator, IQArray<Qubit> target);

        void H(Qubit target);
        void ControlledH(IQArray<Qubit> controls, Qubit target);

        void S(Qubit target);
        void ControlledS(IQArray<Qubit> controls, Qubit target);
        void SInv(Qubit target);
        void ControlledSInv(IQArray<Qubit> controls, Qubit target);

        void T(Qubit target);
        void ControlledT(IQArray<Qubit> controls, Qubit target);
        void TInv(Qubit target);
        void ControlledTInv(IQArray<Qubit> controls, Qubit target);

        Result M(Qubit target);
        Result Measure(IQArray<Pauli> pauli, IQArray<Qubit> target);
        void Reset(Qubit target);

        void Assert(IQArray<Pauli> pauli, IQArray<Qubit> target, Result result, string message);
        void AssertProb(IQArray<Pauli> pauli, IQArray<Qubit> target, double probabilityOfZero, string message, double tolerance);

        void X(Qubit target);
        void ControlledX(IQArray<Qubit> controls, Qubit target);

        void Y(Qubit target);
        void ControlledY(IQArray<Qubit> controls, Qubit target);

        void Z(Qubit target);
        void ControlledZ(IQArray<Qubit> controls, Qubit target);

        void R1(double angle, Qubit target);
        void ControlledR1(IQArray<Qubit> controls, double angle, Qubit target);

        void R1Frac(long numerator, long denominator, Qubit target);
        void ControlledR1Frac(IQArray<Qubit> controls, long numerator, long denominator, Qubit target);

        void SWAP(Qubit q1, Qubit q2);
        void ControlledSWAP(IQArray<Qubit> controls, Qubit q1, Qubit q2);

        void OnOperationStart(ICallable operation, IApplyData arguments);
        void OnOperationEnd(ICallable operation, IApplyData arguments);

        void ClassicallyControlled(Result measurementResult, Action onZero, Action onOne);

        void OnAllocateQubits(IQArray<Qubit> qubits);
        void OnReleaseQubits(IQArray<Qubit> qubits);
        void OnBorrowQubits(IQArray<Qubit> qubits);
        void OnReturnQubits(IQArray<Qubit> qubits);
    }
}
