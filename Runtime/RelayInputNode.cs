using UnityEngine;

namespace NodeGraph
{
    public interface RelayInput
    {

    }

    public interface RelayInput<T> : RelayInput
    {
        T GetValue();
    }

    public abstract class RelayInputNode : BaseNode
    {
    }

    public abstract partial class RelayInputNode<T> : RelayInputNode, RelayInput<T>
    {
        [SerializeField] protected string outputName;

        public override void PreUpdateNodeOrder()
        {
            base.PreUpdateNodeOrder();

            if (!string.IsNullOrEmpty(outputName))
                Graph.RelayNodes[outputName] = this;
        }

        public abstract T GetValue();
    }
}