namespace NodeGraph
{
    [NodeMenuItem("Relay/Int Output")]
    public partial class IntRelayOutputNode : RelayOutputNode<int>
    {
        [Output] private int input;

        public override void OnUpdateValues()
        {
            base.OnUpdateValues();
            input = Value;
        }
    }
}