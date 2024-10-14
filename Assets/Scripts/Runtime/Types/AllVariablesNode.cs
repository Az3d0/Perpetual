using UnityEngine;

namespace CodeGraph
{
    [NodeInfo("AllVariables", "Variable/All", 0, 0)]
    public class AllVariablesNode : CodeGraphNode
    {
        [ExposeVariablesFromGameObject()]
        public GameObject gameObject;

        //doesn't work. Without any non-dynamic ports, dynamic ports aren't added visually
    }
}
