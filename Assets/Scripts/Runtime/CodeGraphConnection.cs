using UnityEditor;
using UnityEngine;

namespace CodeGraph
{
    [System.Serializable]
    public struct CodeGraphConnection
    {
        public CodeGraphConnectionPort inputPort;
        public CodeGraphConnectionPort outputPort;


        public CodeGraphConnection(CodeGraphConnectionPort input, CodeGraphConnectionPort output)
        {
            inputPort = input;
            outputPort = output;
        }

        public CodeGraphConnection(string inputPortId, int inputIndex, string outputPortId, int outputIndex)
        {
            inputPort = new CodeGraphConnectionPort(inputPortId, inputIndex);
            outputPort = new CodeGraphConnectionPort(outputPortId, outputIndex);
        }
    }

    [System.Serializable]
    public struct CodeGraphConnectionPort
    {
        public string nodeId;
        public int portIndex;

        public CodeGraphConnectionPort(string id, int index)
        {
            this.nodeId = id;
            this.portIndex = index;
        }
    }
}
