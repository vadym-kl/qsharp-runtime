using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Quantum.Simulation.Core;
using System.Linq;

namespace Microsoft.Quantum.Simulation.Circuitizer
{
    /// <summary>
    /// Stores the ID of the QPic classical wire where result was written.
    /// </summary>
    class QPicResult : Result, IComparable
    {
        private static int _currentClassicalWireId = 0;

        public int ClassicalWireId { private set; get; }

        public QPicResult()
        {
            ClassicalWireId = _currentClassicalWireId++;
        }

        public override ResultValue GetValue()
        {
            // Not valid to call this.
            return  ResultValue.Zero;
        }

        public int CompareTo(object obj)
        {
            if (!(obj is QPicResult otherResult))
            {
                return 0;
            }
            return otherResult.ClassicalWireId - ClassicalWireId;
        }
    }

    class QPicCircuitizer : ICircuitizer
    {
        private StringBuilder _programText;

        private HashSet<int> _declaredQubitWires;

        private const int WirePadding = 10;

        private Stack<(int Comparand, int TargetId)> _classicalControlContexts;

        private HashSet<int> _terminatedWires;

        private HashSet<int> _lastUsedAsControl;

        private SortedSet<QPicResult> _results;

        public QPicCircuitizer()
        {
            var settings = $"WIREPAD {WirePadding}\n";
            _programText = new StringBuilder(settings);
            _declaredQubitWires = new HashSet<int>();
            _classicalControlContexts = new Stack<(int, int)>();
            _terminatedWires = new HashSet<int>();
            _lastUsedAsControl = new HashSet<int>();
            _results = new SortedSet<QPicResult>();
        }

        public void Assert(IQArray<Pauli> pauli, IQArray<Qubit> target, Result result, string message)
        {
            /* No-Op. */
        }

        public void AssertProb(IQArray<Pauli> pauli, IQArray<Qubit> target, double probabilityOfZero, string message, double tolerance)
        {
            /* No-Op. */
        }

        public void ClassicallyControlled(Result result, Action onZero, Action onOne)
        {
            if(!(result is QPicResult qPicResult))
            {
                throw new ArgumentException($"'{nameof(result)}' must be of type 'QPicResult'");
            }

            _classicalControlContexts.Push((0, qPicResult.ClassicalWireId));
            onZero();
            _classicalControlContexts.Pop();
            _classicalControlContexts.Push((1, qPicResult.ClassicalWireId));
            onOne();
            _classicalControlContexts.Pop();
        }

        public void OnOperationEnd(ICallable operation, IApplyData arguments)
        {
            /* No-Op. */
        }

        public void Reset(Qubit qubit)
        {
            
            WriteOperation("Reset", qubit);
        }

        public void OnOperationStart(ICallable operation, IApplyData arguments)
        {
            /* No-Op. */
        }

        public void ControlledExp(IQArray<Qubit> controls, IQArray<Pauli> pauli, double angle, IQArray<Qubit> target)
        {
            ExpWithAngleString(controls, pauli, angle.ToString(), target);
        }

        public void ControlledExpFrac(IQArray<Qubit> controls, IQArray<Pauli> pauli, long numerator, long denominator, IQArray<Qubit> target)
        {
            var angle = GetAngleFromFrac(numerator, denominator);
            ExpWithAngleString(controls, pauli, angle, target);
        }

        public void ControlledH(IQArray<Qubit> controls, Qubit target)
        {
            WriteOperation("H", target, controls);
        }

        public void ControlledR(IQArray<Qubit> controls, Pauli axis, double angle, Qubit target)
        {
            string gateName = $"$R_{{{GetPauliAxis(axis)}}}({angle})$";
            WriteOperation(gateName, target, controls);   
        }

        public void ControlledR1(IQArray<Qubit> controls, double angle, Qubit target)
        {
            ControlledR(controls, Pauli.PauliZ, angle, target);
        }

        public void ControlledR1Frac(IQArray<Qubit> controls, long numerator, long denominator, Qubit target)
        {
            ControlledRFrac(controls, Pauli.PauliZ, numerator, denominator, target);
        }

        public void ControlledRFrac(IQArray<Qubit> controls, Pauli axis, long numerator, long denominator, Qubit target)
        {
            var angle = GetAngleFromFrac(numerator, denominator);
            string gateName = $"$R_{{{GetPauliAxis(axis)}}}({angle})$";
            WriteOperation(gateName, target, controls);   
        }

        public void ControlledS(IQArray<Qubit> controls, Qubit target)
        {
            WriteOperation("S", target, controls);
        }

        public void ControlledSAdj(IQArray<Qubit> controls, Qubit target)
        {
            WriteOperation("$S^{-1}$", target, controls);
        }

        public void ControlledSWAP(IQArray<Qubit> controls, Qubit q1, Qubit q2)
        {
            var statement = $"q{q1.Id} q{q2.Id} SWAP ";
            if (controls != null)
            {
                foreach (var q in controls)
                {
                    statement += $"q{q.Id} ";
                }
            }
            statement += GetClassicalControlFragment();
            AddStatement(statement);
        }

        public void ControlledT(IQArray<Qubit> controls, Qubit target)
        {
            WriteOperation("T", target, controls);
        }

        public void ControlledTAdj(IQArray<Qubit> controls, Qubit target)
        {
            WriteOperation("$T^{-1}$", target, controls);
        }

        public void ControlledX(IQArray<Qubit> controls, Qubit target)
        {
            UpdateLastUsedAsControls(target, controls);
            var statement = GetClassicalControlFragment();
            if (controls != null)
            {
                foreach (var control in controls)
                {
                    statement += $"q{control.Id} ";
                }
            }
            statement += $"+q{target.Id} ";
            AddStatement(statement);
        }

        public void ControlledY(IQArray<Qubit> controls, Qubit target)
        {
            WriteOperation("Y", target, controls);
        }

        public void ControlledZ(IQArray<Qubit> controls, Qubit target)
        {
            WriteOperation("Z", target, controls);
        }

        public void Exp(IQArray<Pauli> pauli, double angle, IQArray<Qubit> target)
        {
            ControlledExp(null, pauli, angle, target);
        }

        public void ExpFrac(IQArray<Pauli> pauli, long numerator, long denominator, IQArray<Qubit> target)
        {
            ControlledExpFrac(null, pauli, numerator, denominator, target);
        }

        public void H(Qubit target)
        {
            ControlledH(null, target);
        }

        public Result M(Qubit target)
        {
            return Measure(new QArray<Pauli>(new[] { Pauli.PauliZ }), new QArray<Qubit>( new[] { target }));
        }

        public Result Measure(IQArray<Pauli> pauli, IQArray<Qubit> target)
        {
            UpdateLastUsedAsControls(target, null);
            // TODO: Need to create a classical wire from one of the gates in the joint measurement and store it for future evaluation.

            var statement = "";
            for (var i = 0; i < pauli.Count; ++i)
            {
                var p = GetPauliAxis(pauli[i]);
                // TODO: add D shape drawing code as LaTeX macro into QPic preamble, see example in Teams discussion
                var label = "G:op=\"\\draw[fill=white] (-5.000000, -4.000000) -- (3.000000,-4.000000) arc (-90:90:4.000000pt) -- (-5.000000, 4.000000) -- cycle; \\draw (0.000000, 0.000000) node {{\\scriptsize $" + p + "$}};\":sh=0 ";
                statement += $"q{target[i].Id} {label} ";
            }
            AddStatement(statement);
            var result = new QPicResult();
            _results.Add(result);
            return new QPicResult();
        }

        public void OnAllocateQubits(IQArray<Qubit> qubits)
        {
            foreach (var q in qubits)
            {
                _terminatedWires.Remove(q.Id);
                var qubitLabel = $"q{q.Id}";
                if (!_declaredQubitWires.Contains(q.Id))
                {
                    // No need to actually declare wires.
                    _declaredQubitWires.Add(q.Id);
                }
                else
                {
                    AddStatement($"{qubitLabel} IN 0");
                }
            }
        }

        public void OnBorrowQubits(IQArray<Qubit> qubits)
        {
            /* No-Op. */
        }

        public void OnReleaseQubits(IQArray<Qubit> qubits)
        {
            // TODO if the qubit was most recently used as a control, go to that line and add owire. otherwise, use OUT.
            foreach (var q in qubits)
            {
                if (_lastUsedAsControl.Contains(q.Id))
                {
                    MakeWireInvisible(q.Id);
                }
                else if (!_terminatedWires.Contains(q.Id))
                {
                    AddStatement($"q{q.Id} OUT 0");
                }
            }
        }

        public void OnReturnQubits(IQArray<Qubit> qubits)
        {
            /* No-Op. */
        }

        public void R(Pauli axis, double angle, Qubit target)
        {
           ControlledR(null, axis, angle, target);
        }

        public void R1(double angle, Qubit target)
        {
            ControlledR1(null, angle, target);
        }

        public void R1Frac(long numerator, long denominator, Qubit target)
        {
            ControlledR1Frac(null, numerator, denominator, target);
        }

        public void RFrac(Pauli axis, long numerator, long denominator, Qubit target)
        {
            ControlledRFrac(null, axis, numerator, denominator, target);
        }

        public void S(Qubit target)
        {
            ControlledS(null, target);
        }

        public void SAdj(Qubit target)
        {
            ControlledSAdj(null, target);
        }

        public void SWAP(Qubit q1, Qubit q2)
        {
            ControlledSWAP(null, q1, q2);
        }

        public void T(Qubit target)
        {
            ControlledT(null, target);
        }

        public void TAdj(Qubit target)
        {
            ControlledTAdj(null, target);
        }

        public void X(Qubit target)
        {
            ControlledX(null, target);
        }

        public void Y(Qubit target)
        {
            ControlledY(null, target);
        }

        public void Z(Qubit target)
        {
            ControlledZ(null, target);
        }

        public void WriteToFile(string filePath)
        {
            File.WriteAllText(filePath, _programText.ToString());
        }

        public void OnDump<T>(T location, IQArray<Qubit> qubits = null)
        {
            /* No-Op */
        }

        public void OnMessage(string msg)
        {
            AddStatement($"# {msg}");
        }

                private void AddStatement(string statement)
        {
            Console.WriteLine(statement);
            _programText.AppendLine(statement);
        }

        private void WriteOperation(string operation, Qubit target, IQArray<Qubit> controls = null)
        {
            WriteOperation(operation, new QArray<Qubit>(target), controls);
        }

        private void WriteOperation(string operation, IQArray<Qubit> targets, IQArray<Qubit> controls = null)
        {
            UpdateLastUsedAsControls(targets, controls);
            var statement = "";
            var dimensions = GetGateDimensions(operation);
            foreach (var target in targets)
            {
                statement += $"q{target.Id} ";
            }
            statement += $"G {operation} {dimensions} ";
            if (controls != null)
            {
                foreach (var control in controls)
                {
                    statement += $"q{control.Id} ";
                }
            }
            statement += GetClassicalControlFragment();
            AddStatement(statement);
        }

        private void ExpWithAngleString(IQArray<Qubit> controls, IQArray<Pauli> pauli, string angle, IQArray<Qubit> target)
        {
            UpdateLastUsedAsControls(target, controls);
            var statement = "";
            for (int i = 0; i < pauli.Count; ++i)
            {
                var p = pauli[i];
                var t = target[i];
                var pString = GetPauliAxis(p);
                var label = $"\\textrm{{EXP}}: ${p}$, {{{angle}}}";
                var dimensions = GetGateDimensions(label);
                statement += $"q{t.Id} G ${label}$ {dimensions} ";
            }
            if (controls != null)
            {
                foreach (var control in controls)
                {
                    statement += $"q{control.Id} ";
                }
            }
            statement += GetClassicalControlFragment();
            AddStatement(statement);
        }

        private string GetPauliAxis(Pauli axis) 
        {
            switch (axis) 
            {
                case Pauli.PauliI:
                    return "I";
                case Pauli.PauliX:
                    return "X";
                case Pauli.PauliY:
                    return "Y";
                case Pauli.PauliZ:
                    return "Z";
                default:
                    return "I";
            }
        }

        private string GetAngleFromFrac(long numerator, long denominator)
        {
            return $"\\frac{{{numerator}}}{{2^{{{denominator}}}}} \\pi ";
        }

        private int GetGateWidth(string operation)
        {
            // Remove some likely escapes or invisibles to the right number of characters so they don't count toward width.
            operation = operation.Replace("$", "")
                .Replace("{", "")
                .Replace("{", "")
                .Replace("}", "")
                .Replace("^", "")
                .Replace("_", "")
                .Replace("\\frac","")
                .Replace("\\pi", "+")
                .Replace("\\textrm", "");
            return operation.Length * 7 + 5;
        }
        
        private int GetGateHeight(string operation)
        {
            if (operation.Contains("\\frac"))
            {
            return 15; 
            }
            return 10;
        }

        private string GetClassicalControlFragment()
        {
            var prefix = "";
            if (_classicalControlContexts.Any())
            {
                if (_classicalControlContexts.Peek().Comparand == 0)
                {
                    prefix += "-";
                }
                prefix += $"q{_classicalControlContexts.Peek().TargetId} ";
                _lastUsedAsControl.Add(_classicalControlContexts.Peek().TargetId);
            }
            return prefix;
        }

        private string GetGateDimensions(string operation)
        {
            return $"width={GetGateWidth(operation)} height={GetGateHeight(operation)}";
        }

        private string GetExpLabel(Pauli pauli, string angle)
        {
            var p = GetPauliAxis(pauli);
            return $"\\textrm{{EXP}}: ${p}$, {{{angle}}}";
        }
        
        private void UpdateLastUsedAsControls(Qubit target, IQArray<Qubit> controls)
        {
            var targets = new QArray<Qubit>();
            targets.Append(target);
            UpdateLastUsedAsControls(targets, null);
        }

        private void UpdateLastUsedAsControls(IQArray<Qubit> targets, IQArray<Qubit> controls)
        {
            if (targets != null)
            {
                foreach (var target in targets)
                {
                    _lastUsedAsControl.Remove(target.Id);
                }
            }
            if (controls != null)
            {
                foreach (var control in controls)
                {
                    _lastUsedAsControl.Remove(control.Id);
                }
            }
        }

        private void MakeWireInvisible(int qubitId)
        {
            //TODO this might need some debugging.
            var targetQubitName = $"q{qubitId}";
            var idx = _programText.ToString().LastIndexOf(targetQubitName);
            _programText.Insert(idx + targetQubitName.Length, ":owire");
            _terminatedWires.Add(qubitId);
        }
    }
}
