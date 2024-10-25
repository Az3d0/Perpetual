using UnityEngine;

public class KneeHeightTDetector : TerrainDetectorRay
{
    protected override void OnHitDetected(RaycastHit hitinfo)
    {
        if (RigController.ActiveRayHeight != RigController.ActiveRayHeightEnum.Hipheight)
        {
            RigController.ActiveRayHeightChanged.Invoke(RigController.ActiveRayHeightEnum.KneeHeight);
        }
        
        base.OnHitDetected(hitinfo);
    }
}
