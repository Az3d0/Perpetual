using UnityEngine;
using UnityEngine.Events;

namespace CodeGraph
{
    [NodeInfo("InvokeUnityEvent", "Events/InvokeUnityEvent")]
    public class InvokeUnityEventNode : CodeGraphNode
    {

        [ExposedProperty]
        public UnityEvent untityEvent;
    }
}
