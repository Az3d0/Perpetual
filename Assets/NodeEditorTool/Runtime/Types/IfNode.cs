using System;
using UnityEngine;

namespace CodeGraph
{
    [NodeInfo("If", "Logic/If", 1, 2)]
    public class IfNode : CodeGraphNode
    {
        [ExposedProperty()]
        [ExposedInputPortProperty(typeof(bool), "Condition", "If statement")]
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
