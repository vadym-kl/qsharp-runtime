using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Microsoft.Quantum.Simulation.Core;
using Microsoft.Quantum.Simulation.Simulators;

namespace SimulatorSkeleton
{
    public class CircuitizerResult : Result
    {
        public CircuitizerResult(long classicalWireId, IQArray<Qubit> measuredQubits, IQArray<Pauli> measuredPauli)
        {
            ClassicalWireId = classicalWireId;
            MeasuredQubits = measuredQubits;
            MeasuredPauli = measuredPauli;
        }

        public CircuitizerResult(long classicalWireId, Qubit measuredQubit, Pauli measuredPauli = Pauli.PauliZ)
        {
            ClassicalWireId = classicalWireId;
            MeasuredQubits = new QArray<Qubit>(new Qubit[] { measuredQubit });
            MeasuredPauli = new QArray<Pauli>(new Pauli[] { measuredPauli });
        }

        public IQArray<Qubit> MeasuredQubits { get; private set; }
        public IQArray<Pauli> MeasuredPauli { get; private set; }
        public long ClassicalWireId { get; private set; }

        public override ResultValue GetValue()
        {
            throw new Exception("This simulator does not support direct access to measurement outcomes");
        }
    }

    public class ASCIICircuitizer : ICircuitizer
    {
        // TODO: make sure that all gate drawing functions pay attention to classicControls array when drawing

        private readonly List<StringBuilder> lines = new List<StringBuilder>();
        private readonly List<bool> occupancy = new List<bool>();
        private readonly List<bool> released = new List<bool>();
        private readonly List<bool> collapsed = new List<bool>();

        private readonly Stack<Tuple<Int64,bool>> classicControls = new Stack<Tuple<Int64,bool>>();

        private long currentClassicalWireId = -1;
        
        public void Assert(IQArray<Pauli> pauli, IQArray<Qubit> target, Result result, string message){}

        public void AssertProb(IQArray<Pauli> pauli, IQArray<Qubit> target, double probabilityOfZero, string message, double tolerance){}

        public void ClassicallyControlled(Result measurementResult, Action onZero, Action onOne)
        {
            CircuitizerResult result = measurementResult as CircuitizerResult;
            if (result == null)
            {
                throw new Exception("measurementResult must be of type CircuitizerResult and not null");
            }
            long classicalWireId = result.ClassicalWireId;
            classicControls.Push( new Tuple<long,bool>(classicalWireId,false));
            onZero();
            classicControls.Pop();
            classicControls.Push(new Tuple<long, bool>(classicalWireId, true));
            onOne();
            classicControls.Pop();
        }

        public void ControlledExp(IQArray<Qubit> controls, IQArray<Pauli> pauli, double angle, IQArray<Qubit> target)
        {
            for(int i = 0; i < target.Length; i++)
            {
                AddGate("e" + GetPauliAxis(pauli[i]), controls[i].Id, target[i].Id);
            }
        }

        public void ControlledExpFrac(IQArray<Qubit> controls, IQArray<Pauli> pauli, long numerator, long denominator, IQArray<Qubit> target)
        {
            for(int i = 0; i < target.Length; i++)
            {
                AddGate("e" + GetPauliAxis(pauli[i]), controls[i].Id, target[i].Id);
            }
        }

        public void ControlledH(IQArray<Qubit> controls, Qubit target)
        {
            foreach(var control in controls)
            {
                AddGate("H", control.Id, target.Id);
            }
        }

        public void ControlledR(IQArray<Qubit> controls, Pauli axis, double angle, Qubit target)
        {
            foreach(var control in controls)
            {
                AddGate("R"+GetPauliAxis(axis), control.Id, target.Id);
            }
        }

        public void ControlledR1(IQArray<Qubit> controls, double angle, Qubit target)
        {
            foreach(var control in controls)
            {
                AddGate("Rz", control.Id, target.Id);
            }
        }

        public void ControlledR1Frac(IQArray<Qubit> controls, long numerator, long denominator, Qubit target)
        {
            foreach(var control in controls)
            {
                AddGate("Rz", control.Id, target.Id);
            }
        }

        public void ControlledRFrac(IQArray<Qubit> controls, Pauli axis, long numerator, long denominator, Qubit target)
        {
            foreach(var control in controls)
            {
                AddGate("R"+GetPauliAxis(axis), control.Id, target.Id);
            }
        }

        public void ControlledS(IQArray<Qubit> controls, Qubit target)
        {
            foreach(var control in controls)
            {
                AddGate("S", control.Id, target.Id);
            }
        }

        public void ControlledSInv(IQArray<Qubit> controls, Qubit target)
        {
            foreach(var control in controls)
            {
                AddGate("Ƨ", control.Id, target.Id);
            }
        }

        public void ControlledSWAP(IQArray<Qubit> controls, Qubit q1, Qubit q2)
        {
            foreach(var control in controls)
            {
                AddSwap(control.Id, q1.Id, q2.Id);
            }
        }

        public void ControlledT(IQArray<Qubit> controls, Qubit target)
        {
            foreach(var control in controls)
            {
                AddGate("T", control.Id, target.Id);
            }
        }

        public void ControlledTInv(IQArray<Qubit> controls, Qubit target)
        {
            foreach(var control in controls)
            {
                AddGate("┴", control.Id, target.Id);
            }
        }

        public void ControlledX(IQArray<Qubit> controls, Qubit target)
        {
            foreach(var control in controls)
            {
                AddGate("X", control.Id, target.Id);
            }
        }

        public void ControlledY(IQArray<Qubit> controls, Qubit target)
        {
            foreach(var control in controls)
            {
                AddGate("Y", control.Id, target.Id);
            }
        }

        public void ControlledZ(IQArray<Qubit> controls, Qubit target)
        {
            foreach(var control in controls)
            {
                AddGate("Z", control.Id, target.Id);
            }
        }

        public void Exp(IQArray<Pauli> pauli, double angle, IQArray<Qubit> target)
        {
            AddConnectedGate("e", pauli, target);
        }

        public void ExpFrac(IQArray<Pauli> pauli, long numerator, long denominator, IQArray<Qubit> target)
        {
            AddConnectedGate("e", pauli, target);
        }

        public void H(Qubit target)
        {
            AddGate("H", target.Id);
        }

        public Result M(Qubit target)
        {
            currentClassicalWireId++;
            // TODO: draw mesurement result being written into classical wire
            AddGate("Mz", target.Id);
            // collapsed[target.Id] = true;
            return new CircuitizerResult(currentClassicalWireId, target);
        }

        public Result Measure(IQArray<Pauli> pauli, IQArray<Qubit> target)
        {
            currentClassicalWireId++;
            AddConnectedGate("M", pauli, target);
            // TODO: draw mesurement result being written into classical wire with id = currentClassicalWireId
            //foreach (var qubit in target)
            //{
            //    collapsed[qubit.Id] = true;
            //}
            return new CircuitizerResult(currentClassicalWireId, target, pauli);
        }

        public void OnAllocateQubits(IQArray<Qubit> qubits)
        {
            foreach(var qubit in qubits)
            {
                if(qubit.Id >= lines.Count/3)
                {
                    // add spaces upto lines[0].Length - 7
                    occupancy.Add(false);
                    released.Add(false);
                    collapsed.Add(false);
                    lines.Add(new StringBuilder(new string(' ', lines.Count > 0 ? Math.Max(lines[0].Length - 7, 0): 0) + "     "));
                    lines.Add(new StringBuilder(new string(' ', lines.Count > 0 ? Math.Max(lines[0].Length - 7, 0): 0) + "|0>──"));
                    lines.Add(new StringBuilder(new string(' ', lines.Count > 0 ? Math.Max(lines[0].Length - 7, 0): 0) + "     "));
                }
                else if(released[qubit.Id])
                {
                    // reinitialize
                    released[qubit.Id] = false;
                    occupancy[qubit.Id] = false;
                    collapsed[qubit.Id] = false;
                    lines[(3 * qubit.Id)].Append("     ");
                    lines[(3 * qubit.Id) + 1].Append("|0>──");
                    lines[(3 * qubit.Id) + 2].Append("     ");
                }
            }
        }

        public void OnBorrowQubits(IQArray<Qubit> qubits){}

        public void OnOperationEnd(ICallable operation, IApplyData arguments){}

        public void OnOperationStart(ICallable operation, IApplyData arguments){}

        public void OnReleaseQubits(IQArray<Qubit> qubits)
        {
            foreach(var qubit in qubits){
                lines[3 * qubit.Id].Append("     ");
                lines[3 * qubit.Id + 1].Append("──<0|");
                lines[3 * qubit.Id + 2].Append("     ");
                released[qubit.Id] =  true;
            }
        }

        public void OnReturnQubits(IQArray<Qubit> qubits){}

        public void R(Pauli axis, double angle, Qubit target)
        {
            AddGate("R"+GetPauliAxis(axis), target.Id);
        }

        public void R1(double angle, Qubit target)
        {
            AddGate("R1", target.Id);
        }

        public void R1Frac(long numerator, long denominator, Qubit target)
        {
            AddGate("R1", target.Id);
        }

        public void RFrac(Pauli axis, long numerator, long denominator, Qubit target)
        {
            AddGate("R"+GetPauliAxis(axis), target.Id);
        }

        public void S(Qubit target)
        {
            AddGate("S", target.Id);
        }

        public void SInv(Qubit target)
        {
            AddGate("Ƨ", target.Id);
        }

        public void SWAP(Qubit q1, Qubit q2)
        {
            AddSwap(q1.Id, q2.Id);
        }

        public void T(Qubit target)
        {
            AddGate("T", target.Id);
        }

        public void TInv(Qubit target)
        {
            AddGate("┴", target.Id);
        }

        public void X(Qubit target)
        {
            AddGate("X", target.Id);
        }

        public void Y(Qubit target)
        {
            AddGate("Y", target.Id);
        }

        public void Z(Qubit target)
        {
            AddGate("Z", target.Id);
        }

        public override string ToString(){
            StringBuilder sb = new StringBuilder();
            foreach(var line in lines)
            {
                sb.Append(line);
                sb.AppendLine();
            }
            return sb.ToString();
        }

        private void AddGate(string gateName, int targetId){
            if (occupancy[targetId]) {
			    NewColumn();
		    }
            occupancy[targetId] = true;

            lines[(3 * targetId)].Append("┌─────┐");
            lines[(3 * targetId) + 1].Append("┤ " + centeredString(gateName, 3) + " ├");
            lines[(3 * targetId) + 2].Append("└─────┘");       
        }

        private void NewColumn(){
            for(int i = 0;i < occupancy.Count; i++)
            {
                if(released[i])
                {
                    lines[(3 * i)].Append("       ");
				    lines[(3 * i) + 1].Append("       ");
				    lines[(3 * i) + 2].Append("       ");
                }
                else if(!occupancy[i])
                {
                    lines[(3 * i)].Append("       ");
				    lines[(3 * i) + 1].Append(collapsed[i] ? "═══════" : "───────");
				    lines[(3 * i) + 2].Append("       ");
                }
                
                occupancy[i] = false;
            }
        }

        private bool isLastColumnEmpty()
        {
            foreach (var element in occupancy) {
                if (element) {
                    return false;
                }
		    }
		    return true;
        }
        
        private void AddGate(string gateName, int controlId, int targetId)
        {
            if (!isLastColumnEmpty()) {
                NewColumn();
            }
            occupancy[controlId] = true;
            occupancy[targetId] = true;

            lines[(3 * controlId)].Append(controlId < targetId ? "       " : "   │   ");
            lines[(3 * controlId) + 1].Append("───@───");
            lines[(3 * controlId) + 2].Append(controlId < targetId ? "   │   " : "       ");

            lines[(3 * targetId)].Append(controlId < targetId ? "┌──┴──┐" : "┌─────┐");
            lines[(3 * targetId) + 1].Append("┤ "+ centeredString(gateName, 3) + " ├");
            lines[(3 * targetId) + 2].Append(controlId < targetId ? "└─────┘" : "└──┬──┘");

            var min = Math.Min(controlId, targetId);
            var max = Math.Max(controlId, targetId);
            DrawVerticalLine(min, max);
            NewColumn();
        }

        private void AddConnectedGate(string gateName, IQArray<Pauli> Pauli, IQArray<Qubit> target)
        {
            if (!isLastColumnEmpty()) {
                NewColumn();
            }

            var min = target.Min(q => q.Id);
            var max = target.Max(q => q.Id);

            for(int i = 0; i< target.Length; i++)
            {
                var qubit = target[i];
                occupancy[qubit.Id] = true;
                lines[(3 * qubit.Id)].Append(qubit.Id > min ? "┌──┴──┐" : "┌─────┐");
                lines[(3 * qubit.Id) + 1].Append("┤ "+ centeredString(gateName + GetPauliAxis(Pauli[i]), 3) + " ├");
                lines[(3 * qubit.Id) + 2].Append(qubit.Id < max ? "└──┬──┘" : "└─────┘");
            }
            
            var ordered = target.Select(q => q.Id).OrderBy(id => id).ToArray();
            for (int i = 0; i< ordered.Length -1; i++)
            {
                DrawVerticalLine(ordered[i], ordered[i+1]);
            }          
            NewColumn();
        }

        private void AddSwap(int q0Id, int q1Id)
        {
            if (!isLastColumnEmpty()) {
			    NewColumn();
		    }
		    occupancy[q0Id] = true;
		    occupancy[q1Id] = true;

		    lines[(3 * q0Id)].Append(q0Id < q1Id ? "       " : "   │   ");
		    lines[(3 * q0Id) + 1].Append("───╳───");
		    lines[(3 * q0Id) + 2].Append(q0Id < q1Id ? "   │   " : "       ");

		    lines[(3 * q1Id)].Append(q0Id < q1Id ? "   │   " : "       ");
		    lines[(3 * q1Id) + 1].Append("───╳───");
		    lines[(3 * q1Id) + 2].Append(q0Id < q1Id ? "       " : "   │   ");

		    var min = Math.Min(q0Id, q1Id);
		    var max = Math.Max(q0Id, q1Id);
            DrawVerticalLine(min, max);
		    NewColumn();
        }

        private void AddSwap(int controlId, int q0Id, int q1Id)
        {
            if (!isLastColumnEmpty()) {
			    NewColumn();
		    }
		    occupancy[q0Id] = true;
		    occupancy[q1Id] = true;
            occupancy[controlId] = true;

            var minQ = Math.Min(q0Id, q1Id);
		    var maxQ = Math.Max(q0Id, q1Id);

            lines[(3 * controlId)].Append(controlId < minQ ? "       " : "   │   ");
            lines[(3 * controlId) + 1].Append("───@───");
            lines[(3 * controlId) + 2].Append(controlId < maxQ ? "   │   " : "       ");

            // swap lines
		    lines[(3 * q0Id)].Append(q0Id < q1Id ? "       " : "   │   ");
		    lines[(3 * q0Id) + 1].Append("───╳───");
		    lines[(3 * q0Id) + 2].Append(q0Id < q1Id ? "   │   " : "       ");

		    lines[(3 * q1Id)].Append(q0Id < q1Id ? "   │   " : "       ");
		    lines[(3 * q1Id) + 1].Append("───╳───");
		    lines[(3 * q1Id) + 2].Append(q0Id < q1Id ? "       " : "   │   ");

            // DrawVerticalLine(minQ, maxQ);

            // control lines
            if(controlId > maxQ)
            {
                DrawVerticalLine(maxQ, controlId);
                lines[(3 * maxQ) + 2].Replace(' ', '│', lines[(3 * maxQ) + 2].Length - 4, 1);
            }
            else if(controlId < minQ)
            {
                DrawVerticalLine(controlId, minQ);
                lines[(3 * minQ)].Replace(' ', '│', lines[(3 * minQ)].Length - 4, 1);
            }
            
		    NewColumn();
        }

        private void DrawVerticalLine(int min, int max)
        {
            for (int i = min + 1; i < max; ++i) {
                occupancy[i] = true;
                lines[(3 * i)].Append("   │   ");
                lines[(3 * i) + 1].Append("───┼───");
                lines[(3 * i) + 2].Append("   │   ");
            }
        }

        private char GetPauliAxis(Pauli axis)
        {
            switch(axis)
            {
                case Pauli.PauliX:
                    return 'x';
                case Pauli.PauliY:
                    return 'y';
                case Pauli.PauliZ:
                    return 'z';
                default:
                    return '?';              
            }
        }

        private string centeredString(string s, int width)
        {
            if (s.Length >= width)
            {
                return s;
            }

            int leftPadding = (width - s.Length) / 2;
            int rightPadding = width - s.Length - leftPadding;

            return new string(' ', leftPadding) + s + new string(' ', rightPadding);
        }
    }
}
