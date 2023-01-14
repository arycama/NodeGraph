using UnityEngine;

namespace NodeGraph
{
    [NodeMenuItem("Relay/ComputeBuffer Output")]
    public partial class ComputeBufferOutputNode : RelayOutputNode<ComputeBuffer>
    {
        [Output] private ComputeBuffer input;

        public override void OnUpdateValues()
        {
            base.OnUpdateValues();
            input = Value;
        }
    }
}