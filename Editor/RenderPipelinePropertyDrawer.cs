using System;
using UnityEditor;
using UnityEngine;

namespace NodeGraph.Editor
{
    [CustomPropertyDrawer(typeof(RenderPipelineProprerty))]
    public class RenderPipelinePropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var nameField = property.FindPropertyRelative("<Name>k__BackingField");

            nameField.stringValue = EditorGUI.TextField(new Rect(position.x, position.y, position.width * 0.4f, position.height), nameField.stringValue);

            var typeProperty = property.FindPropertyRelative("<Type>k__BackingField");
            EditorGUI.PropertyField(new Rect(position.x + position.width * 0.4f, position.y, position.width * 0.2f, position.height), typeProperty, GUIContent.none);

            string propertyName;
            var type = (RenderPipelinePropertyType)typeProperty.enumValueIndex;
            switch (type)
            {
                case RenderPipelinePropertyType.Bool:
                    propertyName = "<BoolValue>k__BackingField";
                    break;
                case RenderPipelinePropertyType.Int:
                    propertyName = "<IntValue>k__BackingField";
                    break;
                case RenderPipelinePropertyType.Float:
                    propertyName = "<FloatValue>k__BackingField";
                    break;
                case RenderPipelinePropertyType.String:
                    propertyName = "<StringValue>k__BackingField";
                    break;
                case RenderPipelinePropertyType.Color:
                    propertyName = "<ColorValue>k__BackingField";
                    break;

                default:
                    throw new NotSupportedException(type.ToString());
            }

            var valueProperty = property.FindPropertyRelative(propertyName);
            EditorGUI.PropertyField(new Rect(position.x + position.width * 0.6f, position.y, position.width * 0.4f, position.height), valueProperty, GUIContent.none);
        }
    }
}