using UnityEngine.Rendering;

[NodeMenuItem("Relay/CullingResults Output")]
public partial class CullingResultsOutputNode : RelayOutputNode<CullingResults>
{
    [Output] private CullingResults input;

    public override void OnUpdateValues()
    {
        base.OnUpdateValues();
        input = Value;
    }
}