namespace Circuitizer.Tests.Extensions {
    open Microsoft.Quantum.Convert;
    open Microsoft.Quantum.Intrinsic;
    open Microsoft.Quantum.Simulation.Circuitizer.Extensions;

    operation ZTest() : Unit {
        using (q1 = Qubit())
        {
            Z(q1);
        }
    }

    operation RyTest() : Unit {
        using (q1 = Qubit())
        {
            R(PauliY, 60.0, q1);
        }
    }

    operation ConnectedExpTest() : Unit {
        using ((q1, q2, q3) = (Qubit(), Qubit(), Qubit()))
        {
            Exp([PauliZ, PauliY, PauliX], 45.0, [q1, q2, q3]);
        }
    }

    operation SwapTest() : Unit {
        using ((q1, q2) = (Qubit(), Qubit()))
        {
            SWAP(q1, q2);
        }
    }

    operation ControlledXTest() : Unit {
        using ((q1, q2, q3) = (Qubit(), Qubit(), Qubit()))
        {
            Controlled X([q1, q3], q2);
        }
    }

    operation ControlledSwapTest() : Unit {
        using ((q1, q2, q3, q4) = (Qubit(), Qubit(), Qubit(), Qubit()))
        {
            Controlled SWAP([q1, q3], (q2, q4));
        }
    }

    operation MTest() : Unit {
        using (q1 = Qubit())
        {
            let m = M(q1);
        }
    }

    operation MeasureTest() : Unit {
        using ((q1, q2, q3) = (Qubit(), Qubit(), Qubit()))
        {
            let m = Measure([PauliX, PauliY, PauliZ], [q1, q2, q3]);
        }
    }
    
    operation IntegrateCircuitTest() : Unit {
        using( (q1, q2, q3) = (Qubit(),Qubit(), Qubit())) 
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
            
            using ( q = Qubit() ) {
                Controlled Z([q1,q2], q);
            }

            Adjoint T(q2);
            R(PauliY, 60.0, q2);
            let m2 = Measure([PauliX, PauliY], [q2, q3]);
            ApplyIfZero(m2, (R, (PauliY, 60.0, q1)));
        }
    }

    operation Teleport(msg : Qubit, target : Qubit) : Unit {
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

    operation TeleportCircuitTest() : Unit {
        using( (q1,q2) = (Qubit(), Qubit()) ) {
            Teleport(q1,q2);
        }
    }

}