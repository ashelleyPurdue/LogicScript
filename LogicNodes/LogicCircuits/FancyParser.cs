using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using LogicCircuits;
using System.Diagnostics;

namespace LogicCircuits
{
    public class FancyParser
    {
        private StreamReader fileReader;
        private MasterCircuit masterCircuit;

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

        public MasterCircuit GetMasterCircuit()
        {
            return masterCircuit;
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
    }
}
