using Microsoft.Quantum.Intrinsic;
using Microsoft.Quantum.Simulation.Common;
using Microsoft.Quantum.Simulation.Core;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.Quantum.Simulation.Circuitizer
{
    /// <summary>
    /// Simulator that redirects all the calls to a class implementing <see cref="ICircuitizer"/> interface.
    /// </summary>
    public partial class CircuitizerSimulator : SimulatorBase
    {
        private const int PreallocatedQubitCount = 1000;
        /// <summary>
        /// Random number generator used for <a href="https://docs.microsoft.com/qsharp/api/qsharp/microsoft.quantum.intrinsic.random">Microsoft.Quantum.Intrinsic.Random</a> 
        /// </summary>
        public readonly System.Random random;

        public override string Name => "CircuitizerSimulator";

        /// <summary>
        /// An instance of a class implementing <see cref="ICircuitizer"/> interface that this simulator wraps.
        /// </summary>
        public ICircuitizer Circuitizer
        {
            get;
            private set;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="circuitizer">An instance of a class implementing <see cref="ICircuitizer"/> interface to be wrapped. If the parameter is null <see cref="EmptyCircuitizer"/> is used.</param>
        /// <param name="disableBorrowing">If true, all <c>borrowing</c> blocks in Q# behave like <c>using</c> blocks.</param>
        /// <param name="randomSeed">A seed to be used by Q# <a href="https://docs.microsoft.com/qsharp/api/qsharp/microsoft.quantum.intrinsic.random">Microsoft.Quantum.Intrinsic.Random</a> operation.</param>
        public CircuitizerSimulator(ICircuitizer circuitizer = null, bool disableBorrowing = false, int? randomSeed = null)
            : base(new QubitManagerTrackingScope(PreallocatedQubitCount, true, disableBorrowing))
        {
            random = new System.Random(randomSeed == null ? DateTime.Now.Millisecond : randomSeed.Value);
            Circuitizer = circuitizer ?? new EmptyCircuitizer();
            OnOperationStart += Circuitizer.OnOperationStart;
            OnOperationEnd += Circuitizer.OnOperationEnd;
            OnLog += Circuitizer.OnMessage;
        }
    }
}
