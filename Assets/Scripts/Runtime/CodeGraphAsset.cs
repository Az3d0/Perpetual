using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CodeGraph
{
    ///<summary>
    /// Class <c>CodeGraphAsset</c> is a scriptable object
    /// </summary>
    [CreateAssetMenu(menuName = "Code Graph/New Graph")]
    public class CodeGraphAsset : ScriptableObject
    {
        [SerializeReference]
        private List<CodeGraphNode> m_nodes;
        [SerializeField]
        private List<CodeGraphConnection> m_connections;

        public List<CodeGraphNode> Nodes => m_nodes;
        public List<CodeGraphConnection> Connections => m_connections;

        private Dictionary<string, CodeGraphNode> m_NodeDictionary;

        public GameObject gameObject;
        public CodeGraphAsset()
        {
            m_nodes = new List<CodeGraphNode>();
            m_connections = new List<CodeGraphConnection>();
        }

        /// <summary>
        /// Adds each node in the graph to an m_NodeDictionary
        /// </summary>
        /// <param name="gameObject"></param>
        public void Init(GameObject gameObject)
        {
            this.gameObject = gameObject;
            m_NodeDictionary = new Dictionary<string, CodeGraphNode>();

            //Replace Nodes with m_nodes?
            foreach(CodeGraphNode node in Nodes)
            {
                m_NodeDictionary.Add(node.id, node);
            }
        }
        public CodeGraphNode GetStartNode()
        {
            StartNode[] startNodes = Nodes.OfType<StartNode>().ToArray();
            if (startNodes.Length == 0) 
            {
                Debug.LogError("There is no start node in this graph");
                return null;
            }
            return startNodes[0];
        }

        /// <summary>
        /// gets relevant node based on passed nextNodeId parameter
        /// </summary>
        /// <param name="nextNodeId"></param>
        /// <returns></returns>
        public CodeGraphNode GetNode(string nextNodeId)
        {
            if(m_NodeDictionary.TryGetValue(nextNodeId, out CodeGraphNode node))
            {
                return node;
            }
            return null;
        }

        public CodeGraphNode GetNodeFromOutput(string outputNodeId, int index)
        {
            foreach(CodeGraphConnection connection in m_connections)
            {
                if(connection.outputPort.nodeId == outputNodeId && connection.outputPort.portIndex == index)
                {
                    string nodeID = connection.inputPort.nodeId;
                    CodeGraphNode inputNode = m_NodeDictionary[nodeID];
                    return inputNode;
                }
            }

            return null;
        }
    }
}

