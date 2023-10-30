using UnityEngine;

namespace NodeGraph
{
    [NodeMenuItem("Relay/Culling Planes Input")]
    public partial class CullingPlanesRelayInputNode : RelayInputNode<CullingPlanes>
    {
        [Input] private CullingPlanes input;

        public override CullingPlanes GetValue()
        {
            return input;
        }
    }
}