using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEditor;
using System.Reflection;
// TODO: replace this by the new UnityEditor.Searcher package
class CreateEdgeMenuWindow : ScriptableObject, ISearchWindowProvider
{
    private NodeGraphView graphView;
    private EditorWindow window;
    private Port inputPort, outputPort;

    public static CreateEdgeMenuWindow Create(NodeGraphView graphView, EditorWindow window, Port inputPort, Port outputPort)
    {
        var result = CreateInstance<CreateEdgeMenuWindow>();
        result.graphView = graphView;
        result.window = window;
        result.inputPort = inputPort;
        result.outputPort = outputPort;
        return result;
    }

    List<SearchTreeEntry> ISearchWindowProvider.CreateSearchTree(SearchWindowContext context)
    {
        var tree = new List<SearchTreeEntry>
        {
            new SearchTreeGroupEntry(new GUIContent("Create Node"), 0),
        };

        var type = graphView.graph.NodeType;
        var validTypes = TypeCache.GetTypesDerivedFrom(typeof(BaseNode)).Where(derivedType => !derivedType.IsAbstract && (type.IsAssignableFrom(derivedType) || derivedType.IsSubclassOf(typeof(RelayInputNode)) || derivedType.IsSubclassOf(typeof(RelayOutputNode))));
        var titlePaths = new HashSet<string>();

        foreach (var nodeType in validTypes)
        {
            var fields = nodeType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            foreach (var field in fields)
            {
                var inputAttribute = field.GetCustomAttribute<InputAttribute>();
                var outputAttribute = field.GetCustomAttribute<OutputAttribute>();

                if (inputAttribute == null && outputAttribute == null)
                    continue;

                var nodeMenuAttribute = nodeType.GetCustomAttribute<NodeMenuItemAttribute>();
                var nodeName = nodeMenuAttribute == null ? nodeType.Name : nodeMenuAttribute.MenuTitle;
                var level = 0;
                var parts = nodeName.Split('/');

                if (parts.Length > 1)
                {
                    level++;
                    nodeName = parts[parts.Length - 1];
                    var fullTitleAsPath = "";

                    for (var i = 0; i < parts.Length - 1; i++)
                    {
                        var title = parts[i];
                        fullTitleAsPath += title;
                        level = i + 1;

                        // Add section title if the node is in subcategory
                        if (!titlePaths.Contains(fullTitleAsPath))
                        {
                            tree.Add(new SearchTreeGroupEntry(new GUIContent(title))
                            {
                                level = level
                            });
                            titlePaths.Add(fullTitleAsPath);
                        }
                    }
                }

                tree.Add(new SearchTreeEntry(new GUIContent($"{nodeName}:  {field.Name}"))
                {
                    level = level + 1,
                    userData = (nodeType, field.Name)
                });
            }
        }

        return tree;
    }

    // Node creation when validate a choice
    bool ISearchWindowProvider.OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
    {
        // window to graph position
        var windowMousePosition = window.rootVisualElement.ChangeCoordinatesTo(window.rootVisualElement.parent, context.screenMousePosition - window.position.position);
        var graphMousePosition = graphView.contentViewContainer.WorldToLocal(windowMousePosition);

        var data = ((Type, string))searchTreeEntry.userData;
        var node = CreateInstance(data.Item1) as BaseNode;
        node.hideFlags = HideFlags.HideInHierarchy;

        node.name = $"{data.Item1.Name}";
        node.Position = new Rect(graphMousePosition, new Vector2(100, 100));

        AssetDatabase.AddObjectToAsset(node, graphView.graph);
        //AssetDatabase.SaveAssets();

        var view = graphView.AddNode(node) ;
        var targetPort = view.inputContainer.Query<Port>().Where(port => port.portName == data.Item2);
        if (inputPort == null)
        {
            graphView.Connect(targetPort, outputPort);
        }
        else
        {
            graphView.Connect(inputPort, targetPort);
        }

        node.Initialize();

        return true;
    }
}