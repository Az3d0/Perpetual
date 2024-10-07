using Unity.VisualScripting;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [SerializeField] Transform target;

    private void Update()
    {
        transform.LookAt(target.position);
    }
}
