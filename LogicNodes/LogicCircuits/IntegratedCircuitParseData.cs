using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LogicCircuits
{
    public class IntegratedCircuitParseData
    {
        private LogicCircuitParser parser;

        private Dictionary<string, LogicNode> nodeNames = new Dictionary<string, LogicNode>();
        private Dictionary<string, IntegratedCircuitParseData> childNames = new Dictionary<string, IntegratedCircuitParseData>();

        public IntegratedCircuit integratedCircuit;

        private int createTime = System.DateTime.Now.Millisecond;

        public IntegratedCircuitParseData(IntegratedCircuit integratedCircuit, LogicCircuitParser parser)
        {
            this.integratedCircuit = integratedCircuit;
            this.parser = parser;
        }


        public void AddNode(LogicNode node)
        {
            if (!nodeNames.ContainsKey(node.Name))
            {
                nodeNames.Add(node.Name, node);
            }
            else
            {
                throw new DuplicateNodeNameException();
            }
        }

        public void AddChild(IntegratedCircuit circuit)
        {
            childNames.Add(circuit.Name, new IntegratedCircuitParseData(circuit, parser));
        }


        public IntegratedCircuitParseData GetChild(string childName)
        {
            return childNames[childName];
        }

        public LogicNode ResolveName(string namePath)
        {
            //Returns the LogicNode with the given name

            //Get the location of the first dot in the name
            int dotLoc = namePath.IndexOf('.');

            //If there were no dots found, return the node
            if (dotLoc < 0)
            {
                //TODO: Get this to work with a dictionary.
                //Find the node with this name.
                LogicNode node = null;
                for (int i = 0; i < integratedCircuit.NodeCount; i++)
                {
                    LogicNode n = integratedCircuit.GetNode(i);

                    if (n.Name.Equals(namePath))
                    {
                        node = n;
                        break;
                    }
                }

                //If the node wasn't found, return null.
                if (node == null)
                {
                    return null;
                }

                //If the node is trying to be accessed when it's not supposed to be visible, throw an error.
                if ((parser.StackPeek() != this) && (node.Type == NodeType.internalNode))
                {
                    throw new MemberAccessException();
                }

                //If we found it successfully, then return the node
                return node;
            }

            //If there was a dot found, call it recursively on the next child.
            string childName = namePath.Substring(0, dotLoc);
            string newNamePath = namePath.Substring(dotLoc + 1);

            return childNames[childName].ResolveName(newNamePath);
        }
    }
}
