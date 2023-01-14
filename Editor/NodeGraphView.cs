using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class NodeGraphView : GraphView
{
    public NodeGraph graph { get; }

    private EditorWindow window;

    public NodeGraphView(NodeGraph graph, EditorWindow window)
    {
        this.graph = graph;
        this.window = window;

        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());
        this.AddManipulator(new ContentZoomer());
        this.StretchToParentSize();

        nodeCreationRequest = c => SearchWindow.Open(new SearchWindowContext(c.screenMousePosition), CreateNodeMenuWindow.Create(this, window));
        serializeGraphElements += SerializeGraphElementsCallback;
        canPasteSerializedData += CanPasteSerializedDataCallback;
        unserializeAndPaste += UnserializeAndPasteCallback;
        graphViewChanged = GraphViewChangedCallback;
        viewTransformChanged = ViewTransformChangedCallback;

        viewTransform.position = graph.Position;
        viewTransform.scale = graph.Scale;

        InitializeNodeViews();
    }

    private void InitializeNodeViews()
    {
        // Create a View for each node, and store in a dictionary with the node as the key, so it can be accessed below
        var nodeViewsPerNode = new Dictionary<BaseNode, NodeView>();
        foreach (var node in graph.Nodes)
        {
            var baseNodeView = new NodeView(this, node);
            AddElement(baseNodeView);
            nodeViewsPerNode.Add(node, baseNodeView);
        }

        // Init edge views
        foreach (var nodeView in nodeViewsPerNode.Values)
        {
            // Get each "Edge".
            var inputPorts = nodeView.inputContainer.Query<Port>();
            inputPorts.ForEach(inputPort =>
            {
                BaseNode outputNode;
                string connectedFieldName;

                // Check if this is an array port
                var arrayDataOpenIndex = inputPort.portName.IndexOf('[');
                if (arrayDataOpenIndex == -1)
                {
                    outputNode = nodeView.Node.GetConnectedNode(inputPort.portName, out connectedFieldName);
                }
                else
                {
                    var arrayDataCloseIndex = inputPort.portName.IndexOf(']');
                    var arrayIndexString = inputPort.portName.Substring(arrayDataOpenIndex + 1, arrayDataCloseIndex - (arrayDataOpenIndex + 1));
                    var arrayIndex = int.Parse(arrayIndexString);
                    var inputFieldName = inputPort.portName.Substring(0, arrayDataOpenIndex);

                    // Ensure array is within range. We create one extra port to allow resizing the array
                    var arrayLength = nodeView.Node.GetArraySize(inputFieldName);
                    if (arrayLength <= arrayIndex)
                        return;

                    outputNode = nodeView.Node.GetConnectedNodeArray(inputFieldName, arrayIndex, out connectedFieldName);
                }

                if (outputNode == null)
                    return;

                // If there is a connection, get the port from the portView of the connected node
                try
                {
                    var outputNodeView = nodeViewsPerNode[outputNode];
                    var outputPort = outputNodeView.outputContainer.Query<Port>().Where(port => port.portName == connectedFieldName);
                    Connect(inputPort, outputPort);
                }
                catch (KeyNotFoundException)
                {
                    Debug.LogError($"{outputNode} was not found in nodeViewsPerNode");
                    return;
                }
            });
        }
    }

    private string SerializeGraphElementsCallback(IEnumerable<GraphElement> elements)
    {
        var copiedNodes = new CopyPasteHelper();

        foreach (var nodeView in elements.OfType<NodeView>())
        {
            var nodeJson = JsonUtility.ToJson(nodeView.Node, true);
            copiedNodes.nodes.Add(new CopyPasteHelper.Data(nodeView.Node.GetType().Name, nodeJson));
        }

        var result = JsonUtility.ToJson(copiedNodes, true);
        return result;
    }

    private bool CanPasteSerializedDataCallback(string data)
    {
        return true;
    }

    private void UnserializeAndPasteCallback(string operationname, string data)
    {
        var copiedNodes = JsonUtility.FromJson<CopyPasteHelper>(data);

        foreach (var copiedNode in copiedNodes.nodes)
        {
            var node = (ScriptableObject.CreateInstance(copiedNode.type) as BaseNode);
            JsonUtility.FromJsonOverwrite(copiedNode.data, node);
            node.name = $"{copiedNode.type}";
            node.hideFlags = HideFlags.HideInHierarchy;

            AssetDatabase.AddObjectToAsset(node, graph);
            AssetDatabase.SaveAssets();
            AddNode(node);
            node.Initialize();
        }
    }

    private void ViewTransformChangedCallback(GraphView graphView)
    {
        graph.Position = viewTransform.position;
        graph.Scale = viewTransform.scale;
    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        var compatiblePorts = new List<Port>();

        ports.ForEach(dstPort =>
        {
            // Ignore ports on the same node
            if (dstPort.node == startPort.node)
                return;

            // Ignore ports of the same direction
            if (dstPort.direction == startPort.direction)
                return;

            // Ignore ports of the wrong type. (Also check for casting
            if (!dstPort.portType.IsAssignableFrom(startPort.portType))
                return;

            compatiblePorts.Add(dstPort);
        });

        return compatiblePorts;
    }

    public NodeView AddNode(BaseNode node)
    {
        var nodeView = new NodeView(this, node);
        graph.Nodes.Add(node);
        AddElement(nodeView);
        return nodeView;
    }

    public void Connect(Port inputPort, Port outputPort)
    {
        // Disconnect any existing edges
        foreach (var edge in inputPort.connections)
            RemoveElement(edge);

        inputPort.DisconnectAll();

        var inputNodeView = inputPort.node as NodeView;
        var inputNode = inputNodeView.Node;

        EditorUtility.SetDirty(inputNode);
        graph.SetAsDirty();

        if (outputPort == null || outputPort.node == null || !(outputPort.node is NodeView outputNodeView))
            return;

        var outputNode = outputNodeView.Node;

        // Connect the new port
        // First, check if this is an array port
        var arrayDataOpenIndex = inputPort.portName.IndexOf('[');
        if (arrayDataOpenIndex == -1)
        {
            inputNode.SetConnectedNode(inputPort.portName, outputNode, outputPort.portName);
        }
        else
        {
            var arrayDataCloseIndex = inputPort.portName.IndexOf(']');
            var arrayIndexString = inputPort.portName.Substring(arrayDataOpenIndex + 1, arrayDataCloseIndex - (arrayDataOpenIndex + 1));
            var arrayIndex = int.Parse(arrayIndexString);

            var inputFieldName = inputPort.portName.Substring(0, arrayDataOpenIndex);
            inputNode.SetConnectedNodeArray(inputFieldName, outputNode, outputPort.portName, arrayIndex);
            inputNodeView.AddPortViews(inputFieldName, inputNode.GetArraySize(inputFieldName) + 1, inputPort.portType);
        }

        // Create the edge
        var edgeView = new Edge
        {
            input = inputPort,
            output = outputPort,
        };

        inputPort.Connect(edgeView);
        outputPort.Connect(edgeView);
        AddElement(edgeView);
    }

    // Called when the graph view changes, used to remove the scriptableObject assets
    private GraphViewChange GraphViewChangedCallback(GraphViewChange changes)
    {
        if (changes.elementsToRemove != null)
        {
            foreach (var element in changes.elementsToRemove)
            {
                if (element is Edge edge)
                {
                    // Disconnect the edge from the target Node
                    var inputPort = edge.input;
                    var nodeView = inputPort.node as NodeView;
                    var inputNode = nodeView.Node;

                    // First, check if this is an array port
                    var arrayDataOpenIndex = inputPort.portName.IndexOf('[');
                    if (arrayDataOpenIndex == -1)
                    {
                        inputNode.SetConnectedNode(inputPort.portName, null, null);
                    }
                    else
                    {
                        var arrayDataCloseIndex = inputPort.portName.IndexOf(']');
                        var arrayIndexString = inputPort.portName.Substring(arrayDataOpenIndex + 1, arrayDataCloseIndex - (arrayDataOpenIndex + 1));
                        var arrayIndex = int.Parse(arrayIndexString);

                        var inputFieldName = inputPort.portName.Substring(0, arrayDataOpenIndex);
                        inputNode.SetConnectedNodeArray(inputFieldName, null, null, arrayIndex);
                        nodeView.RemovePortViews(inputFieldName, nodeView.Node.GetArraySize(inputFieldName));
                    }
                }

                if (element is NodeView baseNodeView)
                {
                    baseNodeView.Node.Cleanup();

                    // First remove from the node array
                    graph.Nodes.Remove(baseNodeView.Node);

                    // Now remove the child ScriptableObject from this asset
                    AssetDatabase.RemoveObjectFromAsset(baseNodeView.Node);
                    AssetDatabase.SaveAssets();
                }

                EditorUtility.SetDirty(graph);
                graph.SetAsDirty();
            }
        }

        return changes;
    }

    [Serializable]
    public class CopyPasteHelper
    {
        public List<Data> nodes = new List<Data>();

        [Serializable]
        public struct Data
        {
            public string type;
            public string data;

            public Data(string type, string data)
            {
                this.type = type;
                this.data = data;
            }
        }
    }
}