using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEditor;
using System.Reflection;
using System.IO;

// TODO: replace this by the new UnityEditor.Searcher package
class CreateNodeMenuWindow : ScriptableObject, ISearchWindowProvider
{
    NodeGraphView graphView { get; set; }
    EditorWindow window { get; set; }

    public static CreateNodeMenuWindow Create(NodeGraphView graphView, EditorWindow window)
    {
        var result = CreateInstance<CreateNodeMenuWindow>();
        result.graphView = graphView;
        result.window = window;
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
            var nodeMenuItem = nodeType.GetCustomAttribute<NodeMenuItemAttribute>();
            var nodePath = nodeMenuItem != null ? nodeMenuItem.MenuTitle : nodeType.Name;
            var nodeName = nodePath;
            var level = 0;
            var parts = nodePath.Split('/');

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
                        // Add section title if the node is in subcategory
                        tree.Add(new SearchTreeGroupEntry(new GUIContent(title))
                        {
                            level = level
                        });
                        titlePaths.Add(fullTitleAsPath);
                    }
                }
            }

            tree.Add(new SearchTreeEntry(new GUIContent(nodeName))
            {
                level = level + 1,
                userData = nodeType
            });
        }

        return tree;
    }

    // Node creation when validate a choice
    bool ISearchWindowProvider.OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
    {
        // window to graph position
        var windowMousePosition = window.rootVisualElement.ChangeCoordinatesTo(window.rootVisualElement.parent, context.screenMousePosition - window.position.position);
        var graphMousePosition = graphView.contentViewContainer.WorldToLocal(windowMousePosition);

        var nodeType = searchTreeEntry.userData as Type;

        var node = CreateInstance(nodeType) as BaseNode;
        node.hideFlags = HideFlags.HideInHierarchy;
        node.name = $"{nodeType.Name}";

        AssetDatabase.AddObjectToAsset(node, graphView.graph);
        //AssetDatabase.SaveAssets();

        node.Position = new Rect(graphMousePosition, new Vector2(100, 100));
        graphView.AddNode(node);
        node.Initialize();

        return true;
    }
}