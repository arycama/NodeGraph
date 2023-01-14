using UnityEngine;
using UnityEngine.Rendering;

namespace NodeGraph
{
    [NodeMenuItem("Relay/Shadow Split Input")]
    public partial class ShadowSplitDataRelayInputNode : RelayInputNode<ShadowSplitData>
    {
        [Input] private ShadowSplitData input;

        public override ShadowSplitData GetValue()
        {
            return input;
        }
    }
}