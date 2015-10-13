using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LogicCircuits
{
    public sealed class InstantNode : LogicNode
    {
        /*
         * Propegates its input signal instantly, rather than on the next tick.
         * Can only have one input, so it can't be used as a gate.
         * 
         * Useful for connecting input/output signals to integrated circuits without incurring extra delay.
         */

        public InstantNode(string name, IntegratedCircuit circuit, NodeType type = NodeType.internalNode, params LogicNode[] inputs)
            : base(name, circuit, false, type, inputs) { }

        protected override void Initialize()
        {
            maxInputs = 1;
        }

        public override bool GetCurrentOutput()
        {
            //Directly returns the current output of the input

            if (inputs.Count < 1)
            {
                return false;
            }

            return inputs[0].GetCurrentOutput();
        }

        public override void ComputeOutput()
        {
            //Do nothing.
        }
    }
}
