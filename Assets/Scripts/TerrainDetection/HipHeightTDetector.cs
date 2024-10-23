using UnityEngine;

public class HipHeightTDetector : TerrainDetectorRay
{

    protected override void OnHitDetected(RaycastHit hitinfo)
    {
        Debug.Log(hitinfo.collider.gameObject);
        base.OnHitDetected(hitinfo);
    }
}
