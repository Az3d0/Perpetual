using UnityEngine;

namespace CodeGraph
{
    [NodeInfo("Bool", "Variable/Bool", 0, 0)]

    public class BoolNode : CodeGraphNode
    {
        [ExposeFieldsFromScript]
        public GameObject script;

        [ExposedProperty]
        [ExposedOutputPortProperty(typeof(bool), "value", "value of bool")]
        public bool isTrue;
    }
}
