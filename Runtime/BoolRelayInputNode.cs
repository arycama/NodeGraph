using UnityEngine;

namespace NodeGraph
{
    [NodeMenuItem("Relay/Bool Input")]
    public partial class BoolRelayInputNode : RelayInputNode<bool>
    {
        [Input] private bool input;

        public override bool GetValue()
        {
            return input;
        }
    }
}