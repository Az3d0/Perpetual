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
    /// Change this value to adjust the range of input values considered as the foot being grounded. 0 means only a fully held trigger will count as a grounded foot.
    /// </summary>
    [SerializeField]
    private float m_footGroundedTreshHold = 0;

    private Leg m_leftLeg;
    private Leg m_rightLeg;

    private Vector2 m_movementDirection;

    private void Awake()
    {
        m_inputActions = new InputSystem_Actions();
        m_inputActions.Enable();

        if (m_leftLeg == null && m_rightLeg == null)
        {
            m_leftLeg = new Leg(m_leftHip, m_inputActions.PlayerMovement.LeftLeg, m_footGroundedTreshHold);
            m_rightLeg = new Leg(m_rightHip, m_inputActions.PlayerMovement.RightLeg, m_footGroundedTreshHold);
        }

        m_inputActions.PlayerMovement.MovementDirection.performed += OnDirectionIndicated;
    }

    private void OnDirectionIndicated(InputAction.CallbackContext context)
    {
        m_movementDirection = context.ReadValue<Vector2>();
    }

    private void OnDrawGizmos()
    {
        if (m_leftLeg != null && m_rightLeg != null)
        {
            Gizmos.DrawSphere(m_leftLeg.TargetFootLocalPosition, 0.1f);
            //Gizmos.DrawSphere(m_leftLeg.TargetKneeLocalPosition, 0.1f);

            Gizmos.DrawSphere(m_rightLeg.TargetFootLocalPosition, 0.1f);
            //Gizmos.DrawSphere(m_rightLeg.TargetKneeLocalPosition, 0.1f);
        }

    }
    private void OnDestroy()
    {
        m_leftLeg.UnsubscribeFromInputAction();
        m_rightLeg.UnsubscribeFromInputAction();
    }
}
