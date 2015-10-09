namespace LogicCircuits
{
    public class AndGate : LogicNode
    {
        public AndGate(string name, IntegratedCircuit circuit, bool inverted = false, NodeType type = NodeType.internalNode, params LogicNode[] inputs)
            : base(name, circuit, inverted, type, inputs) { }

        public override void ComputeOutput()
        {
            //Make sure there are enough inputs
            if (inputs.Count < 2)
            {
                throw new TooFewInputsException();
            }

            //True if all inputs are true
            computedOutput = true;

            foreach (LogicNode input in inputs)
            {
                if (input.GetCurrentOutput() == false)
                {
                    computedOutput = false;
                    break;
                }
            }

        }

    }
}
