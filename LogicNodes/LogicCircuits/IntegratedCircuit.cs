using System.Collections.Generic;

namespace LogicCircuits
{
    public class IntegratedCircuit
    {
        public string Name
        {
            get { return name; }
        }

        public int NodeCount { get { return nodes.Count; } }
        public int InputCount { get { return inputNodes.Count; } }
        public int OutputCount { get { return outputNodes.Count; } }
        public int ChildCount { get { return children.Count; } }

        private string name;

        private List<LogicNode> nodes = new List<LogicNode>();
        private List<LogicNode> inputNodes = new List<LogicNode>();
        private List<LogicNode> outputNodes = new List<LogicNode>();

        private List<IntegratedCircuit> children = new List<IntegratedCircuit>();

        public IntegratedCircuit(string name, IntegratedCircuit parent)
        {
            if (parent != null)
            {
                parent.AddChildCircuit(this);
            }

            this.name = name;
        }


        public LogicNode GetInput(int i)
        {
            return inputNodes[i];
        }

        public LogicNode GetOutput(int i)
        {
            return outputNodes[i];
        }

        public LogicNode GetNode(int i)
        {
            return nodes[i];
        }

        public IntegratedCircuit GetChild(int i)
        {
            return children[i];
        }

        public void AddChildCircuit(IntegratedCircuit circuit)
        {
            //Adds a child
            if (!children.Contains(circuit))
            {
                children.Add(circuit);
            }
        }

        public void AddNode(LogicNode node, NodeType type = NodeType.internalNode)
        {
            //Adds a node to the circuit.

            if (!nodes.Contains(node))
            {
                nodes.Add(node);

                //If it's an input or an output node, add it to the respective lists.
                if (type == NodeType.inputNode)
                {
                    inputNodes.Add(node);
                }
                else if (type == NodeType.outputNode)
                {
                    outputNodes.Add(node);
                }
            }
        }

        public void ComputeOutputs()
        {
            //Computes the outputs for all nodes.

            //Run for all nodes
            foreach (LogicNode n in nodes)
            {
                n.ComputeOutput();
            }

            //Run for all child circuits
            foreach (IntegratedCircuit c in children)
            {
                c.ComputeOutputs();
            }
        }

        public void UpdateOutputs()
        {
            //Update the outputs for all nodes

            //Run for all nodes
            foreach (LogicNode n in nodes)
            {
                n.UpdateOutput();
            }

            //Run for all child circuits
            foreach (IntegratedCircuit c in children)
            {
                c.UpdateOutputs();
            }
        }
    }
}
