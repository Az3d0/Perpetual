using UnityEngine;

public class HipHeightTDetector : TerrainDetectorRay
{
    protected override void OnHitDetected(RaycastHit hitinfo)
    {
        RigController.ActiveRayHeightChanged.Invoke(RigController.ActiveRayHeightEnum.Hipheight);
        base.OnHitDetected(hitinfo);
    }
}
