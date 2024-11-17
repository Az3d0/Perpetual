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

    private Leg m_leftLeg;
    private Leg m_rightLeg;

    private void Awake()
    {
        m_inputActions = new InputSystem_Actions();
        m_inputActions.Enable();

        if (m_leftLeg == null && m_rightLeg == null)
        {
            m_leftLeg = new Leg(m_leftHip, m_inputActions.PlayerMovement.LeftLeg);
            m_rightLeg = new Leg(m_rightHip, m_inputActions.PlayerMovement.RightLeg);
        }
    }
    private void OnDestroy()
    {
        m_leftLeg.UnsubscribeFromInputAction();
        m_rightLeg.UnsubscribeFromInputAction();
    }
}
