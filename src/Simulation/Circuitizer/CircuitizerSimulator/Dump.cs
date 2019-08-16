using System;
using System.IO;
using System.Linq;
using Microsoft.Quantum.Simulation.Core;

namespace Microsoft.Quantum.Simulation.Circuitizer
{
    public partial class CircuitizerSimulator
    {
        /// <summary>
        /// Dumps the wave function for the given qubits into the given target. 
        /// If the target is QVoid or an empty string, it dumps it to the console
        /// using the `Message` function, otherwise it dumps the content into a file
        /// with the given name.
        /// If the given qubits is null, it dumps the entire wave function, otherwise
        /// it attempts to create the wave function or the resulting subsystem; if it fails
        /// because the qubits are entangled with some external qubit, it just generates a message.
        /// </summary>
        protected virtual QVoid Dump<T>(T target, IQArray<Qubit> qubits = null)
        {
            Circuitizer.OnDump<T>(target, qubits);
            return QVoid.Instance;
        }

        public class CircuitizerSimDumpMachine<T> : Quantum.Diagnostics.DumpMachine<T>
        {
            private CircuitizerSimulator Simulator { get; }

            public CircuitizerSimDumpMachine(CircuitizerSimulator m) : base(m)
            {
                this.Simulator = m;
            }

            public override Func<T, QVoid> Body => (location) =>
            {
                if (location == null) { throw new ArgumentNullException(nameof(location)); }

                return Simulator.Dump(location);
            };
        }

        public class CircuitizerSimDumpRegister<T> : Quantum.Diagnostics.DumpRegister<T>
        {
            private CircuitizerSimulator Simulator { get; }


            public CircuitizerSimDumpRegister(CircuitizerSimulator m) : base(m)
            {
                this.Simulator = m;
            }

            public override Func<(T, IQArray<Qubit>), QVoid> Body => (__in) =>
            {
                var (location, qubits) = __in;

                if (location == null) { throw new ArgumentNullException(nameof(location)); }
                return Simulator.Dump(location, qubits);
            };
        }
    }
}
