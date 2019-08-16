using Microsoft.Quantum.Simulation.Core;

namespace Microsoft.Quantum.Simulation.Circuitizer
{ 
    public partial class CircuitizerSimulator
    {
        public class CircuitizerSimBorrow : Intrinsic.Borrow
        {
            private readonly CircuitizerSimulator sim;
            public CircuitizerSimBorrow(CircuitizerSimulator m) : base(m){
                sim = m;
            }

            public override Qubit Apply()
            {
                IQArray<Qubit> qubits = sim.QubitManager.Borrow(1);
                sim.Circuitizer.OnBorrowQubits(qubits);
                return qubits[0];
            }

            public override IQArray<Qubit> Apply( long count )
            {
                IQArray<Qubit> qubits = sim.QubitManager.Borrow(1);
                sim.Circuitizer.OnBorrowQubits(qubits);
                return qubits;
            }
        }
    }
}