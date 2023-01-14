using UnityEngine;

namespace NodeGraph
{
    [NodeMenuItem("Relay/Int Input")]
    public partial class IntRelayInputNode : RelayInputNode<int>
    {
        [Input] private int input;

        public override int GetValue()
        {
            return input;
        }
    }
}