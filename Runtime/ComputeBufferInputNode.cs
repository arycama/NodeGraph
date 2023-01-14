using UnityEngine;

namespace NodeGraph
{
    [NodeMenuItem("Relay/ComputeBuffer Input")]
    public partial class ComputeBufferInputNode : RelayInputNode<ComputeBuffer>
    {
        [Input] private ComputeBuffer input;

        public override ComputeBuffer GetValue()
        {
            return input;
        }
    }
}