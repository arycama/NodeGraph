using UnityEngine;

[NodeMenuItem("Relay/Float Input")]
public partial class FloatRelayInputNode : RelayInputNode<float>
{
    [Input] private float input;

    public override float GetValue()
    {
        return input;
    }
}
