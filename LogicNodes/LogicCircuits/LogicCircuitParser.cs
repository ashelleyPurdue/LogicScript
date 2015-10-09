﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Diagnostics;

namespace LogicCircuits
{
    public class LogicCircuitParser
    {
        //Constants
        public const char END_COMMAND_TOKEN = ';';
        public const string BEGIN_IC_TOKEN = "integrated_circuit";
        public const string END_IC_TOKEN = "end_integrated_circuit";

        public const string BEGIN_BLUEPRINT_TOKEN = "blueprint";
        public const string END_BLUEPRINT_TOKEN = "end_blueprint";

        public const string LINK_TOKEN = "link";
        public const string TO_TOKEN = "to";

        public const string BUILD_TOKEN = "build";

        public const string DECLARE_INPUT_TOKEN = "input";
        public const string DECLARE_OUTPUT_TOKEN = "output";
        public const string DECLARE_OR_TOKEN = "or";
        public const string DECLARE_NOR_TOKEN = "nor";
        public const string DECLARE_AND_TOKEN = "and";
        public const string DECLARE_NAND_TOKEN = "nand";
        public const string DECLARE_INPUT_BUTTON_TOKEN = "input_button";

        //Fields
        private string text;
        private int textPos = 0;

        private MasterCircuit masterCircuit = new MasterCircuit("master");
        private Stack<IntegratedCircuitParseData> circuitStack = new Stack<IntegratedCircuitParseData>();

        private Dictionary<string, BlueprintParseData> blueprints = new Dictionary<string, BlueprintParseData>();


        //Constructors
        public LogicCircuitParser(string text)
        {
            this.text = text;

            //Start with the master circuit as the first one in the stack
            circuitStack.Push(new IntegratedCircuitParseData(masterCircuit, this));
        }

        //Methods

        public MasterCircuit Parse()
        {
            //Loops through all of the commands in the text and executes them
            textPos = 0;

            string command = NextCommand();
            while (command != null)
            {
                ExecuteCommand(command);
                command = NextCommand();
            }

            return masterCircuit;
        }

        public IntegratedCircuitParseData StackPeek()
        {
            //Peeks at the top of the stack
            return circuitStack.Peek();
        }

        private void ExecuteCommand(string command)
        {
            //Extract the opcode and parameters
            string[] commandArray = command.Split(new string[]{" "}, StringSplitOptions.RemoveEmptyEntries);
            
            //Extract the opcode
            string opCode = commandArray[0];

            //Extract the parameters
            string[] parameters = new string[commandArray.Length - 1];
            for (int i = 0; i < parameters.Length; i++)
            {
                parameters[i] = commandArray[i + 1];
            }

            //Execute the command speficied by the opcode
            if (opCode.Equals(DECLARE_OR_TOKEN))
            {
                CreateOr(false, parameters);
            }
            else if (opCode.Equals(DECLARE_NOR_TOKEN))
            {
                CreateOr(true, parameters);
            }
            else if (opCode.Equals(DECLARE_AND_TOKEN))
            {
                CreateAnd(false, parameters);
            }
            else if (opCode.Equals(DECLARE_NAND_TOKEN))
            {
                CreateAnd(true, parameters);
            }
            else if (opCode.Equals(DECLARE_INPUT_BUTTON_TOKEN))
            {
                CreateInputButton(false, parameters);
            }
            else if (opCode.Equals(LINK_TOKEN))
            {
                LinkInputs(parameters);
            }
            else if (opCode.Equals(DECLARE_INPUT_TOKEN))
            {
                CreateInput(parameters);
            }
            else if (opCode.Equals(DECLARE_OUTPUT_TOKEN))
            {
                CreateOutput(parameters);
            }
            else if (opCode.Equals(BEGIN_IC_TOKEN))
            {
                BeginIC(parameters);
            }
            else if (opCode.Equals(END_IC_TOKEN))
            {
                EndIC();
            }
            else if (opCode.Equals(BEGIN_BLUEPRINT_TOKEN))
            {
                ProcessBlueprint(parameters);
            }
            else if (opCode.Equals(BUILD_TOKEN))
            {
                BuildBlueprint(parameters);
            }
         
        }

        private string NextCommand()
        {
            //Return null if there is no next command
            if (!(textPos < text.Length))
            {
                return null;
            }

            //Move to the next non-whitespace character.
            while (Char.IsWhiteSpace(text[textPos]) && textPos < text.Length)
            {
                textPos++;
            }

            //Retrieves the next command and increments the counter as necessary.
            StringBuilder command = new StringBuilder();
            
            //Keep appending until we hit the end command token, or until we reach the end of the text.
            while (textPos < text.Length)
            {
                if (text[textPos] != END_COMMAND_TOKEN)
                {
                    command.Append(text[textPos]);
                    textPos++;
                }
                else
                {
                    textPos++;
                    break;
                }
            }

            return command.ToString();
        }

        private void ProcessNodeParams(LogicNode node, string[] parameters)
        {
            IntegratedCircuitParseData circuit = circuitStack.Peek();

            //Add the name to this circuit's parse data.
            circuit.AddNode(node);

            //Link the inputs, if there are any.
            LinkInputs(parameters);
        }

        private void LinkInputs(string[] parameters)
        {
            //Links the node specified by parameters[0] with the nodes specified in the rest of the parameters.
            IntegratedCircuitParseData circuit = circuitStack.Peek();

            //Get the base node
            LogicNode baseNode = circuit.ResolveName(parameters[0]);

            //Link it with all of the nodes in the other parameters
            for (int i = 1; i < parameters.Length; i++)
            {
                LogicNode node = circuit.ResolveName(parameters[i]);
                baseNode.ConnectInput(node);
            }
        }


        //Commands

        private void ProcessBlueprint(string[] parameters)
        {
            //parameters[0] = name of blueprint.
            //Creates BlueprintParseData

            //Throw an error if the name already exists.
            if (blueprints.ContainsKey(parameters[0]))
            {
                throw new DuplicateBlueprintNameException();
            }

            //Put the blueprint data in the dictionary
            BlueprintParseData bp = new BlueprintParseData();
            blueprints.Add(parameters[0], bp);

            //Add every following command
            string command = NextCommand();
            while (command != null)
            {
                //If it's the end of the blueprint, break.
                if (command.Equals(END_BLUEPRINT_TOKEN))
                {
                    break;
                }

                //Add the command and move on.
                bp.AddCommand(command);
                command = NextCommand();
            }
        }

        private void BuildBlueprint(string[] parameters)
        {
            //parameters[0] = blueprint name
            //parameters[1] = instance name
            //Builds an instance of the specified blueprint.

            //Get the blueprint
            BlueprintParseData bp = blueprints[parameters[0]];

            //Start an IC with the specified name
            BeginIC(new string[] { parameters[1] });

            //Execute all of the commands in the blueprint
            for (int i = 0; i < bp.Count; i++)
            {
                ExecuteCommand(bp.GetCommand(i));
            }

            //End the IC
            EndIC();
        }

        private void CreateInput(string[] parameters)
        {
            //Creates an input node for the current circuit
            //parameters[0]        = name of the created node
            //parameters[1 and on] = names of all of the gates to connect as inputs

            OrGate node = new OrGate(parameters[0], circuitStack.Peek().integratedCircuit, false, NodeType.inputNode);
            circuitStack.Peek().AddNode(node);
        }

        private void CreateOutput(string[] parameters)
        {
            //Creates an output node for the current circuit
            //parameters[0]        = name of the created node
            //parameters[1 and on] = unused

            OrGate gate = new OrGate(parameters[0], circuitStack.Peek().integratedCircuit, false, NodeType.inputNode);
            ProcessNodeParams(gate, parameters);
        }

        private void CreateOr(bool inverted, string[] parameters)
        {
            //parameters[0]        = name of the created or-gate
            //parameters[1 and on] = names of all of the gates to connect as inputs

            //Create the or-gate
            OrGate gate = new OrGate(parameters[0], circuitStack.Peek().integratedCircuit, inverted);

            ProcessNodeParams(gate, parameters);
        }

        private void CreateAnd(bool inverted, string[] parameters)
        {
            //parameters[0]        = name of the created and-gate
            //parameters[1 and on] = names of all of the gates to connect as inputs

            //Create the and-gate
            AndGate gate = new AndGate(parameters[0], circuitStack.Peek().integratedCircuit, inverted);

            ProcessNodeParams(gate, parameters);
        }

        private void CreateInputButton(bool inverted, string[] parameters)
        {
            //parameters[0]        = name of the created input button
            //parameters[1 and on] = names of all of the gates to connect as inputs

            //Create the button
            InputButton button = new InputButton(parameters[0], circuitStack.Peek().integratedCircuit, inverted);

            ProcessNodeParams(button, parameters);
        }

        private void BeginIC(string[] parameters)
        {
            //Begins a new integrated circuit.
            IntegratedCircuitParseData currentIC = circuitStack.Peek();

            //Create the new IC
            IntegratedCircuit newIC = new IntegratedCircuit(parameters[0], currentIC.integratedCircuit);

            //Add it the to child list
            currentIC.AddChild(newIC);

            //Push it to the top of the stack.
            circuitStack.Push(new IntegratedCircuitParseData(newIC, this));
        }

        private void EndIC()
        {
            //Ends the current integrated circuit
            circuitStack.Pop();
        }
    
    }

    public class BlueprintParseData
    {
        private List<string> commands = new List<string>();

        public int Count
        {
            get { return commands.Count; }
        }

        public void AddCommand(string command)
        {
            commands.Add(command);
        }

        public string GetCommand(int i)
        {
            return commands[i];
        }
    }

    public class DuplicateNodeNameException : System.Exception { }
    public class DuplicateBlueprintNameException : System.Exception { }
    public class MemberNotVisibleException : System.Exception { }
}