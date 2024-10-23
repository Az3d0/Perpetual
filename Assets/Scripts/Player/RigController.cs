using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class RigController : MonoBehaviour
{

    [SerializeField]
    private List<TerrainDetectorRay> m_TerrainDetectorRays;

    private CharacterMovement m_characterMovement;

    private void Awake()
    {
        TryGetComponent<CharacterMovement>(out m_characterMovement);
        if (m_characterMovement == null)
        {
            Debug.LogWarning("RigController cannot find CharacterMovementScript. Check if it is attached to main player object");
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
                    ray.RayForwardVector = m_characterMovement.PlayerMovementDirection;
                }
            }
        } 
    }
}
