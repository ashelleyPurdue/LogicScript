using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using LogicCircuits;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace LogicCircuits
{
    public class FancyParser
    {
        private StreamReader fileReader;
        private MasterCircuit masterCircuit;

        //RegExes

        private static Regex buttonCreationPattern = new Regex("inputButton\\s+\\w+");
        private static Regex nodeCreationPattern = new Regex(".+=.+");

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

            //If it's a button command...
            if (buttonCreationPattern.IsMatch(command))
            {
                CreateButton(command);
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

            masterCircuit.AddNode(new InputButton(name, masterCircuit));

            //TODO: Add to the name list.
        }
    }
}
