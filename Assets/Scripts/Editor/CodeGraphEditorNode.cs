using Codice.Client.BaseCommands.BranchExplorer;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;

namespace CodeGraph.Editor
{
    /// <summary>
    /// Class responsible for assembling nodes in the editor.
    /// </summary>
    public class CodeGraphEditorNode : Node
    {

        private CodeGraphNode m_graphNode;
        private Port m_outputPort;

        private List<Port> m_ports;

        private SerializedObject m_serializedObject;
        private SerializedProperty m_serializedProperty;

        public CodeGraphNode Node => m_graphNode;
        public List<Port> Ports => m_ports;

        public CodeGraphEditorNode(CodeGraphNode node, SerializedObject codeGraphObject)
        {
            this.AddToClassList("code-graph-node");

            m_serializedObject = codeGraphObject;
            m_graphNode = node;

            Type typeinfo = node.GetType();
            NodeInfoAttribute info = typeinfo.GetCustomAttribute<NodeInfoAttribute>();

            title = info.title;

            m_ports = new List<Port>();
            string[] depths = info.menuItem.Split('/');
            foreach (string depth in depths)
            {
                this.AddToClassList(depth.ToLower().Replace(' ', '-'));
            }

            this.name = typeinfo.Name;

            //We do this so output is always index 0;
            for (int i = 0; i < info.flowOutputQuantity; i++)
            {
                CreateFlowOutputPort();
            }
            for (int i = 0; i < info.flowInputQuantity; i++)
            {
                CreateFlowInputPort();
            }

            foreach(FieldInfo property in typeinfo.GetFields())
            {
                if(property.GetCustomAttribute<ExposedPropertyAttribute>() is ExposedPropertyAttribute exposedProperty)
                {
                    PropertyField field = DrawProperty(property.Name);
                    //field.RegisterValueChangeCallback(OnFieldChangeCallback);
                }
            }

            RefreshExpandedState();
        }

        private void FetchSerializedProperty()
        {
            SerializedProperty nodes = m_serializedObject.FindProperty("m_nodes");
            if (nodes.isArray)
            {
                int size = nodes.arraySize;
                for (int i = 0; i < size; i++)
                {
                    var element = nodes.GetArrayElementAtIndex(i);
                    var elementId = element.FindPropertyRelative("m_guid");
                    if (elementId.stringValue == m_graphNode.id)
                    {
                        m_serializedProperty = element;
                    }
                }
            }
        }

        private PropertyField DrawProperty(string propertyName)
        {
            if(m_serializedProperty == null)
            {
                FetchSerializedProperty();
            }
            SerializedProperty prop = m_serializedProperty.FindPropertyRelative(propertyName);
            PropertyField field = new PropertyField(prop);
            field.bindingPath = prop.propertyPath;
            extensionContainer.Add(field);
            return field;
        }



        private void CreateFlowInputPort()
        {
            Port inputPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(PortTypes.FlowPort));
            inputPort.portName = "In";
            inputPort.tooltip = "The flow input";
            m_ports.Add(inputPort);
            inputContainer.Add(inputPort);
        }

        private void CreateFlowOutputPort()
        {
            m_outputPort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(PortTypes.FlowPort));
            m_outputPort.portName = "Out";
            m_outputPort.tooltip = "The flow output";
            m_ports.Add(m_outputPort);
            outputContainer.Add(m_outputPort);
        }



        public void SavePosition()
        {
            m_graphNode.SetPosition(GetPosition());
        }

    }
}
