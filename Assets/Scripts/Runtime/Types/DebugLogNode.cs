using UnityEngine;

namespace CodeGraph
{
    [NodeInfo("Debug Log", "Debug/Debug Log Console")]
    public class DebugLogNode : CodeGraphNode
    {
        [ExposedProperty()]
        public string logMessage;
        public override string OnProcess(CodeGraphAsset currentGraph)
        {
            Debug.Log(logMessage);
            return base.OnProcess(currentGraph);
        }
    }
}
