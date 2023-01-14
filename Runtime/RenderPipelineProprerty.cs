using System;
using UnityEngine;

namespace NodeGraph
{
    [Serializable]
    public class RenderPipelineProprerty
    {
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public RenderPipelinePropertyType Type { get; private set; }
        [field: SerializeField] public bool BoolValue { get; private set; }
        [field: SerializeField] public int IntValue { get; private set; }
        [field: SerializeField] public float FloatValue { get; private set; }
        [field: SerializeField] public string StringValue { get; private set; }
        [field: SerializeField] public Color ColorValue { get; private set; }
    }

    public enum RenderPipelinePropertyType
    {
        Bool,
        Int,
        Float,
        String,
        Color
    }
}