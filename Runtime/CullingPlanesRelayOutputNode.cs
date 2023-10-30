namespace NodeGraph
{
    [NodeMenuItem("Relay/Culling Planes Output")]
    public partial class CullingPlanesRelayOutputNode : RelayOutputNode<CullingPlanes>
    {
        [Output] private CullingPlanes input;

        public override void OnUpdateValues()
        {
            base.OnUpdateValues();
            input = Value;
        }
    }
}