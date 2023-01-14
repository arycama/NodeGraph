using UnityEngine;

namespace NodeGraph
{
    [NodeMenuItem("Relay/Gpu Instance Buffers Input")]
    public partial class GpuInstanceBuffersRelayInputNode : RelayInputNode<GpuInstanceBuffers>
    {
        [Input] private GpuInstanceBuffers input;

        public override GpuInstanceBuffers GetValue()
        {
            return input;
        }
    }
}