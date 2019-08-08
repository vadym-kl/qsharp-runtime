using Microsoft.Quantum.Simulation.Core;

namespace Microsoft.Quantum.Simulation.Circuitizer
{
    public partial class CircuitizerSimulator
    {
        public class CircuitizerSimRelease : Intrinsic.Release
        {
            private readonly CircuitizerSimulator sim;
            public CircuitizerSimRelease(CircuitizerSimulator m) : base(m){
                sim = m;
            }

            public override void Apply(Qubit q)
            {
                sim.Circuitizer.OnReleaseQubits(new QArray<Qubit>(q));
                sim.QubitManager.Release(q);
            }

            public override void Apply(IQArray<Qubit> qubits)
            {
                sim.Circuitizer.OnReleaseQubits(qubits);
                sim.QubitManager.Release(qubits);
            }
        }
    }
}