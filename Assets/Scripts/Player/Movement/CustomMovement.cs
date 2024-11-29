using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CustomMovement : MonoBehaviour
{

    private InputSystem_Actions m_inputActions;

    [SerializeField] 
    private GameObject m_leftHip;

    [SerializeField] 
    private GameObject m_rightHip;

    /// <summary>
    /// Adjusts the leeway for the input the game accepts as the leg being fully extended. 0 means only a fully held trigger will count as a grounded foot.
    /// </summary>
    [SerializeField]
    [Range(0,1)]
    private float m_triggerDeadZone = 0;

    [SerializeField]
    private float m_footMovementRange = .45f;

    [SerializeField]
    private float m_footGroundedTreshhold = 0.1f;
    private Leg m_leftLeg;
    private Leg m_rightLeg;

    private Vector3 m_movementDirection;
    private float m_verticalDirection = 0;

    public float FootGroundedTreshhold => m_footGroundedTreshhold;
    public float FootMovementRange => m_footMovementRange;
    public Vector3 MovementDirection => m_movementDirection;
    public float TriggerDeadZone => m_triggerDeadZone;

    private void Awake()
    {
        m_inputActions = new InputSystem_Actions();
        m_inputActions.Enable();

        if (m_leftLeg == null && m_rightLeg == null)
        {
            m_leftLeg = new Leg(m_leftHip, m_inputActions.PlayerMovement.LeftLeg, this);
            m_rightLeg = new Leg(m_rightHip, m_inputActions.PlayerMovement.RightLeg, this);
        }

        m_inputActions.PlayerMovement.MovementDirection.performed += OnDirectionIndicated;
    }

    private void Update()
    {
        m_leftLeg.UpdateLegValues();
        m_rightLeg.UpdateLegValues();

        if (m_leftLeg.IsFootGrounded())
        {
            m_leftLeg.GroundFoot();
        }
        if (m_rightLeg.IsFootGrounded())
        {
            m_rightLeg.GroundFoot(); 
        }
    }
    private void OnDirectionIndicated(InputAction.CallbackContext context)
    {
        Vector2 horizontalDirection = context.ReadValue<Vector2>();
        m_movementDirection = new Vector3(horizontalDirection.x, m_verticalDirection, horizontalDirection.y);

        Debug.DrawRay(gameObject.transform.position, m_movementDirection, Color.red);

        m_leftLeg.MoveFoot();
        m_rightLeg.MoveFoot();
    }

    private void OnDrawGizmos()
    {
        if (m_leftLeg != null && m_rightLeg != null)
        {
            m_leftLeg.GizmoDebugLeg();
            m_rightLeg.GizmoDebugLeg();
        }

    }
    private void OnDestroy()
    {
        m_leftLeg.UnsubscribeFromInputAction();
        m_rightLeg.UnsubscribeFromInputAction();
    }
}
