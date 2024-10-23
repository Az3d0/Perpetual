using UnityEngine;

public static class IsoMath
{
    private static Matrix4x4 isoMatrix => Matrix4x4.Rotate(Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, 0));

    public static Vector3 ToIso(this Vector3 input) => isoMatrix.MultiplyPoint3x4(input);
}
