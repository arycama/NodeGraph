namespace NodeGraph
{
    [NodeMenuItem("Relay/Vector4 Array Output")]
    public partial class Vector4ArrayRelayOutputNode : RelayOutputNode<Vector4Array>
    {
        [Output] private Vector4Array input;

        public override void OnUpdateValues()
        {
            base.OnUpdateValues();
            input = Value;
        }
    }
}