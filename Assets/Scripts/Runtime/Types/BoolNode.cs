using UnityEngine;

namespace CodeGraph
{
    [NodeInfo("Bool", "Variable/Bool", 0, 0)]

    public class BoolNode : CodeGraphNode
    {

        [ExposedProperty]
        [ExposedOutputPortProperty(typeof(bool), "value", "value of bool")]
        public bool isTrue;
    }
}
