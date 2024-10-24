using UnityEngine;

namespace CodeGraph
{
    [NodeInfo("Start", "Process/Start", 0, 1)]
    public class StartNode : CodeGraphNode
    {
        public override string OnProcess(CodeGraphAsset currentGraph, int outPortIndex = 0)
        {
            return base.OnProcess(currentGraph);
        }
    }
}
