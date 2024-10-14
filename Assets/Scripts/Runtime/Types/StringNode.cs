using UnityEngine;

namespace CodeGraph
{
    [NodeInfo("String", "Variable/String", 0, 0)]
    public class StringNode : CodeGraphNode
    {
        [ExposeVariablesFromGameObject(typeof(string))]
        public GameObject gameObject;

        [ExposedProperty]
        [ExposedOutputPortProperty(typeof(string), "manualString", "string for manual testing")]
        public string text;
    }
}
