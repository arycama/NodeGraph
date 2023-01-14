using UnityEngine;
using UnityEngine.Rendering;

namespace NodeGraph
{
    [NodeMenuItem("Relay/CullingResults Input")]
    public partial class CullingResultsRelayInputNode : RelayInputNode<CullingResults>
    {
        [Input] private CullingResults input;

        public override CullingResults GetValue()
        {
            return input;
        }
    }
}