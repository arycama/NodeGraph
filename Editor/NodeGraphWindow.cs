using System;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

public class NodeGraphWindow : EditorWindow
{
    private static bool skipLastGraphLoad;

    private NodeGraph graph;
    private NodeGraphView graphView;

    [OnOpenAsset(0)]
    public static bool OnBaseGraphOpened(int instanceID, int line)
    {
        if (EditorUtility.InstanceIDToObject(instanceID) is NodeGraph graph)
        {
            // As we can't pass data to OnEnable (Which is called inside GetWindow), set a static bool to tell it not
            // to load the last graph. Set it back to false after
            skipLastGraphLoad = true;
            var window = GetWindow<NodeGraphWindow>(graph.name);

            if(window.graph != null && window.graph != graph)
            {
                window = EditorWindow.CreateWindow<NodeGraphWindow>(graph.name);
            }

            skipLastGraphLoad = false;
            window.OpenGraph(graph);
            return true;
        }

        return false;
    }

    private void OpenGraph(NodeGraph graph)
    {
        // If this graph is double clicked when already open, don't re-open it
        if (graph == this.graph)
        {
            //return;
        }

        this.graph = graph;
        if (graphView != null)
        {
            rootVisualElement.Remove(graphView);
        }

        graphView = new NodeGraphView(graph, this);
        rootVisualElement.Add(graphView);
    }


    private void OnEnable()
    {
        // This is set when an asset is opened by double clicking, in this case we don't want to open the last graph
        if (skipLastGraphLoad)
            return;

        // A graph may already be loaded by OnBaseGraphOpened, so we don't want to load a different one
        var newGraph = graph;
        if (newGraph == null)
        {
            var lastGraphGuid = EditorPrefs.GetString("NodeGraphWindow.LastGraphGuid");
            if (!string.IsNullOrEmpty(lastGraphGuid))
            {
                var graphPath = AssetDatabase.GUIDToAssetPath(lastGraphGuid);
                newGraph = AssetDatabase.LoadAssetAtPath<NodeGraph>(graphPath);
            }

            // If we didn't end up loading a graph, return
            if (newGraph == null)
            {
                return;
            }
        }

        OpenGraph(newGraph);
    }

    private void OnDisable()
    {
        rootVisualElement.Remove(graphView);
        graphView = null;

        if (graph != null)
        {
            var lastGraphPath = AssetDatabase.GetAssetPath(graph);
            var lastGraphGuid = AssetDatabase.AssetPathToGUID(lastGraphPath);
            EditorPrefs.SetString("NodeGraphWindow.LastGraphGuid", lastGraphGuid);
        }
    }
}