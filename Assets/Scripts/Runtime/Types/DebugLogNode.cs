using UnityEngine;

namespace CodeGraph
{
    [NodeInfo("Debug Log", "Debug/Debug Log Console")]
    public class DebugLogNode : CodeGraphNode
    {
        [ExposedProperty()]
        [ExposedInputPortProperty(typeof(string), "string", "string")]
        public string logMessage;
        public override string OnProcess(CodeGraphAsset currentGraph, int outPortIndex = 0)
        {
            Debug.Log(logMessage);
            return base.OnProcess(currentGraph);
        }
    }
}
