using Codice.Client.BaseCommands.BranchExplorer;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.MemoryProfiler;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace CodeGraph.Editor
{
    /// <summary>
    /// Class responsible for assembling nodes in the editor.
    /// </summary>
    public class CodeGraphEditorNode : Node
    {
        private CodeGraphView m_codeGraphview;

        private CodeGraphNode m_graphNode;
        private Port m_outputPort;

        private List<Port> m_dynamicPorts;
        //Change this later?
        private Type m_nodeVariableType;

        private List<Port> m_ports;
        
        private Dictionary<string, Component> m_components;

        private SerializedObject m_serializedObject;
        private SerializedProperty m_serializedProperty;

        public CodeGraphNode Node => m_graphNode;
        public List<Port> Ports => m_ports;
        public CodeGraphView CodeGraphView => m_codeGraphview;

        public CodeGraphEditorNode(CodeGraphNode node, SerializedObject codeGraphObject, CodeGraphView graphView)
        {
            this.AddToClassList("code-graph-node");

            m_serializedObject = codeGraphObject;
            m_graphNode = node;
            m_codeGraphview = graphView;

            Type typeinfo = node.GetType();
            NodeInfoAttribute info = typeinfo.GetCustomAttribute<NodeInfoAttribute>();

            title = info.Title;

            m_ports = new List<Port>();
            m_dynamicPorts = new List<Port>();
            m_components = new Dictionary<string, Component>();

            string[] depths = info.MenuItem.Split('/');
            foreach (string depth in depths)
            {
                this.AddToClassList(depth.ToLower().Replace(' ', '-'));
            }

            this.name = typeinfo.Name;

            //Order ensures output is always index 0;
            for (int i = 0; i < info.FlowOutputQuantity; i++)
            {
                CreateFlowOutputPort();
            }
            for (int i = 0; i < info.FlowInputQuantity; i++)
            {
                CreateFlowInputPort();
            }

            foreach(FieldInfo property in typeinfo.GetFields())
            {
                if(property.GetCustomAttribute<ExposedPropertyAttribute>() is ExposedPropertyAttribute exposedProperty)
                {
                    PropertyField field = DrawProperty(property.Name);
                }
                if(property.GetCustomAttribute<ExposedInputPortPropertyAttribute>() is ExposedInputPortPropertyAttribute exposedInputPortProperty)
                {
                    CreateCustomInputPort(exposedInputPortProperty.PortType, exposedInputPortProperty.PortName, exposedInputPortProperty.ToolTip);
                }
                if (property.GetCustomAttribute<ExposedOutputPortPropertyAttribute>() is ExposedOutputPortPropertyAttribute exposedOutputPortProperty)
                {
                    CreateCustomOutputPort(exposedOutputPortProperty.PortType, exposedOutputPortProperty.PortName, exposedOutputPortProperty.ToolTip);
                }
                if (property.GetCustomAttribute<ExposeVariablesFromGameObjectAttribute>() is ExposeVariablesFromGameObjectAttribute exposeVariablesFromGameObject)
                {
                    //not very resuable code atm
                    CreateMultiStepVariableSelector(property, exposeVariablesFromGameObject);
                }
            }
            
            RefreshExpandedState();
        }

        private void ObjectChanged(ChangeEvent<UnityEngine.Object> evt, VisualElement scriptInspector)
        {
            m_components.Clear();
            scriptInspector.Clear();
            var t = evt.newValue;
            if (t == null)
                return;
            GameObject go = (GameObject)t;

            List<string> ComponentList = new List<string>() { "none" };

            foreach (Component comp in go.GetComponents<MonoBehaviour>())
            {
                m_components.Add(comp.GetType().ToString(), comp);
                ComponentList.Add(comp.GetType().ToString());
            }
            DropdownField dropDown = new DropdownField("dropdown", ComponentList, 0);
            var dropDownElementInspector = new Box();

            scriptInspector.Add(dropDown);
            scriptInspector.Add(dropDownElementInspector);

            dropDown.RegisterCallback<ChangeEvent<string>, VisualElement>(DropdownFieldChanged, dropDownElementInspector);

            //trigger callback once to refresh list of connections and ports
            DropdownFieldChanged(new ChangeEvent<string>(), dropDownElementInspector);
        }

        private void DropdownFieldChanged(ChangeEvent<string> evt, VisualElement dropDownElementInspector)
        {
            //clear out previous data
            dropDownElementInspector.Clear();
            if (m_dynamicPorts.Count > 0)
            {
                List<GraphElement> elementsToRemove = new List<GraphElement>();
                foreach (Port port in m_dynamicPorts)
                {
                    foreach (Edge edge in port.connections)
                    {
                        elementsToRemove.Add(edge);
                        edge.input.Disconnect(edge);
                        edge.RemoveAt(0);
                    }
                    outputContainer.Remove(port);
                }
                UpdateCodeGraphView(elementsToRemove);
            }
            m_dynamicPorts.Clear();
            var t = evt.newValue;
            if (t == null)
                return;

            //Set up new data
            m_components.TryGetValue(t, out Component selectedComponent);
            MonoBehaviour monoComponent = (MonoBehaviour)selectedComponent;

            CreateOutputPortsOfType(monoComponent, m_nodeVariableType);
            dropDownElementInspector.Add(new InspectorElement(selectedComponent));

        }


        private void UpdateCodeGraphView(List<GraphElement> elementsToRemove)
        {
            GraphViewChange graphViewChange = new GraphViewChange();
            graphViewChange.elementsToRemove = elementsToRemove;

            m_codeGraphview.graphViewChanged?.Invoke(graphViewChange);
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

        private void CreateCustomInputPort(Type portType, string portName, string toolTip)
        {
            Port inputPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, portType);
            inputPort.portName = portName;
            inputPort.tooltip = toolTip;
            m_ports.Add(inputPort);
            inputContainer.Add(inputPort);
        }

        private void CreateCustomOutputPort(Type portType, string portName, string toolTip = "default tooltip", bool isDynamic = false)
        {
            Port outputPort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, portType);
            outputPort.portName = portName;
            outputPort.tooltip = toolTip;
            if(isDynamic) { m_dynamicPorts.Add(outputPort); }
            else { m_ports.Add(outputPort); }
            outputContainer.Add(outputPort);

        }

        private void CreateOutputPortsOfType(MonoBehaviour mono, Type type)
        {
            if(mono == null) return;
            foreach(FieldInfo field in mono.GetType().GetFields())
            {
                if (type == null)
                {
                    CreateCustomOutputPort(field.FieldType, field.Name, $"type of {field.FieldType}", true);
                }
                else if (field.FieldType == type)
                {
                    CreateCustomOutputPort(field.FieldType, field.Name, $"type of {field.FieldType}", true);
                }
            }
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
            Port outputPort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(PortTypes.FlowPort));
            outputPort.portName = "Out";
            outputPort.tooltip = "The flow output";
            m_ports.Add(outputPort);
            outputContainer.Add(outputPort);
        }

        private void CreateMultiStepVariableSelector(FieldInfo property, ExposeVariablesFromGameObjectAttribute exposeVariablesFromGameObject)
        {
            //replace this in the future as it could cause issues in cases where multiple types of variabels are present
            m_nodeVariableType = exposeVariablesFromGameObject.Type;
            PropertyField p = DrawProperty(property.Name);

            VisualElement root = p.parent;
            var scriptInspector = new Box();
            root.Add(scriptInspector);

            p.RegisterCallback<ChangeEvent<UnityEngine.Object>, VisualElement>(
                ObjectChanged, scriptInspector);
        }

        public void SavePosition()
        {
            m_graphNode.SetPosition(GetPosition());
        }

    }
}
