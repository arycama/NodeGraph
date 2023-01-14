[NodeMenuItem("Relay/Float Output")]
public partial class FloatRelayOutputNode : RelayOutputNode<float>
{
    [Output] private float input;

    public override void OnUpdateValues()
    {
        base.OnUpdateValues();
        input = Value;
    }
}

