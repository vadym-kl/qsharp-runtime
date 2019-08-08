using Microsoft.Quantum.Simulation.Core;
using System;

namespace Microsoft.Quantum.Simulation.Circuitizer
{
    public partial class CircuitizerSimulator
    {
        public static long SampleDistribution(IQArray<double> unnormalizedDistribution, double uniformZeroOneSample)
        {
            double total = 0.0;
            foreach (double prob in unnormalizedDistribution)
            {
                if (prob < 0)
                {
                    throw new ExecutionFailException("Random expects array of non-negative doubles.");
                }
                total += prob;
            }

            if (total == 0)
            {
                throw new ExecutionFailException("Random expects array of non-negative doubles with positive sum.");
            }

            double sample = uniformZeroOneSample * total;
            double sum = unnormalizedDistribution[0];
            for (int i = 0; i < unnormalizedDistribution.Length - 1; ++i)
            {
                if (sum >= sample)
                {
                    return i;
                }
                sum += unnormalizedDistribution[i];
            }
            return unnormalizedDistribution.Length;
        }

        public class CircuitizerSimrandom : Quantum.Intrinsic.Random
        {
            private CircuitizerSimulator Simulator { get; }
            public CircuitizerSimrandom(CircuitizerSimulator m) : base(m)
            {
                Simulator = m;
            }

            public override Func<IQArray<double>, Int64> Body => (p) =>
            {
                return SampleDistribution(p, Simulator.random.NextDouble());
            };            
        }
    }
}
