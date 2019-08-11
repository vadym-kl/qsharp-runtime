using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Microsoft.Quantum.Simulation.Core;
using Microsoft.Quantum.Simulation.Simulators;
using math = System.Math;

namespace Microsoft.Quantum.Simulation.Circuitizer
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

        private readonly Dictionary<int, int> qubitIdToIndexMap = new Dictionary<int, int>();

        private readonly Dictionary<int, int> bitIdToIndexMap = new Dictionary<int, int>();

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
            AddControlledConnectedGate("e", controls, pauli, target);
        }

        public void ControlledExpFrac(IQArray<Qubit> controls, IQArray<Pauli> pauli, long numerator, long denominator, IQArray<Qubit> target)
        {
            AddControlledConnectedGate("e", controls, pauli, target);
        }

        public void ControlledH(IQArray<Qubit> controls, Qubit target)
        {
            AddGate("H", controls, target);
        }

        public void ControlledR(IQArray<Qubit> controls, Pauli axis, double angle, Qubit target)
        {
            AddGate("R"+GetPauliAxis(axis), controls, target);
        }

        public void ControlledR1(IQArray<Qubit> controls, double angle, Qubit target)
        {
            AddGate("Rz", controls, target);
        }

        public void ControlledR1Frac(IQArray<Qubit> controls, long numerator, long denominator, Qubit target)
        {
            AddGate("Rz", controls, target);
        }

        public void ControlledRFrac(IQArray<Qubit> controls, Pauli axis, long numerator, long denominator, Qubit target)
        {
            AddGate("R"+GetPauliAxis(axis), controls, target);
        }

        public void ControlledS(IQArray<Qubit> controls, Qubit target)
        {
            AddGate("S", controls, target);
        }

        public void ControlledSAdj(IQArray<Qubit> controls, Qubit target)
        {
            AddGate("Ƨ", controls, target);
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
            AddGate("T", controls, target);
        }

        public void ControlledTAdj(IQArray<Qubit> controls, Qubit target)
        {
            AddGate("┴", controls, target);
        }

        public void ControlledX(IQArray<Qubit> controls, Qubit target)
        {
            AddGate("X", controls, target);
        }

        public void ControlledY(IQArray<Qubit> controls, Qubit target)
        {
            AddGate("Y", controls, target);
        }

        public void ControlledZ(IQArray<Qubit> controls, Qubit target)
        {
            AddGate("Z", controls, target);
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
            AddMeasureGate(target.Id);
            return new CircuitizerResult(currentClassicalWireId, target);
        }

        public Result Measure(IQArray<Pauli> pauli, IQArray<Qubit> target)
        {
            currentClassicalWireId++;
            AddConnectedGate("M", pauli, target);
            return new CircuitizerResult(currentClassicalWireId, target, pauli);
        }

        public void OnAllocateQubits(IQArray<Qubit> qubits)
        {
            foreach(var qubit in qubits)
            {
                var originalLinesCount = lines.Count;
                if(qubit.Id >= originalLinesCount/3)
                {
                    // map since classical bits may be added in between
                    qubitIdToIndexMap[qubit.Id] = occupancy.Count;
                    occupancy.Add(false);
                    released.Add(false);
                    collapsed.Add(false);
                    lines.Add(new StringBuilder(new string(' ', originalLinesCount > 0 ? math.Max(lines[0].Length - 7, 0): 0) + "       "));
                    lines.Add(new StringBuilder(new string(' ', originalLinesCount > 0 ? math.Max(lines[0].Length - 7, 0): 0) + "|0>────"));
                    lines.Add(new StringBuilder(new string(' ', originalLinesCount > 0 ? math.Max(lines[0].Length - 7, 0): 0) + "       "));
                }
                else if(released[qubit.Id])
                {
                    // reinitialize
                    NewColumn();
                    int qubitIndex = qubitIdToIndexMap[qubit.Id];
                    released[qubitIndex] = false;
                    occupancy[qubitIndex] = false;
                    collapsed[qubitIndex] = false;
                    lines[(3 * qubitIndex) + 1].Replace("       ", "|0>────", lines[3*qubitIndex].Length -7, 7);
                }
            }
        }

        public void OnBorrowQubits(IQArray<Qubit> qubits){}

        public void OnOperationEnd(ICallable operation, IApplyData arguments){}

        public void OnOperationStart(ICallable operation, IApplyData arguments){}

        public void OnReleaseQubits(IQArray<Qubit> qubits)
        {
            foreach(var qubit in qubits){
                lines[3 * qubit.Id].Append("       ");
                lines[3 * qubit.Id + 1].Append("────<0|");
                lines[3 * qubit.Id + 2].Append("       ");
                released[qubit.Id] =  true;
                occupancy[qubit.Id] = true;
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

        public void SAdj(Qubit target)
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

        public void TAdj(Qubit target)
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

        private void AddClassicalBit(int measuredId)
        {
            bitIdToIndexMap[(int)currentClassicalWireId] = occupancy.Count;
            occupancy.Add(true);
            released.Add(false);
            collapsed.Add(true);
            lines.Add(new StringBuilder(new string(' ', lines[3* measuredId].Length - 7) + "   ║   "));
            lines.Add(new StringBuilder(new string(' ', lines[3* measuredId].Length - 7) + "   ╚═══"));
            lines.Add(new StringBuilder(new string(' ', lines[3* measuredId].Length - 7) + "       "));
        }

        private void AddGate(string gateName, int targetId){
            int targetIndex = qubitIdToIndexMap[targetId];
            if (occupancy[targetIndex]) {
			    NewColumn();
		    }
            occupancy[targetIndex] = true;

            lines[(3 * targetIndex)].Append("┌─────┐");
            lines[(3 * targetIndex) + 1].Append("┤ " + centeredString(gateName, 3) + " ├");
            lines[(3 * targetIndex) + 2].Append("└─────┘");

            DrawClassicControls(targetIndex);
            NewColumn();   
        }

        private void AddMeasureGate(int targetId)
        {
            int targetIndex = qubitIdToIndexMap[targetId];
            if(!isLastColumnEmpty())
            {
                NewColumn();
            }

            occupancy[targetIndex] = true;
            lines[(3 * targetIndex)].Append("┌─────┐");
            lines[(3 * targetIndex) + 1].Append("┤ " + centeredString("Mz", 3) + " ├");
            lines[(3 * targetIndex) + 2].Append("└──╥──┘");

            AddClassicalBit(targetIndex);
            DrawVerticalClassicalLine(targetIndex, occupancy.Count);
            NewColumn();
        }

        private void NewColumn(){
            for(int i = 0;i < occupancy.Count; i++)
            {
                if(!occupancy[i])
                {
                    lines[(3 * i)].Append("       ");
				    lines[(3 * i) + 1].Append(released[i] ? "       " : collapsed[i] ? "═══════" : "───────");
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
        
        private void AddGate(string gateName, IQArray<Qubit> controls, Qubit target)
        {
            if (!isLastColumnEmpty()) {
                NewColumn();
            }
            int targetIndex = qubitIdToIndexMap[target.Id];
            occupancy[targetIndex] = true;
            var min = math.Min(controls.Min(q => qubitIdToIndexMap[q.Id]), targetIndex);
            var max = math.Max(controls.Max(q => qubitIdToIndexMap[q.Id]), target.Id);
            int controlIndex = 0;
            foreach(var control in controls)
            {
                controlIndex = qubitIdToIndexMap[control.Id];
                occupancy[controlIndex] = true;
                lines[(3 * controlIndex)].Append(controlIndex == min ? "       " : "   │   ");
                lines[(3 * controlIndex) + 1].Append("───●───");
                lines[(3 * controlIndex) + 2].Append(controlIndex == max ? "       ": "   │   " );
            }

            lines[(3 * targetIndex)].Append(targetIndex > min ? "┌──┴──┐" : "┌─────┐");
            lines[(3 * targetIndex) + 1].Append("┤ "+ centeredString(gateName, 3) + " ├");
            lines[(3 * targetIndex) + 2].Append(targetIndex < max ? "└──┬──┘": "└─────┘");

            DrawVerticalLine(min, max);
            NewColumn();
        }

        private void AddControlledConnectedGate(string gateName, IQArray<Qubit> controls, IQArray<Pauli> Pauli, IQArray<Qubit> target)
        {
            if (!isLastColumnEmpty()) {
                NewColumn();
            }

            var min = math.Min(controls.Min(q => qubitIdToIndexMap[q.Id]), target.Min(q => qubitIdToIndexMap[q.Id]));
            var max = math.Max(controls.Max(q => qubitIdToIndexMap[q.Id]), target.Max(q => qubitIdToIndexMap[q.Id]));

            int controlIndex = 0;
            foreach(var control in controls)
            {
                controlIndex = qubitIdToIndexMap[control.Id];
                occupancy[controlIndex] = true;
                lines[(3 * controlIndex)].Append(controlIndex == min ? "       " : "   │   ");
                lines[(3 * controlIndex) + 1].Append("───●───");
                lines[(3 * controlIndex) + 2].Append(controlIndex == max ? "       ": "   │   " );
            }

            int targetIndex = 0;
            for(int i = 0; i< target.Length; i++)
            {
                targetIndex = qubitIdToIndexMap[target[i].Id];
                occupancy[targetIndex] = true;
                lines[(3 * targetIndex)].Append(targetIndex > min ? "┌──┴──┐" : "┌─────┐");
                lines[(3 * targetIndex) + 1].Append("┤ "+ centeredString(gateName + GetPauliAxis(Pauli[i]), 3) + " ├");
                lines[(3 * targetIndex) + 2].Append(targetIndex < max ? "└──┬──┘" : "└─────┘");
            }
            
            DrawVerticalLine(min, max);
            NewColumn();
        }

        private void AddConnectedGate(string gateName, IQArray<Pauli> Pauli, IQArray<Qubit> target)
        {
            if (!isLastColumnEmpty()) {
                NewColumn();
            }

            var min = target.Min(q => qubitIdToIndexMap[q.Id]);
            var max = target.Max(q => qubitIdToIndexMap[q.Id]);

            int targetIndex = 0;
            for(int i = 0; i< target.Length; i++)
            {
                targetIndex = qubitIdToIndexMap[target[i].Id];
                occupancy[targetIndex] = true;
                lines[(3 * targetIndex)].Append(targetIndex > min ? "┌──┴──┐" : "┌─────┐");
                lines[(3 * targetIndex) + 1].Append("┤ "+ centeredString(gateName + GetPauliAxis(Pauli[i]), 3) + " ├");
                lines[(3 * targetIndex) + 2].Append(targetIndex < max ? "└──┬──┘" : (gateName.StartsWith("M") ? "└──╥──┘" : "└─────┘"));
            }
            
            DrawVerticalLine(min, max);
            if(gateName.StartsWith("M"))
            {
                AddClassicalBit(max);
                DrawVerticalClassicalLine(max, occupancy.Count);
            }
            DrawClassicControls(max);
            NewColumn();
        }

        private void AddSwap(int q0Id, int q1Id)
        {
            if (!isLastColumnEmpty()) {
			    NewColumn();
		    }
            int q0Index = qubitIdToIndexMap[q0Id];
            int q1Index = qubitIdToIndexMap[q1Id];

		    occupancy[q0Index] = true;
		    occupancy[q1Index] = true;

		    lines[(3 * q0Index)].Append(q0Index < q1Index ? "       " : "   │   ");
		    lines[(3 * q0Index) + 1].Append("───╳───");
		    lines[(3 * q0Index) + 2].Append(q0Index < q1Index ? "   │   " : "       ");

		    lines[(3 * q1Index)].Append(q0Index < q1Index ? "   │   " : "       ");
		    lines[(3 * q1Index) + 1].Append("───╳───");
		    lines[(3 * q1Index) + 2].Append(q0Index < q1Index ? "       " : "   │   ");

		    var min = math.Min(q0Index, q1Index);
		    var max = math.Max(q0Index, q1Index);
            DrawVerticalLine(min, max);
            DrawClassicControls(max);
		    NewColumn();
        }

        private void AddSwap(int controlId, int q0Id, int q1Id)
        {
            if (!isLastColumnEmpty()) {
			    NewColumn();
		    }

            int q0Index = qubitIdToIndexMap[q0Id];
            int q1Index = qubitIdToIndexMap[q1Id];
            int controlIndex = qubitIdToIndexMap[controlId];
		    occupancy[q0Index] = true;
		    occupancy[q1Index] = true;
            occupancy[controlIndex] = true;

            var min = math.Min(controlIndex, math.Min(q0Index, q1Index));
		    var max = math.Max(controlIndex, math.Max(q0Index, q1Index));

            lines[(3 * controlIndex)].Append(controlIndex == min ? "       " : "   │   ");
            lines[(3 * controlIndex) + 1].Append("───@───");
            lines[(3 * controlIndex) + 2].Append(controlIndex == max ? "       " : "   │   ");

            // swap lines
		    lines[(3 * q0Index)].Append(q0Index == min ? "       " : "   │   ");
		    lines[(3 * q0Index) + 1].Append("───╳───");
		    lines[(3 * q0Index) + 2].Append(q0Index == max ? "       " : "   │   ");

		    lines[(3 * q1Index)].Append(q1Index == min ? "       " : "   │   ");
		    lines[(3 * q1Index) + 1].Append("───╳───");
		    lines[(3 * q1Index) + 2].Append(q1Index == max ? "       " : "   │   ");

            DrawVerticalLine(min, max);
            DrawClassicControls(max);          
		    NewColumn();
        }

        private void DrawClassicControls(int toIndex)
        {
            if(classicControls.TryPeek(out Tuple<long, bool> control))
            {
                int controlIndex = bitIdToIndexMap[(int)control.Item1];
                occupancy[controlIndex] = true;
                lines[(3 * controlIndex)].Append("   ║   " );
                lines[(3 * controlIndex) + 1].Append(control.Item2 ? "═══●═══" : "═══○═══");
                lines[(3 * controlIndex) + 2].Append("       " );

                DrawVerticalClassicalLine(toIndex, controlIndex);
            }
        }

        private void DrawVerticalLine(int min, int max)
        {
            for (int i = min; i < max; ++i) 
            {
                if (!occupancy[i])
                {
                    occupancy[i] = true;
                    lines[(3 * i)].Append("   │   ");
                    lines[(3 * i) + 1].Append(collapsed[i] ? "═══╪═══" : released[i] ? "   │   " : "───┼───");
                    lines[(3 * i) + 2].Append("   │   ");
                }                
            }
        }

        private void DrawVerticalClassicalLine(int min, int max)
        {
            for (int i = min; i < max; ++i) 
            {
                if (!occupancy[i])
                {
                    occupancy[i] = true;
                    lines[(3 * i)].Append("   ║   ");
                    lines[(3 * i) + 1].Append(collapsed[i] ? "═══╬═══" : (released[i] ? "   ║   " : "───╫───"));
                    lines[(3 * i) + 2].Append("   ║   ");
                }               
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

        public void Reset(Qubit qubit)
        {
            //TODO: discuss how to show qubit reset in a nice way
        }

        public void OnDump<T>(T location, IQArray<Qubit> qubits = null)
        {
            //TODO
        }

        public void OnMessage(string msg)
        {
            //TODO
        }
    }
}
