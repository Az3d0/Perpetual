using System;
using UnityEngine;

namespace CodeGraph
{
    ///<summary>
    /// Class <c>CodeGraphNode</c> is the base class of individual nodes
    /// </summary>
    [System.Serializable]
    public class CodeGraphNode
    {
        [SerializeField]
        private string m_guid;
        [SerializeField]
        private Rect m_position;

        public string typeName;

        public string id => m_guid;
        public Rect position => m_position;

        public CodeGraphNode()
        {
            NewGUID();
        }

        private void NewGUID()
        {
            m_guid = Guid.NewGuid().ToString();
        }

        public void SetPosition(Rect position)
        {
            m_position = position;
        }

        public virtual string OnProcess(CodeGraphAsset currentGraph)
        {
            CodeGraphNode nextNodeInFlow = currentGraph.GetNodeFromOutput(m_guid, 0);
            if (nextNodeInFlow != null)
            {
                return nextNodeInFlow.id;
            }
            return string.Empty;
        }
    }
}
