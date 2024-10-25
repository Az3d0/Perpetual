using System;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;


public class TerrainDetectorRay : MonoBehaviour
{
    private Ray m_ray;
    [SerializeField] private RayDirectionEnum m_rayDirection = RayDirectionEnum.Forward;
    [SerializeField] private float m_rayLength = 0.5f;

    
    public Vector3 RayDirectionVector;
    public Action<RaycastHit> TerrainDetected;

    [HideInInspector]
    public RigController RigController;

    public Ray Ray => m_ray;
    public RayDirectionEnum RayDirection => m_rayDirection;

    private void OnValidate()
    {
        Debug.Log(m_rayDirection.ToString());
        switch (m_rayDirection)
        {
            case RayDirectionEnum.Forward:
                RayDirectionVector = Vector3.forward; break;
            case RayDirectionEnum.Downward:
                RayDirectionVector = Vector3.down; break;
            case RayDirectionEnum.Upward:
                RayDirectionVector = Vector3.up; break;
        }
    }
    protected void OnEnable()
    {
        //won't be needing unsubscribe since the action is generated by the same script
        TerrainDetected += OnHitDetected;
    }
    protected virtual void Update()
    {
        m_ray = new Ray(transform.position, RayDirectionVector);

        if (Physics.Raycast(m_ray, out RaycastHit hitInfo, m_rayLength))
        {
            if(hitInfo.collider != null)
            {
                TerrainDetected.Invoke(hitInfo);
            }
        }

        if (RigController.DebugMode)
        {
            EnableDebugMode(m_ray);
        }

    }

    public virtual void EnableDebugMode(Ray ray)
    {
        Debug.DrawRay(ray.origin, ray.direction, Color.red);
    }

    private void ResetRigControllerState()
    {
        if(RigController.ActiveRayHeight != RigController.ActiveRayHeightEnum.None)
        {
            RigController.ActiveRayHeightChanged.Invoke(RigController.ActiveRayHeightEnum.None);
        }
    }
    protected virtual void OnHitDetected(RaycastHit hitinfo)
    {
    }
}

public enum RayDirectionEnum
{
    Forward,
    Downward,
    Upward
}