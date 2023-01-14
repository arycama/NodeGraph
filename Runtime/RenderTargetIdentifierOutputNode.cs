using UnityEngine.Rendering;

namespace NodeGraph
{
    [NodeMenuItem("Relay/RenderTargetIdentifier Output")]
    public partial class RenderTargetIdentifierOutputNode : RelayOutputNode<RenderTargetIdentifier>
    {
        [Output] private RenderTargetIdentifier input;

        public override void OnUpdateValues()
        {
            base.OnUpdateValues();
            input = Value;
        }
    }
}