using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class RigController : MonoBehaviour
{

    [SerializeField]
    private List<TerrainDetectorRay> m_TerrainDetectorRays;
    private List<LimbIKControllerRay> m_IKControllerRays;

    [SerializeField] 
    bool m_debugMode;

    private Animator m_animator;
    [HideInInspector]
    public ActiveRayHeightEnum ActiveRayHeight = ActiveRayHeightEnum.None;
    public Action<ActiveRayHeightEnum> ActiveRayHeightChanged;

    public bool DebugMode => m_debugMode;
    private CharacterMovement m_characterMovement;

    private void Awake()
    {
        ActiveRayHeightChanged += OnActiveRayHeightChanged;

        m_IKControllerRays = new List<LimbIKControllerRay>();
        m_animator = GetComponent<Animator>();
        TryGetComponent<CharacterMovement>(out m_characterMovement);
        if (m_characterMovement == null)
        {
            Debug.LogWarning("RigController cannot find CharacterMovementScript. Check if it is attached to main player object");
        }

        foreach(TerrainDetectorRay ray in m_TerrainDetectorRays)
        {
            ray.RigController = this;

            Type type = ray.GetType();

            if(type == typeof(LimbIKControllerRay))
            {
                Debug.Log(ray.name);
                m_IKControllerRays.Add((LimbIKControllerRay)ray);
            }
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

    private void OnAnimatorIK(int layerIndex)
    {
        if (!m_animator) return;

        foreach(LimbIKControllerRay ray in m_IKControllerRays)
        {
            ray.HandleLimbIK(m_animator);
        }
    }

    public enum ActiveRayHeightEnum
    {
        None,
        KneeHeight,
        Hipheight
    }

}
