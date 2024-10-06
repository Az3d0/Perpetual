using System.Diagnostics;
using UnityEngine;

namespace CodeGraph
{
    [NodeInfo("Move Self", "Transform/Move Self")]
    public class MoveNode : CodeGraphNode
    {

        [ExposedProperty()]
        public Vector3 direction;
        public override string OnProcess(CodeGraphAsset currentGraph)
        {
            currentGraph.gameObject.transform.position += direction;
            return base.OnProcess(currentGraph);
        }
    }
}
