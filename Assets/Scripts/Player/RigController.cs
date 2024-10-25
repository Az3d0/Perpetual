using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class RigController : MonoBehaviour
{

    [SerializeField]
    private List<TerrainDetectorRay> m_TerrainDetectorRays;

    [SerializeField] 
    bool m_debugMode;
    [HideInInspector]
    public ActiveRayHeightEnum ActiveRayHeight = ActiveRayHeightEnum.None;
    public Action<ActiveRayHeightEnum> ActiveRayHeightChanged;

    public bool DebugMode => m_debugMode;
    private CharacterMovement m_characterMovement;

    private void Awake()
    {
        ActiveRayHeightChanged += OnActiveRayHeightChanged;
        TryGetComponent<CharacterMovement>(out m_characterMovement);
        if (m_characterMovement == null)
        {
            Debug.LogWarning("RigController cannot find CharacterMovementScript. Check if it is attached to main player object");
        }

        foreach(TerrainDetectorRay ray in m_TerrainDetectorRays)
        {
            ray.RigController = this;
        }
    }
    void Update()
    {
        if(m_characterMovement != null)
        {
            if(m_characterMovement.IsMoving)
            {
                foreach (TerrainDetectorRay ray in m_TerrainDetectorRays)
                {
                    if (ray.RayDirection == RayDirectionEnum.Downward) break;
                    ray.RayDirectionVector = m_characterMovement.PlayerMovementDirection;
                }
            }
        } 
    }


    private void OnDrawGizmosSelected()
    {
        foreach (TerrainDetectorRay ray in m_TerrainDetectorRays)
        {
            if (DebugMode)
            {
                ray.EnableDebugMode(new Ray(ray.transform.position, ray.RayDirectionVector));
            }
        }
    }
    private void OnActiveRayHeightChanged(ActiveRayHeightEnum activeRayHeight)
    {
        if (activeRayHeight != ActiveRayHeight)
        {
            ActiveRayHeight = activeRayHeight;
            Debug.Log(ActiveRayHeight);
        }
    }
    public enum ActiveRayHeightEnum
    {
        None,
        KneeHeight,
        Hipheight
    }
}
