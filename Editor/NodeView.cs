using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace NodeGraph.Editor
{
    public class NodeView : Node
    {
        private NodeGraphView graphView;
        private Dictionary<string, (VisualElement, List<PortView>)> arrayPortViews = new Dictionary<string, (VisualElement, List<PortView>)>();
        private Image previewTextureImage;
        private Dictionary<string, object> cachedPropertyValues = new();

        public BaseNode Node { get; }

        public NodeView(NodeGraphView graphView, BaseNode node)
        {
            this.graphView = graphView;
            Node = node;

            var type = node.GetType();

            // Set the position and title
            SetPosition(node.Position);
            title = ObjectNames.NicifyVariableName(type.Name.Replace("Node", string.Empty));

            // Iterate over all the fields, and add them to ports, or create propertyFields for inspector-editable fields
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            var serializedNode = new SerializedObject(node);

            foreach (var field in fields)
            {
                var inputAttribute = field.GetCustomAttribute<InputAttribute>();
                if (inputAttribute != null)
                {
                    var p = PortView.CreatePortView(Direction.Input, graphView, field.FieldType, field.Name);
                    inputContainer.Add(p);
                }

                var outputAttribute = field.GetCustomAttribute<OutputAttribute>();
                if (outputAttribute != null)
                {
                    var p = PortView.CreatePortView(Direction.Output, graphView, field.FieldType, field.Name);
                    outputContainer.Add(p);
                }

                var inputArrayAttribute = field.GetCustomAttribute<InputArrayAttribute>();
                if (inputArrayAttribute != null)
                {
                    // Create a port for each array element, plus one additional port used to resize the array when connected
                    int arrayLength;
                    try
                    {
                        arrayLength = node.GetArraySize(field.Name);
                    }
                    catch (NullReferenceException)
                    {
                        // Array might not be initialized
                        return;
                    }

                    var arrayPortList = new List<PortView>();

                    var arrayContainer = new VisualElement() { name = field.Name };
                    inputContainer.Add(arrayContainer);

                    for (var i = 0; i <= arrayLength; i++)
                    {
                        var portView = PortView.CreatePortView(Direction.Input, graphView, field.FieldType.GetElementType(), $"{field.Name}[{i}]");
                        arrayContainer.Add(portView);
                        arrayPortList.Add(portView);
                    }

                    arrayPortViews.Add(field.Name, (arrayContainer, arrayPortList));
                }

                if (field.GetCustomAttribute<SerializeField>() != null && field.GetCustomAttribute<HideInInspector>() == null)
                {
                    // Find the serializedProperty for this field, create a PropertyField for it, and add it to the main container
                    var serializedProperty = serializedNode.FindProperty(field.Name);
                    var element = new PropertyField(serializedProperty);

                    // Range attributes need extra space or they don't
                    //if(field.GetCustomAttribute<RangeAttribute>() != null)
                    {
                        var style = element.style;
                        style.width = 300;
                    }

                    element.Bind(serializedNode);
                    mainContainer.Add(element);

                    if (serializedProperty.propertyType != SerializedPropertyType.Generic)
                    {
                        cachedPropertyValues.Add(serializedProperty.name, GetSerializedPropertyValue(serializedProperty));
                        element.RegisterValueChangeCallback(OnValueChangedCallback);
                    }
                }
            }

            // PreviewTexture
            if (node.HasPreviewTexture)
            {
                node.CreatePreviewTexture(new Vector2Int(128, 128));

                previewTextureImage = new Image();
                previewTextureImage.scaleMode = ScaleMode.ScaleToFit;
                previewTextureImage.style.height = 128;
                mainContainer.Add(previewTextureImage);
                previewTextureImage.image = node.PreviewTexture;
            }
        }

        private void OnValueChangedCallback(SerializedPropertyChangeEvent evt)
        {
            var newValue = GetSerializedPropertyValue(evt.changedProperty);

            // Need to use .Equals here to ensure value comparison is used, since cachedPropertyValues contains boxed objects
            if (cachedPropertyValues.TryGetValue(evt.changedProperty.name, out var oldValue))
            {
                if ((newValue != null || oldValue == null) && (newValue == null || newValue.Equals(oldValue)))
                    return;

                // Debug.Log($"{evt.changedProperty.name}: OldValue {cachedPropertyValues[evt.changedProperty]} != NewValue {newValue}");
                graphView.graph.SetAsDirty();
                cachedPropertyValues[evt.changedProperty.name] = newValue;
                Node.NodeChanged();
            }
        }

        private object GetSerializedPropertyValue(SerializedProperty property)
        {
            switch (property.propertyType)
            {
                case SerializedPropertyType.Integer:
                    return property.intValue;
                case SerializedPropertyType.Boolean:
                    return property.boolValue;
                case SerializedPropertyType.Float:
                    return property.floatValue;
                case SerializedPropertyType.String:
                    return property.stringValue;
                case SerializedPropertyType.Color:
                    return property.colorValue;
                case SerializedPropertyType.ObjectReference:
                    return property.objectReferenceValue;
                case SerializedPropertyType.LayerMask:
                    return property.intValue;
                case SerializedPropertyType.Enum:
                    return property.enumValueIndex;
                case SerializedPropertyType.Vector2:
                    return property.vector2Value;
                case SerializedPropertyType.Vector3:
                    return property.vector3Value;
                case SerializedPropertyType.Vector4:
                    return property.vector4Value;
                case SerializedPropertyType.Rect:
                    return property.rectValue;
                case SerializedPropertyType.ArraySize:
                    return property.arraySize;
                case SerializedPropertyType.Character:
                    return property.stringValue;
                case SerializedPropertyType.AnimationCurve:
                    return property.animationCurveValue;
                case SerializedPropertyType.Bounds:
                    return property.boundsValue;
                case SerializedPropertyType.Quaternion:
                    return property.quaternionValue;
                case SerializedPropertyType.ExposedReference:
                    return property.exposedReferenceValue;
                case SerializedPropertyType.FixedBufferSize:
                    return property.fixedBufferSize;
                case SerializedPropertyType.Vector2Int:
                    return property.vector2IntValue;
                case SerializedPropertyType.Vector3Int:
                    return property.vector3IntValue;
                case SerializedPropertyType.RectInt:
                    return property.rectIntValue;
                case SerializedPropertyType.BoundsInt:
                    return property.boundsIntValue;
            }

            throw new NotImplementedException(property.propertyType.ToString());
        }

        public void AddPortViews(string fieldName, int maxIndex, Type fieldType)
        {
            var list = arrayPortViews[fieldName];

            for (var i = list.Item2.Count; i < maxIndex; i++)
            {
                var portView = PortView.CreatePortView(Direction.Input, graphView, fieldType, $"{fieldName}[{i}]");
                list.Item1.Add(portView);
                list.Item2.Add(portView);
            }
        }

        public void RemovePortViews(string fieldName, int maxIndex)
        {
            var list = arrayPortViews[fieldName];

            for (var i = list.Item2.Count - 1; i > maxIndex; i--)
            {
                list.Item1.Remove(list.Item2[i]);
                list.Item2.RemoveAt(i);
            }
        }

        public override Rect GetPosition() => Node.Position;

        public override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);
            Node.Position = newPos;
        }

        struct SerializedPropertyComparer : IEqualityComparer<SerializedProperty>
        {
            public bool Equals(SerializedProperty x, SerializedProperty y)
            {
                return x.name == y.name && x.propertyType == y.propertyType;
            }

            public int GetHashCode(SerializedProperty obj)
            {
                return obj.name.GetHashCode() ^ obj.propertyType.GetHashCode();
            }
        }
    }
}