using UnityEngine.Rendering;

namespace NodeGraph
{
    [NodeMenuItem("Relay/ShadowSplitData Output")]
    public partial class ShadowSplitDataRelayOutputNode : RelayOutputNode<ShadowSplitData>
    {
        [Output] private ShadowSplitData input;

        public override void OnUpdateValues()
        {
            base.OnUpdateValues();
            input = Value;
        }
    }
}