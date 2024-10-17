using Unity.VisualScripting;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [SerializeField] private GameObject m_target;
    private CharacterMovement m_characterMovement;
    private Vector3 m_cameraMovementDirection;

    private void Awake()
    {
        m_target.TryGetComponent<CharacterMovement>(out m_characterMovement);

    }

    private void Update()
    {
        if(m_characterMovement != null)
        {
            transform.position += m_characterMovement.DeltaPlayerMovement;
        }
    }
}
