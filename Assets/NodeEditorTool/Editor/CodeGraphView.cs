using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace CodeGraph.Editor
{
    public class CodeGraphView : GraphView
    {
        private CodeGraphAsset m_codeGraph;
        private SerializedObject m_serializedObject;
        private CodeGraphEditorWindow m_window;

        public CodeGraphEditorWindow window => m_window;

        public List<CodeGraphEditorNode> m_graphNodes;
        public Dictionary<string, CodeGraphEditorNode> m_nodeDictionary;
        public Dictionary<Edge, CodeGraphConnection> m_connectionDictionary;

        private CodeGraphWindowSearchProvider m_searchProvider;
        public CodeGraphView(SerializedObject serializedObject, CodeGraphEditorWindow window)
        {
            //instantiate
            m_serializedObject = serializedObject;
            m_codeGraph = (CodeGraphAsset)serializedObject.targetObject;
            m_window = window;

            m_graphNodes = new List<CodeGraphEditorNode>();
            m_nodeDictionary = new Dictionary<string, CodeGraphEditorNode>();
            m_connectionDictionary = new Dictionary<Edge, CodeGraphConnection>();

            m_searchProvider = ScriptableObject.CreateInstance<CodeGraphWindowSearchProvider>();
            m_searchProvider.graph = this;
            this.nodeCreationRequest = ShowSearchWindow;

            //Styling dictated by USS file. Currently included styles: grid
            StyleSheet style = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/NodeEditorTool/Editor/USS/CodeGraphEditor.uss");
            styleSheets.Add(style);

            //Add GridBackground as a child of RootVisualElement/CodeGraphView/VisualElement
            GridBackground background = new GridBackground();
            background.name = "Grid";
            Add(background);
            background.SendToBack();

            //Add grid navigation and content manipulation features 
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            this.AddManipulator(new ClickSelector());

            //Draws already existing nodes + connections on creation. (Otherwise existing nodes would not have a visual representation after window is reloaded)
            DrawNodes();
            DrawConnections();

            //Make sure this is after initial draw methods, otherwise draw methods will infinitely trigger the event.
            graphViewChanged += OnGraphViewChangedEvent;
            
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            List<Port> allPorts = new List<Port>();
            List<Port> ports = new List<Port>();

            foreach(var node in m_graphNodes)
            {
                allPorts.AddRange(node.Ports);
            }
            foreach (Port p in allPorts)
            {
                if(p == startPort) { continue; }
                if(p.node == startPort.node) { continue; }
                if(p.direction == startPort.direction) { continue; }
                if(p.portType == startPort.portType)
                {
                    ports.Add(p);
                }
            }
            return ports;
        }

        // called when any change happens to graphview
        private GraphViewChange OnGraphViewChangedEvent(GraphViewChange graphViewChange)
        {
            //on element moved
            if(graphViewChange.movedElements != null)
            {
                Undo.RecordObject(m_serializedObject.targetObject, "Moved Element");
                foreach(CodeGraphEditorNode editorNode in graphViewChange.movedElements.OfType<CodeGraphEditorNode>())
                {
                    editorNode.SavePosition();
                }
            }
            //on element removed
            if(graphViewChange.elementsToRemove != null)
            {
                List<CodeGraphEditorNode> nodes =graphViewChange.elementsToRemove.OfType<CodeGraphEditorNode>().ToList();

                //remove node
                if(nodes.Count > 0)
                {
                    Undo.RecordObject(m_serializedObject.targetObject, "Removed Element From Graph");
                    for (int i = nodes.Count - 1; i >= 0; i--)
                    {
                        RemoveNode(nodes[i]);
                    }
                }

                //remove connection
                foreach(Edge e in graphViewChange.elementsToRemove.OfType<Edge>())
                {
                    RemoveConnection(e);
                }

            }
            //on connection added
            if(graphViewChange.edgesToCreate != null)
            {
                Undo.RecordObject(m_serializedObject.targetObject, "Added Connections");
                foreach (Edge edge in graphViewChange.edgesToCreate)
                {
                    CreateEdge(edge); 
                }
            }
            return graphViewChange;
        }

        private void CreateEdge(Edge edge)
        {
            CodeGraphEditorNode inputNode = (CodeGraphEditorNode)edge.input.node;
            int inputIndex = inputNode.Ports.IndexOf(edge.input);
            
            CodeGraphEditorNode outputNode = (CodeGraphEditorNode)edge.output.node;
            int outputIndex = outputNode.Ports.IndexOf(edge.output);

            CodeGraphConnection connection = new CodeGraphConnection(inputNode.Node.id, inputIndex, outputNode.Node.id, outputIndex);
            m_codeGraph.Connections.Add(connection);
            m_connectionDictionary.Add(edge, connection);
        }
        private void RemoveConnection(Edge e)
        {
            if(m_connectionDictionary.TryGetValue(e, out CodeGraphConnection connection))
            {
                Debug.Log("removing node");
                m_codeGraph.Connections.Remove(connection);
                m_connectionDictionary.Remove(e);
            }
        }
        private void RemoveNode(CodeGraphEditorNode editorNode)
        {
            m_codeGraph.Nodes.Remove(editorNode.Node);
            m_nodeDictionary.Remove(editorNode.Node.id);
            m_graphNodes.Remove(editorNode);
            m_serializedObject.Update();
        }
        private void DrawNodes()
        {
            foreach(CodeGraphNode node in m_codeGraph.Nodes)
            {
                AddNodeToGraph(node);
            }
            Bind();
        }
        private void DrawConnections()
        {
            if (m_codeGraph.Connections == null) return;
            foreach(CodeGraphConnection connection in m_codeGraph.Connections)
            {
                DrawConnection(connection);
            }
        }

        private void DrawConnection(CodeGraphConnection connection)
        {
            CodeGraphEditorNode inputNode = GetNode(connection.inputPort.nodeId);
            CodeGraphEditorNode outputNode = GetNode(connection.outputPort.nodeId);
            if(inputNode == null) return ;
            if(outputNode == null) return ;

            Port inPort = inputNode.Ports[connection.inputPort.portIndex];
            Port outPort = outputNode.Ports[connection.outputPort.portIndex];
            Edge edge = inPort.ConnectTo(outPort);
            AddElement(edge);

            m_connectionDictionary.Add(edge, connection);
        }

        private CodeGraphEditorNode GetNode(string nodeId)
        {
            CodeGraphEditorNode node = null;
            m_nodeDictionary.TryGetValue(nodeId, out node);
            return node;
        }

        private void ShowSearchWindow(NodeCreationContext obj)
        {
            Debug.Log("search window opened");
            m_searchProvider.target = (VisualElement)focusController.focusedElement;
            SearchWindow.Open(new SearchWindowContext(obj.screenMousePosition), m_searchProvider);
        }
        public void Add(CodeGraphNode node)
        {
            Undo.RecordObject(m_serializedObject.targetObject, "Added Node");

            m_codeGraph.Nodes.Add(node);
            m_serializedObject.Update();

            AddNodeToGraph(node);
            Bind();
        }

        private void AddNodeToGraph(CodeGraphNode node)
        {
            node.typeName = node.GetType().AssemblyQualifiedName;

            CodeGraphEditorNode editorNode = new CodeGraphEditorNode(node, m_serializedObject, this);
            editorNode.SetPosition(node.position);
            m_graphNodes.Add(editorNode);
            m_nodeDictionary.Add(node.id, editorNode);

            AddElement(editorNode);
        }

        private void Bind()
        {
            m_serializedObject.Update();
            this.Bind(m_serializedObject);
        }

        //custom right click menu
        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt) 
        {
            //do something like this (read up on documentation):
            //if(evt.target is GraphView)
            //{
            //    evt.menu.AppendAction("Add Variable")
            //}
            base.BuildContextualMenu(evt);
        }
    }
}
