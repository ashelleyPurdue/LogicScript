namespace LogicCircuits
{
    public class MasterCircuit : IntegratedCircuit
    {
        public MasterCircuit(string name)
            : base(name, null)
        {
        }

        public void ComputeAndUpdate()
        {
            ComputeOutputs();
            UpdateOutputs();
        }
    }
}
