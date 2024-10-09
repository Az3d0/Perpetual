using UnityEngine;

namespace CodeGraph
{
    [NodeInfo("If", "Logic/If", 1, 2)]
    public class IfNode : CodeGraphNode
    {
        // create property that takes input node
        [ExposedProperty()]
        public bool isTrue;
        public override string OnProcess(CodeGraphAsset currentGraph, int outPortIndex = 0)
        {
            if (isTrue)
            {
                return base.OnProcess(currentGraph, 0);
            }
            else
            {
                return base.OnProcess(currentGraph, 1);
            }
        }
    }
}
