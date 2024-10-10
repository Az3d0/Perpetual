using System;
using UnityEngine;

namespace CodeGraph
{
    public class ExposedInputPortPropertyAttribute : Attribute
    {
        private Type m_portType;
        private string m_portName;
        private string m_toolTip;

        public Type PortType => m_portType;
        public string PortName => m_portName;
        public string ToolTip => m_toolTip;

        public ExposedInputPortPropertyAttribute(Type portType, string portName, string toolTip)
        {
            m_portType = portType;
            m_portName = portName;
            m_toolTip = toolTip;
        }
    }
}
