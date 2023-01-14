[NodeMenuItem("Relay/Bool Output")]
public partial class BoolRelayOutputNode : RelayOutputNode<bool>
{
    [Output] private bool input;

    public override void OnUpdateValues()
    {
        base.OnUpdateValues();
        input = Value;
    }
}
