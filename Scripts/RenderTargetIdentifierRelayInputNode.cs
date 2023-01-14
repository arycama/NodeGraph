using UnityEngine;
using UnityEngine.Rendering;

[NodeMenuItem("Relay/RenderTargetIdentifier Input")]
public partial class RenderTargetIdentifierRelayInputNode : RelayInputNode<RenderTargetIdentifier>
{
    [Input] private RenderTargetIdentifier input;

    public override RenderTargetIdentifier GetValue()
    {
        return input;
    }
}
