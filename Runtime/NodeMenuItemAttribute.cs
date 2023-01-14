using System;

namespace NodeGraph
{
    public class NodeMenuItemAttribute : Attribute
    {
        public string MenuTitle { get; }

        public NodeMenuItemAttribute(string menuTitle = null)
        {
            this.MenuTitle = menuTitle;
        }
    }
}