namespace Microsoft.Quantum.Simulation.Circuitizer
{
    using Microsoft.Quantum.Simulation.Core;

    public partial class CircuitizerSimulator
    {
        public class CircuitizerSimReturn : Intrinsic.Return
        {
            private readonly CircuitizerSimulator sim;
            public CircuitizerSimReturn(CircuitizerSimulator m) : base(m){
                sim = m;
            }

            public override void Apply(Qubit q)
            {
                sim.Circuitizer.OnReturnQubits(new QArray<Qubit>(q));
                sim.QubitManager.Return(q);
            }

            public override void Apply(IQArray<Qubit> qubits)
            {
                sim.Circuitizer.OnReturnQubits(qubits);
                sim.QubitManager.Return(qubits);
            }
        }
    }
}