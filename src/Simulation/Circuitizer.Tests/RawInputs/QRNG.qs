namespace Circuitizer.Tests.Extensions {
    open Microsoft.Quantum.Intrinsic;

    operation QRNG() : Result {
        using(q = Qubit()) {
            H(q);

            let r = M(q);

            if (r == One) {
                X(q);
            }

            return r;
        }
    }
}