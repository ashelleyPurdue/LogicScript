using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using LogicCircuits;
using System.Diagnostics;
using System.IO;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        private MasterCircuit masterCircuit;

        private List<CheckBox> nodeBoxes = new List<CheckBox>();
        private List<LogicNode> nodes = new List<LogicNode>();

        private List<CheckBox> buttonBoxes = new List<CheckBox>();
        private List<InputButton> buttons = new List<InputButton>();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //Test IC parcing
            fileBrowser.ShowDialog();
            LogicCircuitParser parser = new LogicCircuitParser(File.ReadAllText(fileBrowser.FileName));

            masterCircuit = parser.Parse();

            //Build the controls
            BuildCircuitControls(masterCircuit);

            timer1.Enabled = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //Update the inputs for each input button
            for (int i = 0; i < buttons.Count; i++)
            {
                buttons[i].pressed = buttonBoxes[i].Checked;
            }

            //Update the circuit
            masterCircuit.ComputeAndUpdate();

            //Update the checked-values for each of the non-InputButton nodes
            for (int i = 0; i < nodes.Count; i++)
            {
                nodeBoxes[i].Checked = nodes[i].GetCurrentOutput();
            }

            timer1.Enabled = true;
        }

        private void BuildCircuitControls(IntegratedCircuit circuit)
        {
            //Creates a control for every node in the circuit, and in all of the circuit's children

            //Create controls for every child of the circuit
            for (int i = 0; i < circuit.ChildCount; i++)
            {
                BuildCircuitControls(circuit.GetChild(i));
            }

            //Create a checkbox for every node
            for (int i = 0; i < circuit.NodeCount; i++)
            {
                LogicNode node = circuit.GetNode(i);

                //Create and configure the checkbox
                CheckBox cb = new CheckBox();
                cb.Text = node.Name;
                cb.Enabled = false;

                //Add it to the flow panel
                flowPanel.Controls.Add(cb);

                //Add it to a different list depending on if it's an InputButton or not.
                if (node is InputButton)
                {
                    buttons.Add((InputButton)node);
                    buttonBoxes.Add(cb);
                    cb.Enabled = true;
                }
                else
                {
                    nodes.Add(node);
                    nodeBoxes.Add(cb);
                }
            }

        }
    }
}
