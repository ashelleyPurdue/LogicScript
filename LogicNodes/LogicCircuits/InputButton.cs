using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LogicCircuits
{
    public class InputButton : LogicNode
    {
        public bool pressed = false;

        public InputButton(string name, IntegratedCircuit circuit, bool inverted = false, NodeType type = NodeType.internalNode, params LogicNode[] inputs)
            : base(name, circuit, inverted, type, inputs) { }

        public override void ComputeOutput()
        {
            computedOutput = pressed;
        }
    }
}
