using System.Diagnostics;

namespace LogicCircuits
{
    public class OrGate : LogicNode
    {
        public OrGate(string name, IntegratedCircuit circuit, bool inverted = false, NodeType type = NodeType.internalNode, params LogicNode[] inputs)
            : base(name, circuit, inverted, type, inputs) { }

        public override void ComputeOutput()
        {
            //True if any of the inputs is true

            computedOutput = false;

            foreach (LogicNode input in inputs)
            {
                //DEBUG: Error stuff
                if (input == null)
                {
                    Debug.WriteLine("null input!");
                    Debug.WriteLine("node name: " + Name);
                    Debug.WriteLine("input count: " + inputs.Count);
                }

                //
                if (input.GetCurrentOutput() == true)
                {
                    computedOutput = true;
                    break;
                }
            }
        }

    }
}
