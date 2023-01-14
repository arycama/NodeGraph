using UnityEngine;

namespace NodeGraph
{
    [NodeMenuItem("Relay/Vector4 Array Input")]
    public partial class Vector4ArrayRelayInputNode : RelayInputNode<Vector4Array>
    {
        [Input] private Vector4Array input;

        public override Vector4Array GetValue()
        {
            return input;
        }
    }
}