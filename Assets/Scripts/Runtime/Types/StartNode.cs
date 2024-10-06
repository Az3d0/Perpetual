using UnityEngine;

namespace CodeGraph
{
    [NodeInfo("Start", "Process/Start", false, true)]
    public class StartNode : CodeGraphNode
    {
        public override string OnProcess(CodeGraphAsset currentGraph)
        {
            Debug.Log("Hello World. Start!");
            return base.OnProcess(currentGraph);
        }
    }
}
