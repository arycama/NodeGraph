using UnityEngine;

namespace NodeGraph
{
    [NodeMenuItem("Relay/Matrix4x4 Input")]
    public partial class Matrix4x4RelayInputNode : RelayInputNode<Matrix4x4>
    {
        [Input] private Matrix4x4 input;

        public override Matrix4x4 GetValue()
        {
            return input;
        }
    }
}