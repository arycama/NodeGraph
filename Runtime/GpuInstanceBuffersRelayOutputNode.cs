namespace NodeGraph
{
    [NodeMenuItem("Relay/Gpu Instance Buffers Output")]
    public partial class GpuInstanceBuffersRelayOutputNode : RelayOutputNode<GpuInstanceBuffers>
    {
        [Output] private GpuInstanceBuffers input;

        public override void OnUpdateValues()
        {
            base.OnUpdateValues();
            input = Value;
        }
    }
}