using System;
using UnityEngine;

namespace CodeGraph
{
    /// <summary>
    /// <para> This attribute defines the properties of nodes. </para>
    /// <see cref="string"/> title, <see cref="string"/> menuItem, <see cref="int"/>  flowInputQuantity, <see cref="int"/>  flowOutputQuantity
    /// </summary>
    public class NodeInfoAttribute : Attribute
    {
        private string m_nodeTitle;
        private string m_menuItem;

        private int m_flowInputQuantity;
        private int m_flowOutputQuantity;

        public string Title => m_nodeTitle;
        public string MenuItem => m_menuItem;

        public int FlowInputQuantity => m_flowInputQuantity;
        public int FlowOutputQuantity => m_flowOutputQuantity;


        public NodeInfoAttribute(string title, string menuItem = "", int flowInputQuantity = 1, int flowOutputQuantity = 1)
        {
            m_nodeTitle = title;
            m_menuItem = menuItem;
            m_flowInputQuantity = flowInputQuantity;
            m_flowOutputQuantity = flowOutputQuantity;
        }
    }
}
