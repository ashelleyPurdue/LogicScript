using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using LogicCircuits;
using System.Diagnostics;
using System.Text.RegularExpressions;

//CURRENT TASK: Redoing expressions

namespace LogicCircuits
{
    public class FancyParser
    {
        private StreamReader fileReader;
        private MasterCircuit masterCircuit;

        private Dictionary<string, LogicNode> nameBook = new Dictionary<string, LogicNode>(); //All logic nodes and their names in the current context

        //RegExes

        private static Regex buttonCreationPattern = new Regex("inputButton\\s+\\w+");
        private static Regex nodeCreationPattern = new Regex(".+=.+");

        private static Regex expressionPattern = new Regex("((\\w+|\\(.+\\))\\s*(&&|\\|\\|)\\s*)*(\\w+|\\(.+\\))\\s*(&&|\\|\\|)\\s*(\\w+|\\|\\(.+\\))");   //I wish YACC worked for C#

        private static Regex simpleExpressionPattern = new Regex("\\w+\\s*(&&|\\|\\|)\\s*\\w+");
        private static Regex simpleNamePattern = new Regex("\\s*\\w+\\s*");
        private static Regex simpleAndPattern = new Regex("\\w+\\s*&&\\s*\\w+");

        //Constructors

        public FancyParser(string fileName, string masterCircuitName)
        {
            //Start the master circuit
            masterCircuit = new MasterCircuit(masterCircuitName);

            //Open the file
            fileReader = new StreamReader(fileName);

            //Parse the file, generating the circuits
            Parse();

            //Close the reader.
            fileReader.Close();
        }


        //Interface

        public MasterCircuit GetMasterCircuit()
        {
            return masterCircuit;
        }


        //Misc methods

        private LogicNode ResolveName(string name)
        {
            //Takes a name and returns the logic node
            //TODO: Take integrated circuits into account

            Console.WriteLine("Resolving name " + name + " as " + nameBook[name].Name);
           
            return nameBook[name];
        }

        private void Parse()
        {
            //Execute every command.

            string command = NextCommand();
            while (command != null)
            {
                ExecuteCommand(command);
                command = NextCommand();
            }
        }

        private void ExecuteCommand(string command)
        {
            //TODO: Execute the command
            Console.WriteLine(command);

            if (buttonCreationPattern.IsMatch(command))
            {
                //If it's a button command...
                Console.WriteLine("Creating button.");
                CreateButton(command);
                Console.WriteLine("Button created.");
            }
            else if (nodeCreationPattern.IsMatch(command))
            {
                //If it's a build node command...
                Console.WriteLine("Building node.");
                BuildNode(command);
                Console.WriteLine("Node built.");
            }
        }

        private string NextCommand()
        {
            //Gets the next command, separated by a semicolon
            //Returns null if there isn't a next command.

            StringBuilder builder = new StringBuilder();

            while (true)
            {
                //Return null if end of stream
                if (fileReader.EndOfStream)
                {
                    return null;
                }

                //Get the next c
                char c = (char)(fileReader.Read());

                //If c is a semicolon, that's the end of the command.
                if (c == ';')
                {
                    return builder.ToString();
                }

                //Skip newlines
                if (c == '\r' || c == '\n')
                {
                    continue;
                }

                //Append c to the builder
                builder.Append(c);
            }
        }

        //Command methods

        private void CreateButton(string command)
        {
            //Creates a button
            string[] words = command.Split(' ');
            string name = words[1];

            LogicNode button = new InputButton(name, masterCircuit);

            masterCircuit.AddNode(button);

            //TODO: Add to the name list.
            //TODO: Support for integrated circuits
            nameBook.Add(name, button);
        }

        private void BuildNode(string command)
        {
            //Builds a node based on the given expression
            //TODO: Support for integrated circuits, not just master

            //Get the name from the command
            string nodeName = simpleNamePattern.Match(command).ToString();

            //Get the expression from the command
            string expression = expressionPattern.Match(command).ToString();

            //Build the expression
            Console.WriteLine("Building expression " + expression);
            LogicNode node = BuildExpression(expression, nodeName, masterCircuit);
            masterCircuit.AddNode(node);
            Console.WriteLine("Expression built");

            //Add to the name list
            nameBook.Add(nodeName, node);
        }

        //Command helper methods

        private LogicNode BuildExpression(string expression, string name, IntegratedCircuit circuit)
        {
            //Recursively builds a node network out of an expression
            //TODO: Do it recursively, with support for parentheses.

            return BuildSimpleExpression(expression, name, circuit);
        }
        
        private LogicNode BuildSimpleExpression(string expression, string name, IntegratedCircuit circuit)
        {
            //Builds a logic node network represented by the simple expression

            LogicNode outNode = null;

            //Extract the names
            MatchCollection nameMatches = simpleNamePattern.Matches(expression);
            List<string> names = new List<string>();

            foreach (Match match in nameMatches)
            {
                Console.WriteLine("Extracted name " + match.ToString());
                names.Add(match.ToString().Trim());
            }

            Console.WriteLine("Extracted names");

            //Get the inputs
            LogicNode inputA = ResolveName(names[0]);
            LogicNode inputB = ResolveName(names[1]);

            Console.WriteLine("Resolved names");

            //Build an and gate
            if (simpleAndPattern.IsMatch(expression))
            {
                outNode = new AndGate("unnamed", circuit, false, NodeType.outputNode);
            }
            //TODO: Build other gates

            Console.WriteLine("Built gate");

            //Connect the inputs and return
            outNode.ConnectInput(inputA);
            outNode.ConnectInput(inputB);

            Console.WriteLine("Connected input.");

            return outNode;
        }
    }
}
