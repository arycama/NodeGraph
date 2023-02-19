using UnityEngine;

namespace NodeGraph
{
    [NodeMenuItem("Relay/Matrix4x4 Output")]
    public partial class Matrix4x4RelayOutputNode : RelayOutputNode<Matrix4x4>
    {
        [Output] private Matrix4x4 input;

        public override void OnUpdateValues()
        {
            base.OnUpdateValues();
            input = Value;
        }
    }
}