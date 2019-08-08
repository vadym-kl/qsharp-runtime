﻿using System;
using Microsoft.Quantum.Simulation.Core;

namespace Microsoft.Quantum.Simulation.Circuitizer
{
    /// <summary>
    /// An interface for implementing QDK target machines which work 
    /// on a quantum circuit level. 
    /// It is intended to be used with <see cref="CircuitizerSimulator"/>.
    /// </summary>
    /// <remarks>
    /// Simulators implemented using <see cref="ICircuitizer"/> interface do not manage qubits on their own.
    /// Instead they are notified when qubits are allocated, released, borrowed and returned.
    /// </remarks>
    public interface ICircuitizer
    {
        /// <summary>
        /// Called when <a href="https://docs.microsoft.com/qsharp/api/qsharp/microsoft.quantum.intrinsic.r">Microsoft.Quantum.Intrinsic.R</a> is called in Q#.
        /// In Q# the operation applies 𝑒𝑥𝑝(-𝑖⋅<paramref name="theta"/>⋅<paramref name="axis"/>/2) to <paramref name="qubit"/>.  
        /// </summary>
        /// <remarks>
        /// When adjoint of R is called in Q#, <see cref="R(Pauli, double, Qubit)"/> is called with <paramref name="theta"/> replaced by -<paramref name="theta"/>.
        /// The names and the order of the parameters are the same as for the corresponding Q# operation.
        /// </remarks>
        /// <param name="axis">Pauli operator to be exponentiated to form the rotation.</param>
        /// <param name="theta">Angle about which the qubit is to be rotated.</param>
        /// <param name="qubit">Qubit to which the gate should be applied.</param>
        void R(Pauli axis, double theta, Qubit qubit);

        /// <summary>
        /// Called when controlled <a href="https://docs.microsoft.com/qsharp/api/qsharp/microsoft.quantum.intrinsic.r">Microsoft.Quantum.Intrinsic.R</a> is called in Q#.
        /// In Q# the operation applies 𝑒𝑥𝑝(-𝑖⋅<paramref name="theta"/>⋅<paramref name="axis"/>/2) to <paramref name="qubit"/> controlled on <paramref name="controls"/>.  
        /// </summary>
        /// <remarks>
        /// When adjoint of Controlled R is called in Q#, <see cref="ControlledR(IQArray{Qubit}, Pauli, double, Qubit)"/> is called with <paramref name="theta"/> replaced by -<paramref name="theta"/>.
        /// The names and the order of the parameters are the same as for the corresponding Q# operation.
        /// </remarks>
        /// <param name="controls">The array of qubits on which the operation is controlled.</param>
        /// <param name="axis">Pauli operator to be exponentiated to form the rotation.</param>
        /// <param name="theta">Angle about which the qubit is to be rotated.</param>
        /// <param name="qubit">Qubit to which the gate should be applied.</param>
        void ControlledR(IQArray<Qubit> controls, Pauli axis, double theta, Qubit qubit);

        /// <summary>
        /// Called when <a href="https://docs.microsoft.com/qsharp/api/qsharp/microsoft.quantum.intrinsic.rfrac">Microsoft.Quantum.Intrinsic.RFrac</a> is called in Q#.
        /// In Q# the operation applies 𝑒𝑥𝑝(𝑖⋅π⋅<paramref name="numerator"/>⋅<paramref name="axis"/>/2^<paramref name="power"/>) to <paramref name="qubit"/>.  
        /// </summary>
        /// <remarks>
        /// When adjoint of RFrac is called in Q#, <see cref="RFrac(Pauli, long, long, Qubit)"/> is called with <paramref name="numerator"/> replaced by -<paramref name="numerator"/>.
        /// The names and the order of the parameters are the same as for the corresponding Q# operation.
        /// </remarks>
        /// <param name="axis">Pauli operator to be exponentiated to form the rotation.</param>
        /// <param name="numerator">Numerator in the dyadic fraction representation of the angle by which the qubit is to be rotated.</param>
        /// <param name="power">Power of two specifying the denominator of the angle by which the qubit is to be rotated.</param>
        /// <param name="qubit">Qubit to which the gate should be applied.</param>
        void RFrac(Pauli axis, long numerator, long power, Qubit qubit);

        /// <summary>
        /// Called when a controlled <a href="https://docs.microsoft.com/qsharp/api/qsharp/microsoft.quantum.intrinsic.rfrac">Microsoft.Quantum.Intrinsic.RFrac</a> is called in Q#.
        /// In Q# the operation applies 𝑒𝑥𝑝(𝑖⋅π⋅<paramref name="numerator"/>⋅<paramref name="axis"/>/2^<paramref name="power"/>) to <paramref name="qubit"/> controlled on <paramref name="controls"/>.
        /// </summary>
        /// <remarks>
        /// When adjoint of Controlled RFrac is called in Q#, <see cref="ControlledRFrac(IQArray{Qubit}, Pauli, long, long, Qubit)"/> is called with <paramref name="numerator"/> replaced by -<paramref name="numerator"/>.
        /// The names and the order of the parameters are the same as for the corresponding Q# operation.
        /// </remarks>
        /// <param name="controls">The array of qubits on which the operation is controlled.</param>
        /// <param name="axis">Pauli operator to be exponentiated to form the rotation.</param>
        /// <param name="numerator">Numerator in the dyadic fraction representation of the angle by which the qubit is to be rotated.</param>
        /// <param name="power">Power of two specifying the denominator of the angle by which the qubit is to be rotated.</param>
        /// <param name="qubit">Qubit to which the gate should be applied.</param>
        void ControlledRFrac(IQArray<Qubit> controls, Pauli axis, long numerator, long power, Qubit qubit);

        /// <summary>
        /// Called when <a href="https://docs.microsoft.com/qsharp/api/qsharp/microsoft.quantum.intrinsic.exp">Microsoft.Quantum.Intrinsic.Exp</a> is called in Q#.
        /// In Q# the operation applies 𝑒𝑥𝑝(𝑖⋅<paramref name="theta"/>⋅<paramref name="paulis"/>) to <paramref name="qubits"/>.  
        /// </summary>
        /// <remarks>
        /// When adjoint of Exp is called in Q#, <see cref="Exp(IQArray{Pauli}, double, IQArray{Qubit})"/> is called with <paramref name="theta"/> replaced by -<paramref name="theta"/>.
        /// The names and the order of the parameters are the same as for the corresponding Q# operation.
        /// </remarks>
        /// <param name="paulis">Array of single-qubit Pauli values representing a multi-qubit Pauli to be applied.</param>
        /// <param name="theta">Angle about the given multi-qubit Pauli operator by which the target register is to be rotated.</param>
        /// <param name="qubits">Register to apply the exponent to.</param>
        void Exp(IQArray<Pauli> paulis, double theta, IQArray<Qubit> qubits);

        /// <summary>
        /// Called when a controlled <a href="https://docs.microsoft.com/qsharp/api/qsharp/microsoft.quantum.intrinsic.exp">Microsoft.Quantum.Intrinsic.Exp</a> is called in Q#.
        /// In Q# the operation applies 𝑒𝑥𝑝(𝑖⋅<paramref name="theta"/>⋅<paramref name="paulis"/>) to <paramref name="qubits"/> controlled on <paramref name="controls"/>.  
        /// </summary>
        /// <remarks>
        /// When adjoint of Controlled Exp is called in Q#, <see cref="ControlledExp(IQArray{Qubit}, IQArray{Pauli}, double, IQArray{Qubit})"/> is called with <paramref name="theta"/> replaced by -<paramref name="theta"/>.
        /// The names and the order of the parameters are the same as for the corresponding Q# operation.
        /// </remarks>
        /// <param name="controls">The array of qubits on which the operation is controlled.</param>
        /// <param name="paulis">Array of single-qubit Pauli values representing a multi-qubit Pauli to be applied.</param>
        /// <param name="theta">Angle about the given multi-qubit Pauli operator by which the target register is to be rotated.</param>
        /// <param name="qubits">Register to apply the exponent to.</param>
        void ControlledExp(IQArray<Qubit> controls, IQArray<Pauli> paulis, double theta, IQArray<Qubit> qubits);

        /// <summary>
        /// Called when <a href="https://docs.microsoft.com/qsharp/api/qsharp/microsoft.quantum.intrinsic.expfrac">Microsoft.Quantum.Intrinsic.ExpFrac</a> is called in Q#.
        /// In Q# the operation applies 𝑒𝑥𝑝(𝑖⋅π⋅<paramref name="numerator"/>⋅<paramref name="paulis"/>/2^<paramref name="power"/>) to <paramref name="qubits"/>.
        /// </summary>
        /// <remarks>
        /// When adjoint of ExpFrac is called in Q#, <see cref="ExpFrac(IQArray{Pauli}, long, long, IQArray{Qubit})"/> is called with <paramref name="numerator"/> replaced by -<paramref name="numerator"/>.
        /// The names and the order of the parameters are the same as for the corresponding Q# operation.
        /// </remarks>
        /// <param name="paulis">Array of single-qubit Pauli values representing a multi-qubit Pauli to be applied.</param>
        /// <param name="numerator">Numerator in the dyadic fraction representation of the angle by which the qubit register is to be rotated.</param>
        /// <param name="power">Power of two specifying the denominator of the angle by which the qubit register is to be rotated.</param>
        /// <param name="qubits">Register to apply the exponent to.</param>
        void ExpFrac(IQArray<Pauli> paulis, long numerator, long power, IQArray<Qubit> qubits);

        /// <summary>
        /// Called when controlled <a href="https://docs.microsoft.com/qsharp/api/qsharp/microsoft.quantum.intrinsic.expfrac">Microsoft.Quantum.Intrinsic.ExpFrac</a> is called in Q#.
        /// In Q# the operation applies 𝑒𝑥𝑝(𝑖⋅π⋅<paramref name="numerator"/>⋅<paramref name="paulis"/>/2^<paramref name="power"/>) to <paramref name="qubits"/> controlled on <paramref name="controls"/>.
        /// </summary>
        /// <remarks>
        /// When adjoint of Controlled ExpFrac is called in Q#, <see cref="ControlledExpFrac(IQArray{Qubit}, IQArray{Pauli}, long, long, IQArray{Qubit})"/> is called with <paramref name="numerator"/> replaced by -<paramref name="numerator"/>.
        /// The names and the order of the parameters are the same as for the corresponding Q# operation.
        /// </remarks>
        /// <param name="controls">The array of qubits on which the operation is controlled.</param>
        /// <param name="paulis">Array of single-qubit Pauli values representing a multi-qubit Pauli to be applied.</param>
        /// <param name="numerator">Numerator in the dyadic fraction representation of the angle by which the qubit register is to be rotated.</param>
        /// <param name="power">Power of two specifying the denominator of the angle by which the qubit register is to be rotated.</param>
        /// <param name="qubits">Register to apply the exponent to.</param>
        void ControlledExpFrac(IQArray<Qubit> controls, IQArray<Pauli> paulis, long numerator, long power, IQArray<Qubit> qubits);

        /// <summary>
        /// Called when <a href="https://docs.microsoft.com/qsharp/api/qsharp/microsoft.quantum.intrinsic.h">Microsoft.Quantum.Intrinsic.H</a> is called in Q#.
        /// In Q# the operation applies Hadamard gate to <paramref name="qubit"/>. The gate is given by matrix H=((1,1),(1,-1))/√2.
        /// </summary>
        /// <remarks>
        /// When adjoint of H is called in Q#, <see cref="H(Qubit)"/> is called because Hadamard is self-adjoint.
        /// The names and the order of the parameters are the same as for the corresponding Q# operation.
        /// </remarks>
        /// <param name="qubit">Qubit to which the gate should be applied.</param>
        void H(Qubit qubit);

        /// <summary>
        /// Called when controlled <a href="https://docs.microsoft.com/qsharp/api/qsharp/microsoft.quantum.intrinsic.h">Microsoft.Quantum.Intrinsic.H</a> is called in Q#.
        /// In Q# the operation applies Hadamard gate to <paramref name="qubit"/> controlled on <paramref name="controls"/>. The gate is given by matrix H=((1,1),(1,-1))/√2.
        /// </summary>
        /// <remarks>
        /// When adjoint of Controlled H is called in Q#, <see cref="ControlledH(IQArray{Qubit}, Qubit)"/> is called because Hadamard is self-adjoint.
        /// The names and the order of the parameters are the same as for the corresponding Q# operation.
        /// </remarks>
        /// <param name="controls">The array of qubits on which the operation is controlled.</param>
        /// <param name="qubit">Qubit to which the gate should be applied.</param>
        void ControlledH(IQArray<Qubit> controls, Qubit qubit);

        /// <summary>
        /// Called when <a href="https://docs.microsoft.com/qsharp/api/qsharp/microsoft.quantum.intrinsic.s">Microsoft.Quantum.Intrinsic.S</a> is called in Q#.
        /// In Q# the operation applies S gate to <paramref name="qubit"/>. The gate is given by matrix S=((1,0),(0,𝑖)).
        /// </summary>
        /// <remarks>
        /// When adjoint of S is called in Q#, <see cref="SAdj(Qubit)"/> is called.
        /// The names and the order of the parameters are the same as for the corresponding Q# operation.
        /// </remarks>
        /// <param name="qubit">Qubit to which the gate should be applied.</param>
        void S(Qubit qubit);

        /// <summary>
        /// Called when controlled <a href="https://docs.microsoft.com/qsharp/api/qsharp/microsoft.quantum.intrinsic.s">Microsoft.Quantum.Intrinsic.S</a> is called in Q#.
        /// In Q# the operation applies S gate to <paramref name="qubit"/> controlled on <paramref name="controls"/>. The gate is given by matrix S=((1,0),(0,𝑖)).
        /// </summary>
        /// <remarks>
        /// When adjoint of Controlled S is called in Q#, <see cref="ControlledSAdj(IQArray{Qubit}, Qubit)"/> is called.
        /// The names and the order of the parameters are the same as for the corresponding Q# operation.
        /// </remarks>
        /// <param name="controls">The array of qubits on which the operation is controlled.</param>
        /// <param name="qubit">Qubit to which the gate should be applied.</param>
        void ControlledS(IQArray<Qubit> controls, Qubit qubit);

        /// <summary>
        /// Called when adjoint <a href="https://docs.microsoft.com/qsharp/api/qsharp/microsoft.quantum.intrinsic.s">Microsoft.Quantum.Intrinsic.S</a> is called in Q#.
        /// In Q# the operation applies S† gate to <paramref name="qubit"/>. The gate is given by matrix S†=((1,0),(0,-𝑖)).
        /// </summary>
        /// <remarks>
        /// When adjoint of Adjoint S is called in Q#, <see cref="S(Qubit)"/> is called.
        /// The names and the order of the parameters are the same as for the corresponding Q# operation.
        /// </remarks>
        /// <param name="qubit">Qubit to which the gate should be applied.</param>
        void SAdj(Qubit qubit);

        /// <summary>
        /// Called when controlled adjoint <a href="https://docs.microsoft.com/qsharp/api/qsharp/microsoft.quantum.intrinsic.s">Microsoft.Quantum.Intrinsic.S</a> is called in Q#.
        /// In Q# the operation applies S† gate to <paramref name="qubit"/> controlled on <paramref name="controls"/>. The gate is given by matrix S†=((1,0),(0,𝑖)).
        /// </summary>
        /// <remarks>
        /// When adjoint of Controlled S† is called in Q#, <see cref="ControlledS(IQArray{Qubit}, Qubit)"/> is called.
        /// The names and the order of the parameters are the same as for the corresponding Q# operation.
        /// </remarks>
        /// <param name="controls">The array of qubits on which the operation is controlled.</param>
        /// <param name="qubit">Qubit to which the gate should be applied.</param>
        void ControlledSAdj(IQArray<Qubit> controls, Qubit qubit);

        /// <summary>
        /// Called when <a href="https://docs.microsoft.com/qsharp/api/qsharp/microsoft.quantum.intrinsic.t">Microsoft.Quantum.Intrinsic.T</a> is called in Q#.
        /// In Q# the operation applies T gate to <paramref name="qubit"/>. The gate is given by matrix T=((1,0),(0,𝑒𝑥𝑝(𝑖⋅π/4))).
        /// </summary>
        /// <remarks>
        /// When adjoint of T is called in Q#, <see cref="TAdj(Qubit)"/> is called.
        /// The names and the order of the parameters are the same as for the corresponding Q# operation.
        /// </remarks>
        /// <param name="qubit">Qubit to which the gate should be applied.</param>
        void T(Qubit qubit);

        /// <summary>
        /// Called when controlled <a href="https://docs.microsoft.com/qsharp/api/qsharp/microsoft.quantum.intrinsic.t">Microsoft.Quantum.Intrinsic.T</a> is called in Q#.
        /// In Q# the operation applies T gate to <paramref name="qubit"/> controlled on <paramref name="controls"/>. The gate is given by matrix T=((1,0),(0,𝑒𝑥𝑝(𝑖⋅π/4))).
        /// </summary>
        /// <remarks>
        /// When adjoint of Controlled T is called in Q#, <see cref="ControlledTsAdj(IQArray{Qubit}, Qubit)"/> is called.
        /// The names and the order of the parameters are the same as for the corresponding Q# operation.
        /// </remarks>
        /// <param name="controls">The array of qubits on which the operation is controlled.</param>
        /// <param name="qubit">Qubit to which the gate should be applied.</param>
        void ControlledT(IQArray<Qubit> controls, Qubit qubit);

        /// <summary>
        /// Called when adjoint <a href="https://docs.microsoft.com/qsharp/api/qsharp/microsoft.quantum.intrinsic.t">Microsoft.Quantum.Intrinsic.T</a> is called in Q#.
        /// In Q# the operation applies T† gate to <paramref name="qubit"/>. The gate is given by matrix T†=((1,0),(0,𝑒𝑥𝑝(-𝑖⋅π/4))).
        /// </summary>
        /// <remarks>
        /// When adjoint of Adjoint T is called in Q#, <see cref="T(Qubit)"/> is called.
        /// The names and the order of the parameters are the same as for the corresponding Q# operation.
        /// </remarks>
        /// <param name="qubit">Qubit to which the gate should be applied.</param>
        void TAdj(Qubit qubit);

        /// <summary>
        /// Called when controlled adjoint <a href="https://docs.microsoft.com/qsharp/api/qsharp/microsoft.quantum.intrinsic.t">Microsoft.Quantum.Intrinsic.T</a> is called in Q#.
        /// In Q# the operation applies T† gate to <paramref name="qubit"/> controlled on <paramref name="controls"/>. The gate is given by matrix T†=((1,0),(0,𝑒𝑥𝑝(-𝑖⋅π/4))).
        /// </summary>
        /// <remarks>
        /// When adjoint of Controlled T† is called in Q#, <see cref="ControlledT(IQArray{Qubit}, Qubit)"/> is called.
        /// The names and the order of the parameters are the same as for the corresponding Q# operation.
        /// </remarks>
        /// <param name="controls">The array of qubits on which the operation is controlled.</param>
        /// <param name="qubit">Qubit to which the gate should be applied.</param>
        void ControlledTAdj(IQArray<Qubit> controls, Qubit qubit);

        /// <summary>
        /// Called when <a href="https://docs.microsoft.com/qsharp/api/qsharp/microsoft.quantum.intrinsic.m">Microsoft.Quantum.Intrinsic.M</a> is called in Q#.
        /// In Q# the operation measures <paramref name="qubit"/> in Z basis, in other words in the computational basis.
        /// </summary>
        /// <remarks>
        /// The names and the order of the parameters are the same as for the corresponding Q# operation.
        /// Class implementing <see cref="ICircuitizer"/> interface can return any class derived from <see cref="Result"/>.
        /// </remarks>
        /// <param name="qubit">Qubit to which the gate should be applied.</param>
        /// <returns> Zero if the +1 eigenvalue is observed, and One if the -1 eigenvalue is observed.</returns>
        Result M(Qubit qubit);

        /// <summary>
        /// Called when <a href="https://docs.microsoft.com/qsharp/api/qsharp/microsoft.quantum.intrinsic.measure">Microsoft.Quantum.Intrinsic.Measure</a> is called in Q#.
        /// In Q# the operation measures multi-qubit Pauli observable given by <paramref name="bases"/> on <paramref name="qubits"/>.
        /// </summary>
        /// <remarks>
        /// The names and the order of the parameters are the same as for the corresponding Q# operation.
        /// Class implementing <see cref="ICircuitizer"/> interface can return any class derived from <see cref="Result"/>.
        /// </remarks>
        /// <param name="qubits">Qubits to which the gate should be applied.</param>
        /// <param name="bases">Array of single-qubit Pauli values describing multi-qubit Pauli observable.</param>
        /// <returns> Zero if the +1 eigenvalue is observed, and One if the -1 eigenvalue is observed.</returns>
        Result Measure(IQArray<Pauli> bases, IQArray<Qubit> qubits);

        /// <summary>
        /// Called when <a href="https://docs.microsoft.com/qsharp/api/qsharp/microsoft.quantum.intrinsic.reset">Microsoft.Quantum.Intrinsic.Reset</a> is called in Q#.
        /// In Q# the operation, measures <paramref name="qubit"/> and ensures it is in the |0⟩ state such that it can be safely released.
        /// </summary>
        /// <remarks>
        /// The names and the order of the parameters are the same as for the corresponding Q# operation.
        /// </remarks>
        /// <param name="qubit">Qubit to which the gate should be applied.</param>
        void Reset(Qubit qubit);

        /// <summary>
        /// Called when <a href="https://docs.microsoft.com/qsharp/api/qsharp/microsoft.quantum.intrinsic.assert">Microsoft.Quantum.Intrinsic.Assert</a> is called in Q#.
        /// </summary>
        /// <remarks>
        /// The names and the order of the parameters are the same as for the corresponding Q# operation.
        /// </remarks>
        /// <param name="bases">A multi-qubit Pauli operator, for which the measurement outcome is asserted.</param>
        /// <param name="qubits">A register on which to make the assertion.</param>
        /// <param name="result">The expected result of <c>Measure(bases, qubits)</c> </param>
        /// <param name="msg">A message to be reported if the assertion fails.</param>
        void Assert(IQArray<Pauli> bases, IQArray<Qubit> qubits, Result result, string msg);

        /// <summary>
        /// Called when <a href="https://docs.microsoft.com/qsharp/api/qsharp/microsoft.quantum.intrinsic.assert">Microsoft.Quantum.Intrinsic.Assert</a> is called in Q#.
        /// </summary>
        /// <remarks>
        /// The names and the order of the parameters is similar to the corresponding Q# operation./
        /// </remarks>
        /// <param name="bases">A multi-qubit Pauli operator, for which the measurement outcome is asserted.</param>
        /// <param name="qubits">A register on which to make the assertion.</param>
        /// <param name="probabilityOfZero">The probability with which result Zero is expected.</param>
        /// <param name="msg">A message to be reported if the assertion fails.</param>
        /// <param name="tol">The precision with which the probability of Zero outcome is specified.</param>
        void AssertProb(IQArray<Pauli> bases, IQArray<Qubit> qubits, double probabilityOfZero, string msg, double tol);

        /// <summary>
        /// Called when <a href="https://docs.microsoft.com/qsharp/api/qsharp/microsoft.quantum.intrinsic.x">Microsoft.Quantum.Intrinsic.X</a> is called in Q#.
        /// In Q# the operation applies X gate to <paramref name="qubit"/>. The gate is given by matrix X=((0,1),(1,0)).
        /// </summary>
        /// <remarks>
        /// When adjoint of X is called in Q#, <see cref="X(Qubit)"/> is called because X is self-adjoint.
        /// The names and the order of the parameters are the same as for the corresponding Q# operation.
        /// </remarks>
        /// <param name="qubit">Qubit to which the gate should be applied.</param>
        void X(Qubit qubit);

        /// <summary>
        /// Called when controlled <a href="https://docs.microsoft.com/qsharp/api/qsharp/microsoft.quantum.intrinsic.x">Microsoft.Quantum.Intrinsic.X</a> is called in Q#.
        /// In Q# the operation applies X gate to <paramref name="qubit"/> controlled on <paramref name="controls"/>. The gate is given by matrix X=((0,1),(1,0)).
        /// </summary>
        /// <remarks>
        /// When adjoint of Controlled X is called in Q#, <see cref="ControlledX(IQArray{Qubit}, Qubit)"/> is called because X is self-adjoint.
        /// The names and the order of the parameters are the same as for the corresponding Q# operation.
        /// </remarks>
        /// <param name="controls">The array of qubits on which the operation is controlled.</param>
        /// <param name="qubit">Qubit to which the gate should be applied.</param>
        void ControlledX(IQArray<Qubit> controls, Qubit qubit);

        /// <summary>
        /// Called when <a href="https://docs.microsoft.com/qsharp/api/qsharp/microsoft.quantum.intrinsic.y">Microsoft.Quantum.Intrinsic.Y</a> is called in Q#.
        /// In Q# the operation applies Y gate to <paramref name="qubit"/>. The gate is given by matrix Y=((0,-𝑖),(𝑖,0)).
        /// </summary>
        /// <remarks>
        /// When adjoint of Y is called in Q#, <see cref="Y(Qubit)"/> is called because Y is self-adjoint.
        /// The names and the order of the parameters are the same as for the corresponding Q# operation.
        /// </remarks>
        /// <param name="qubit">Qubit to which the gate should be applied.</param>
        void Y(Qubit qubit);

        /// <summary>
        /// Called when controlled <a href="https://docs.microsoft.com/qsharp/api/qsharp/microsoft.quantum.intrinsic.y">Microsoft.Quantum.Intrinsic.Y</a> is called in Q#.
        /// In Q# the operation applies X gate to <paramref name="qubit"/> controlled on <paramref name="controls"/>. The gate is given by matrix Y=((0,-𝑖),(𝑖,0)).
        /// </summary>
        /// <remarks>
        /// When adjoint of Controlled Y is called in Q#, <see cref="ControlledY(IQArray{Qubit}, Qubit)"/> is called because Y is self-adjoint.
        /// The names and the order of the parameters are the same as for the corresponding Q# operation.
        /// </remarks>
        /// <param name="controls">The array of qubits on which the operation is controlled.</param>
        /// <param name="qubit">Qubit to which the gate should be applied.</param>
        void ControlledY(IQArray<Qubit> controls, Qubit qubit);

        /// <summary>
        /// Called when <a href="https://docs.microsoft.com/qsharp/api/qsharp/microsoft.quantum.intrinsic.z">Microsoft.Quantum.Intrinsic.Z</a> is called in Q#.
        /// In Q# the operation applies Z gate to <paramref name="qubit"/>. The gate is given by matrix Z=((1,0),(0,-1)).
        /// </summary>
        /// <remarks>
        /// When adjoint of Z is called in Q#, <see cref="Z(Qubit)"/> is called because Z is self-adjoint.
        /// The names and the order of the parameters are the same as for the corresponding Q# operation.
        /// </remarks>
        /// <param name="qubit">Qubit to which the gate should be applied.</param>
        void Z(Qubit qubit);

        /// <summary>
        /// Called when controlled <a href="https://docs.microsoft.com/qsharp/api/qsharp/microsoft.quantum.intrinsic.z">Microsoft.Quantum.Intrinsic.Z</a> is called in Q#.
        /// In Q# the operation applies Z gate to <paramref name="qubit"/> controlled on <paramref name="controls"/>. The gate is given by matrix Z=((1,0),(0,-1)).
        /// </summary>
        /// <remarks>
        /// When adjoint of Controlled Z is called in Q#, <see cref="ControlledZ(IQArray{Qubit}, Qubit)"/> is called because Z is self-adjoint.
        /// The names and the order of the parameters are the same as for the corresponding Q# operation.
        /// </remarks>
        /// <param name="controls">The array of qubits on which the operation is controlled.</param>
        /// <param name="qubit">Qubit to which the gate should be applied.</param>
        void ControlledZ(IQArray<Qubit> controls, Qubit qubit);

        /// <summary>
        /// Called when <a href="https://docs.microsoft.com/qsharp/api/qsharp/microsoft.quantum.intrinsic.r1">Microsoft.Quantum.Intrinsic.R1</a> is called in Q#.
        /// In Q# the operation applies gate given by matrix ((1,0),(0,𝑒𝑥𝑝(𝑖⋅<paramref name="theta"/>))) to <paramref name="qubit"/>.  
        /// </summary>
        /// <remarks>
        /// When adjoint of R1 is called in Q#, <see cref="R1(double, Qubit)"/> is called with <paramref name="theta"/> replaced by -<paramref name="theta"/>.
        /// The names and the order of the parameters are the same as for the corresponding Q# operation.
        /// </remarks>
        /// <param name="theta">Angle about which the qubit is to be rotated.</param>
        /// <param name="qubit">Qubit to which the gate should be applied.</param>
        void R1(double theta, Qubit qubit);

        /// <summary>
        /// Called when controlled <a href="https://docs.microsoft.com/qsharp/api/qsharp/microsoft.quantum.intrinsic.r">Microsoft.Quantum.Intrinsic.R</a> is called in Q#.
        /// In Q# the operation applies gate given by matrix ((1,0),(0,𝑒𝑥𝑝(𝑖⋅<paramref name="theta"/>))) to <paramref name="qubit"/> controlled on <paramref name="controls"/>.  
        /// </summary>
        /// <remarks>
        /// When adjoint of Controlled R1 is called in Q#, <see cref="ControlledR1(IQArray{Qubit}, double, Qubit)"/> is called with <paramref name="theta"/> replaced by -<paramref name="theta"/>.
        /// The names and the order of the parameters are the same as for the corresponding Q# operation.
        /// </remarks>
        /// <param name="controls">The array of qubits on which the operation is controlled.</param>
        /// <param name="theta">Angle about which the qubit is to be rotated.</param>
        /// <param name="qubit">Qubit to which the gate should be applied.</param>
        void ControlledR1(IQArray<Qubit> controls, double theta, Qubit qubit);

        /// <summary>
        /// Called when <a href="https://docs.microsoft.com/qsharp/api/qsharp/microsoft.quantum.intrinsic.r1frac">Microsoft.Quantum.Intrinsic.R1Frac</a> is called in Q#.
        /// In Q# the operation applies gate given by matrix ((1,0),(0,𝑒𝑥𝑝(𝑖⋅π⋅<paramref name="numerator"/>/2^<paramref name="power"/>))) to <paramref name="qubit"/>.  
        /// </summary>
        /// <remarks>
        /// When adjoint of R1Frac is called in Q#, <see cref="R1Frac(long, long, Qubit)"/> is called with <paramref name="numerator"/> replaced by -<paramref name="numerator"/>.
        /// The names and the order of the parameters are the same as for the corresponding Q# operation.
        /// </remarks>
        /// <param name="numerator">Numerator in the dyadic fraction representation of the angle by which the qubit is to be rotated.</param>
        /// <param name="power">Power of two specifying the denominator of the angle by which the qubit is to be rotated.</param>
        /// <param name="qubit">Qubit to which the gate should be applied.</param>
        void R1Frac(long numerator, long power, Qubit qubit);

        /// <summary>
        /// Called when a controlled <a href="https://docs.microsoft.com/qsharp/api/qsharp/microsoft.quantum.intrinsic.r1frac">Microsoft.Quantum.Intrinsic.R1Frac</a> is called in Q#.
        /// In Q# the operation applies gate given by matrix ((1,0),(0,𝑒𝑥𝑝(𝑖⋅π⋅<paramref name="numerator"/>/2^<paramref name="power"/>))) to <paramref name="qubit"/> controlled on <paramref name="controls"/>.
        /// </summary>
        /// <remarks>
        /// When adjoint of Controlled RFrac is called in Q#, <see cref="ControlledR1Frac(IQArray{Qubit}, long, long, Qubit)"/> is called with <paramref name="numerator"/> replaced by -<paramref name="numerator"/>.
        /// The names and the order of the parameters are the same as for the corresponding Q# operation.
        /// </remarks>
        /// <param name="controls">The array of qubits on which the operation is controlled.</param>
        /// <param name="numerator">Numerator in the dyadic fraction representation of the angle by which the qubit is to be rotated.</param>
        /// <param name="power">Power of two specifying the denominator of the angle by which the qubit is to be rotated.</param>
        /// <param name="qubit">Qubit to which the gate should be applied.</param>
        void ControlledR1Frac(IQArray<Qubit> controls, long numerator, long power, Qubit qubit);

        /// <summary>
        /// Called when <a href="https://docs.microsoft.com/qsharp/api/qsharp/microsoft.quantum.intrinsic.swap">Microsoft.Quantum.Intrinsic.SWAP</a> is called in Q#.
        /// In Q# the operation applies gate given by rule |ψ⟩⊗|ϕ⟩ ↦ |ϕ⟩⊗|ψ⟩ where |ϕ⟩,|ψ⟩ arbitrary one qubit states.
        /// </summary>
        /// <remarks>
        /// When adjoint of SWAP is called in Q#, <see cref="SWAP(Qubit, Qubit)"/> is called because SWAP is self-adjoint.
        /// The names and the order of the parameters are the same as for the corresponding Q# operation.
        /// </remarks>
        /// <param name="qubit1">First qubit to be swapped.</param>
        /// <param name="qubit2">Second qubit to be swapped.</param>
        void SWAP(Qubit qubit1, Qubit qubit2);

        /// <summary>
        /// Called when controlled <a href="https://docs.microsoft.com/qsharp/api/qsharp/microsoft.quantum.intrinsic.swap">Microsoft.Quantum.Intrinsic.SWAP</a> is called in Q#.
        /// In Q# the operation applies gate given by rule |ψ⟩⊗|ϕ⟩ ↦ |ϕ⟩⊗|ψ⟩ where |ϕ⟩,|ψ⟩ arbitrary one qubit states controlled on <paramref name="controls"/>.
        /// </summary>
        /// <remarks>
        /// When adjoint of Controlled SWAP is called in Q#, <see cref="ControlledSWAP(IQArray{Qubit}, Qubit, Qubit)"/> is called because SWAP is self-adjoint.
        /// The names and the order of the parameters are the same as for the corresponding Q# operation.
        /// </remarks>
        /// <param name="controls">The array of qubits on which the operation is controlled.</param>
        /// <param name="qubit1">First qubit to be swapped.</param>
        /// <param name="qubit2">Second qubit to be swapped.</param>
        void ControlledSWAP(IQArray<Qubit> controls, Qubit qubit1, Qubit qubit2);

        /// <summary>
        /// Called before a call to any Q# operation.
        /// </summary>
        /// <param name="operation">Information about operation being called.</param>
        /// <param name="arguments">Information about the arguments passed to the operation.</param>
        /// <remarks>
        /// To get the fully qualified Q# name of operation being called use <see cref="ICallable.FullName"/>.
        /// For the variant of operation, that is to find if Adjoint, Controlled or Controlled Adjoint being called use <see cref="ICallable.Variant"/>.
        /// To get a sequence of all qubits passed to the operation use <see cref="IApplyData.Qubits"/>.
        /// </remarks>
        void OnOperationStart(ICallable operation, IApplyData arguments);

        /// <summary>
        /// Called in the end of the call to any Q# operation.
        /// </summary>
        /// <param name="operation">Information about operation being called.</param>
        /// <param name="arguments">Information about the arguments passed to the operation.</param>
        /// <remarks>
        /// To get the fully qualified Q# name of operation being called use <see cref="ICallable.FullName"/>.
        /// For the variant of operation, that is to find if Adjoint, Controlled or Controlled Adjoint being called use <see cref="ICallable.Variant"/>.
        /// To get a sequence of all qubits passed to the operation use <see cref="IApplyData.Qubits"/>.
        /// </remarks>
        void OnOperationEnd(ICallable operation, IApplyData arguments);

        /// <summary>
        /// Intended for a limited support of branching upon measurement results on a simulator level.
        /// </summary>
        /// <param name="measurementResult">The result of the measurement upon which branching is to be performed.</param>
        /// <param name="onZero">Corresponds to quantum program that must be executed if <paramref name="measurementResult"/> result is <see cref="ResultValue.Zero"/></param>
        /// <param name="onOne">Corresponds to quantum program that must be executed if <paramref name="measurementResult"/> result is <see cref="ResultValue.One"/></param>
        /// <remarks>
        /// Calling <c>onZero()</c> will result in the execution of quantum program that Q# user intends to execute if <paramref name="measurementResult"/> result is <see cref="ResultValue.Zero"/>.
        /// The program is executed with the same instance of <see cref="ICircuitizer"/> interface.
        /// </remarks>
        void ClassicallyControlled(Result measurementResult, Action onZero, Action onOne);

        /// <summary>
        /// Called when qubits are allocated by Q# <a href="https://docs.microsoft.com/quantum/language/statements#clean-qubits"><c>using</c></a> block. 
        /// </summary>
        /// <param name="qubits">Qubits that are allocated</param>.
        /// <remarks>
        /// Every qubit in simulation framework has a unique identifier <see cref="Qubit.Id"/>.
        /// All newly allocated qubits are in |0⟩ state.
        /// </remarks>
        void OnAllocateQubits(IQArray<Qubit> qubits);

        /// <summary>
        /// Called when qubits are released in Q# in the end of <a href="https://docs.microsoft.com/quantum/language/statements#clean-qubits"><c>using</c></a> block. 
        /// </summary>
        /// <param name="qubits">Qubits that are released</param>.
        /// <remarks>
        /// Every qubit in simulation framework has a unique identifier <see cref="Qubit.Id"/>.
        /// All qubits are expected to be released in |0⟩ state.
        /// </remarks>
        void OnReleaseQubits(IQArray<Qubit> qubits);

        /// <summary>
        /// Called when qubits are borrowed by Q# <a href="https://docs.microsoft.com/quantum/language/statements#dirty-qubits"><c>borrowing</c></a> block. 
        /// </summary>
        /// <param name="qubits">Qubits that are borrowed</param>.
        /// <remarks>
        /// Every qubit in simulation framework has a unique identifier <see cref="Qubit.Id"/>.
        /// Borrowed qubits can be in any state.
        /// </remarks>
        void OnBorrowQubits(IQArray<Qubit> qubits);

        /// <summary>
        /// Called when qubits are returned in the end of Q# <a href="https://docs.microsoft.com/quantum/language/statements#dirty-qubits"><c>borrowing</c></a> block. 
        /// </summary>
        /// <param name="qubits">Qubits that has been allocated</param>.
        /// <remarks>
        /// Every qubit in simulation framework has a unique identifier <see cref="Qubit.Id"/>.
        /// Borrowed qubits are expected to be returned in the same state as the state they have been borrowed in.
        /// </remarks>
        void OnReturnQubits(IQArray<Qubit> qubits);

        /// <summary>
        /// Called when 
        /// <a href="https://docs.microsoft.com/qsharp/api/qsharp/microsoft.quantum.diagnostics.dumpregister">Microsoft.Quantum.Diagnostics.DumpRegister</a>
        /// or 
        /// <a href="https://docs.microsoft.com/qsharp/api/qsharp/microsoft.quantum.diagnostics.dumpmachine">Microsoft.Quantum.Diagnostics.DumpMachine</a>
        /// are called in Q#.
        /// </summary>
        /// <remarks>
        /// The names and the order of the parameters are similar to corresponding Q# operations. 
        /// </remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="location">Provides information on where to generate the state's DumpMachine was called.</param>
        /// <param name="qubits">The list of qubits to report. If null, </param>
        void OnDump<T>(T location, IQArray<Qubit> qubits = null);

        /// <summary>
        /// Called when <a href="https://docs.microsoft.com/qsharp/api/qsharp/microsoft.quantum.intrinsic.message">Microsoft.Quantum.Intrinsic.Message</a> is called in Q#.
        /// </summary>
        /// <remarks>
        /// The names and the order of the parameters is the same as corresponding Q# operations. 
        /// </remarks>
        /// <param name="msg">The message to be reported.</param>
        void OnMessage(string msg);
    }
}
