using UnityEngine;
using UnityEngine.Rendering;

namespace NodeGraph
{
    [NodeMenuItem("Relay/RenderTargetIdentifier Input")]
    public partial class RenderTargetIdentifierRelayInputNode : RelayInputNode<RenderTargetIdentifier>
    {
        [Input] private RenderTargetIdentifier input;

        public override RenderTargetIdentifier GetValue()
        {
            return input;
        }
    }
}