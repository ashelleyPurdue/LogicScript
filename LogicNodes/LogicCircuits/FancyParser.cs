using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;

namespace LogicNodes.LogicCircuits
{
    public class FancyParser
    {
        private StreamReader fileReader;

        public FancyParser(string fileName)
        {
            //Open the file

            fileReader = new StreamReader(fileName);
        }

        private string NextCommand()
        {
            //Gets the next command, separated by a semicolon

            StringBuilder builder = new StringBuilder();

            char c = (char)(fileReader.Read());
            while (c != ';')
            {
                builder.Append(c);
                c = (char)(fileReader.Read());
            }

            return builder.ToString();
        }
    }
}
