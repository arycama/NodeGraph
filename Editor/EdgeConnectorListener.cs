using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace NodeGraph.Editor
{
    public class BaseEdgeConnectorListener : IEdgeConnectorListener
    {
        private readonly NodeGraphView graphView;

        public BaseEdgeConnectorListener(NodeGraphView graphView)
        {
            this.graphView = graphView;
        }

        void IEdgeConnectorListener.OnDropOutsidePort(Edge edge, Vector2 position)
        {
            // when on of the port is null, then the edge was created and dropped outside of a port
            if (edge.input == null || edge.output == null)
            {
                var provider = CreateEdgeMenuWindow.Create(graphView, EditorWindow.focusedWindow, edge.input, edge.output);
                var context = new SearchWindowContext(position + EditorWindow.focusedWindow.position.position);
                SearchWindow.Open(context, provider);
            }
        }

        void IEdgeConnectorListener.OnDrop(GraphView graphView, Edge edge)
        {
            (graphView as NodeGraphView).Connect(edge.input, edge.output);
        }
    }
}