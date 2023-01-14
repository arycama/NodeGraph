using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

namespace NodeGraph.Editor
{
    public class PortView : Port
    {
        private static readonly Dictionary<Type, Color> portColors = new()
    {
        {typeof(RenderTargetIdentifier), Color.green },
        {typeof(ComputeBuffer), Color.yellow },
        {typeof(NodeConnection), Color.magenta },
        {typeof(AttachmentDescriptor), Color.white },
    };

        private PortView(Orientation portOrientation, Direction portDirection, Capacity portCapacity, Type type) : base(portOrientation, portDirection, portCapacity, type)
        {
        }

        public static PortView CreatePortView(Direction direction, NodeGraphView graphView, Type fieldType, string name)
        {
            var pv = new PortView(Orientation.Horizontal, direction, Capacity.Multi, fieldType)
            {
                m_EdgeConnector = new BaseEdgeConnector(graphView),
                portName = name,
                portType = fieldType
            };

            if (portColors.TryGetValue(fieldType, out var portColor))
                pv.portColor = portColor;

            pv.AddManipulator(pv.m_EdgeConnector);
            pv.AddToClassList(pv.portName);
            return pv;
        }
    }
}