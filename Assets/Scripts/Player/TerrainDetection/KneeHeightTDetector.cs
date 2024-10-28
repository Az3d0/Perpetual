using UnityEditor.PackageManager;
using UnityEngine;

public class KneeHeightTDetector : TerrainDetectorRay
{
    [SerializeField] 
    AvatarIKGoal controlledFoot;

    protected override void OnHitDetected(RaycastHit hitinfo)
    {
        RigController.FootIK(controlledFoot, hitinfo);
        base.OnHitDetected(hitinfo);
    }

    private void OnAnimatorIK(int layerIndex)
    {
        Animator.SetIKPosition(controlledFoot, Hitinfo.point);
    }
}
