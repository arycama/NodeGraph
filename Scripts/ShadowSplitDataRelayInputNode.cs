using UnityEngine;
using UnityEngine.Rendering;

[NodeMenuItem("Relay/Shadow Split Input")]
public partial class ShadowSplitDataRelayInputNode : RelayInputNode<ShadowSplitData>
{
    [Input] private ShadowSplitData input;

    public override ShadowSplitData GetValue()
    {
        return input;
    }
}
