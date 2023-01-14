using System;
using System.Collections.Generic;
using UnityEngine;

namespace NodeGraph
{
    public class NodeGraph : ScriptableObject
    {
        [SerializeField] private Vector3 scale = Vector3.one;
        [SerializeField] private Vector3 position = Vector3.zero;
        [SerializeField] private SerializableTuple<string, RenderPipelineProprerty[]>[] properties = Array.Empty<SerializableTuple<string, RenderPipelineProprerty[]>>();
        [SerializeField] private List<BaseNode> nodes = new List<BaseNode>();

        protected readonly HashSet<BaseNode> processedNodes = new HashSet<BaseNode>();
        protected readonly List<BaseNode> nodesToProcess = new List<BaseNode>();

        public List<BaseNode> Nodes => nodes;

        public Vector3 Scale { get => scale; set => scale = value; }

        public Vector3 Position { get => position; set => position = value; }

        public virtual Type NodeType => typeof(BaseNode);

        public Dictionary<string, RelayInput> RelayNodes { get; private set; } = new();

        private SortedList<int, Action> onGraphModified = new();

        public RenderPipelineProprerty GetPipelineProperty(string category, string name)
        {
            foreach (var property in properties)
            {
                if (property.Item1 != category)
                    continue;

                foreach (var item in property.Item2)
                    if (item.Name == name)
                        return item;
            }

            return default;
        }

        public void AddListener(Action action, int priority)
        {
            onGraphModified.Add(priority, action);
        }

        public void RemoveListener(Action action)
        {
            var index = onGraphModified.IndexOfValue(action);
            if (index > -1)
                onGraphModified.RemoveAt(index);
        }

        public void SetAsDirty()
        {
            foreach (var action in onGraphModified.Values)
                action.Invoke();
        }

        protected void UpdateNodeOrder(List<BaseNode> nodes)
        {
            // Clear processed nodes
            processedNodes.Clear();
            nodesToProcess.Clear();

            // Pre update nodes, for relays
            foreach (var node in nodes)
            {
                node.Graph = this;
                node.PreUpdateNodeOrder();
            }

            foreach (var node in nodes)
            {
                UpdateNode(node);
            }
        }

        protected void UpdateNodeOrder() => UpdateNodeOrder(nodes);

        protected void UpdateNode(BaseNode node)
        {
            // Skip if we've already added this node
            if (!processedNodes.Add(node))
                return;

            AddChildNodes(node);

            // Finally add the top level node
            nodesToProcess.Add(node);
        }

        public T GetNodeOfType<T>() where T : BaseNode
        {
            foreach (var node in nodes)
            {
                if (node is T typedNode)
                    return typedNode;
            }

            return default;
        }
        private void AddChildNodes(BaseNode node)
        {
            // Additional node.. used for relays. A bit hacky
            // Extra null check needed because sub graphs override relay node functionality a bit
            if (node.TryGetAdditionalNode(out var additionalNode) && additionalNode != null)
            {
                // Skip if we've already added this node
                if (processedNodes.Add(additionalNode))
                {
                    AddChildNodes(additionalNode);
                    nodesToProcess.Add(additionalNode);
                }
            }

            // Add any child nodes first
            for (var i = 0; i < node.GetNodeCount(); i++)
            {
                var connectedNode = node.GetNodeAtIndex(i);
                if (connectedNode == null)
                    continue;

                if (!processedNodes.Add(connectedNode))
                    continue;

                AddChildNodes(connectedNode);
                nodesToProcess.Add(connectedNode);
            }

            // Check array nodes too
            for (var i = 0; i < node.GetNodeArrayCount(); i++)
            {
                for (var j = 0; j < node.GetNodeArrayElementCount(i); j++)
                {
                    var connectedNode = node.GetNodeAtArrayIndex(i, j);
                    if (connectedNode == null)
                        continue;

                    if (!processedNodes.Add(connectedNode))
                        continue;

                    AddChildNodes(connectedNode);
                    nodesToProcess.Add(connectedNode);
                }
            }
        }
    }
}