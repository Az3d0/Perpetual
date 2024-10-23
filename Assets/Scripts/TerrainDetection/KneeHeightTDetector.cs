using UnityEngine;

public class KneeHeightTDetector : TerrainDetectorRay
{
    protected override void OnHitDetected(RaycastHit hitinfo)
    {
        Debug.Log(hitinfo.collider.gameObject);
        base.OnHitDetected(hitinfo);
    }
}
