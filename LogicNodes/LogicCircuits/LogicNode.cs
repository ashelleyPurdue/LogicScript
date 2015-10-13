using System.Collections.Generic;

namespace LogicCircuits
{
    public abstract class LogicNode
    {
        public string Name
        {
            get { return name; }
        }

        public NodeType Type
        {
            get { return type; }
        }

        protected List<LogicNode> inputs = new List<LogicNode>();

        protected int maxInputs = -1;

        private string name;

        private bool currentOutput = false;
        private bool inverted = false;
        private NodeType type;

        protected bool computedOutput = false;


        //Constructors

        public LogicNode(string name, IntegratedCircuit circuit, bool inverted = false, NodeType type = NodeType.internalNode, params LogicNode[] inputs)
        {
            //Do the normal constructor stuff
            circuit.AddNode(this, type);
            this.inverted = inverted;
            this.name = name;
            this.type = type;

            //Connect all the inputs
            foreach (LogicNode node in inputs)
            {
                this.inputs.Add(node);
            }

            //Call the intialize method
            Initialize();
        }


        //Methods

        protected virtual void Initialize()
        {
            //Do any initialization here.
        }

        public void ConnectInput(LogicNode input)
        {
            //Connects another node as an input

            if (!inputs.Contains(input))
            {
                if ((maxInputs < 0) || (inputs.Count < maxInputs))
                {
                    inputs.Add(input);
                }
                else
                {
                    throw new TooManyInputsException();
                }
            }

        }

        public void ConnectOutput(LogicNode output)
        {
            //Connects another node as an output
            output.ConnectInput(this);
        }

        public void UpdateOutput()
        {
            //Updates the current output with the computed output.
            currentOutput = computedOutput;
        }

        public virtual bool GetCurrentOutput()
        {
            //Returns the current output.

            if (inverted)
            {
                return !currentOutput;
            }
            else
            {
                return currentOutput;
            }
        }


        //Abstract methods

        //Computes the output using the inputs.
        public abstract void ComputeOutput();
    }

    public class TooManyInputsException : System.Exception
    {
    }

    public class TooFewInputsException : System.Exception
    {
    }
}
