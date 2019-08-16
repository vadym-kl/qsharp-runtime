using Microsoft.Quantum.Simulation.Core;

namespace Microsoft.Quantum.Simulation.Circuitizer
{
    public partial class CircuitizerSimulator
    {
        public class CircuitizerSimAllocate : Intrinsic.Allocate
        {
            private readonly CircuitizerSimulator sim;
            public CircuitizerSimAllocate(CircuitizerSimulator m) : base(m){
                sim = m;
            }

            public override Qubit Apply()
            {
                IQArray<Qubit> qubits = sim.QubitManager.Allocate(1);
                sim.Circuitizer.OnAllocateQubits(qubits);
                return qubits[0];
            }

            public override IQArray<Qubit> Apply( long count )
            {
                IQArray<Qubit> qubits = sim.QubitManager.Allocate(count);
                sim.Circuitizer.OnAllocateQubits(qubits);
                return qubits;
            }
        }
    }
}