using UnityEditor.Animations;
using UnityEditor.SceneManagement;
using UnityEditor.SearchService;
using UnityEngine;

namespace CodeGraph
{
    [NodeInfo("Bool", "Variable/Bool", 0, 0)]

    public class BoolNode : CodeGraphNode
    {
        [ExposeVariablesFromGameObject(typeof(bool))]
        public GameObject gameObject;

        [ExposedProperty]
        [ExposedOutputPortProperty(typeof(bool), "manualBool", "toggle bool for manual testing")]
        public bool isTrue;
    }
}
