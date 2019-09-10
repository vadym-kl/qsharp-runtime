namespace Circuitizer.Tests {
    open Microsoft.Quantum.Intrinsic;

    operation Wrapper(q: Qubit, b: Bool) : Unit 
    is Adj + Ctl {
        if (b) {X(q);}
    }

    operation ToyOperation(q: Qubit, ctrl: Qubit) : Result {
        H(q);
        Adjoint Rx(1.2, q);
        Controlled Wrapper([ctrl], (q, false));
        (Wrapper(q,_))(false);

        let r = M(q);

        if (r == Zero) {
            (Wrapper(q,_))(true);
            CNOT(ctrl, q);
            Adjoint Rx(1.2, q);
        }
        
        if (One == M(q)) {
            Y(q);
            Controlled Y([ctrl], q);
            R(PauliY, 1.2, q);
        }

        return r;
    }
}