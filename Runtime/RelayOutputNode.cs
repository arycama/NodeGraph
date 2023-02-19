using UnityEngine;
using UnityEngine.Assertions;

namespace NodeGraph
{
    public abstract class RelayOutputNode : BaseNode
    {

    }

    public abstract partial class RelayOutputNode<T> : RelayOutputNode
    {
        [SerializeField] protected string inputName;

        protected T Value { get; private set; }

        public override void OnUpdateValues()
        {
            if (string.IsNullOrEmpty(inputName))
                return;

            if (Graph.RelayNodes.TryGetValue(inputName, out var node))
            {
                var typedNode = node as RelayInput<T>;
                Assert.IsNotNull(typedNode, $"Node {inputName} of type {node.GetType()} was not convertible to type {typeof(T)}");
                Value = typedNode.GetValue();
            }
        }

        public override bool TryGetAdditionalNode(out BaseNode node)
        {
            node = null;
            if (string.IsNullOrEmpty(inputName))
                return false;

            if (Graph.RelayNodes.TryGetValue(inputName, out var relayNode))
            {
                node = relayNode as BaseNode;
                return true;
            }

            return false;
        }
    }
}