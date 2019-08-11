namespace CircuitTests {
    open Microsoft.Quantum.Convert;
    open Microsoft.Quantum.Intrinsic;
    open Microsoft.Quantum.ClassicalControl;

    operation Z() : Unit {
        using (q1 = Qubit())
        {
            Z(q1);
        }
    }

    operation RY() : Unit {
        using (q1 = Qubit())
        {
            R(PauliY, 60.0, q1);
        }
    }

    operation ConnectedExp() : Unit {
        using ((q1, q2, q3) = (Qubit(), Qubit(), Qubit()))
        {
            Exp([PauliX, PauliY, PauliX], 45.0, [q1, q3, q2]);
        }
    }

    operation HelloWorldCircuit() : Unit {
        using( (q1,q2, q3) = (Qubit(),Qubit(), Qubit())) 
        {
            H(q1);
            H(q2);
            CNOT(q1,q2);
            CNOT(q2,q1);

            using ( q = Qubit() ) {
                Controlled Z([q1,q2], q);
            }

            SWAP(q1, q2);
            Adjoint S(q1);
            Controlled T([q1], q2);
            R(PauliX, 45.0 ,q1);
            Exp([PauliX, PauliY, PauliX], 45.0, [q1, q3, q2]);
            Controlled SWAP([q2], (q1, q3));
            
            let r = M(q1);
            ApplyIfOne(r, (Adjoint T, q2));
            //if(M(q1)== One)
            //{
            //	Adjoint T(q2);
            //}
            


            using ( q = Qubit() ) {
                Controlled Z([q1,q2], q);
            }


            Adjoint T(q2);
            R(PauliY, 60.0, q2);
            let m2 = Measure([PauliX], [q2]);
        }
    }

    operation Teleport (msg : Qubit, target : Qubit) : Unit {
        using (register = Qubit()) {
            // Create some entanglement that we can use to send our message.
            H(register);
            CNOT(register, target);

            // Encode the message into the entangled pair,
            // and measure the qubits to extract the classical data
            // we need to correctly decode the message into the target qubit:
            CNOT(msg, register);
            H(msg);
            let data1 = M(msg);
            let data2 = M(register);

            // decode the message by applying the corrections on
            // the target qubit accordingly:
            // if (data1 == One) { Z(target); }
            ApplyIfOne(data1, (Z, target));
            //if (data2 == One) { X(target); }
            ApplyIfOne(data2, (X, target));

            // Reset our "register" qubit before releasing it.
            Reset(register);
        }
    }

    operation TeleportCircuit() : Unit {
        using( (q1,q2) = (Qubit(), Qubit()) ) {
            Teleport(q1,q2);
        }
    }

}