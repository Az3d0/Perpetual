using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class FollowCamera : MonoBehaviour
{
    [SerializeField] 
    private GameObject m_target;

    [SerializeField]
    [Range(0f, 1f)]
    private float m_rotationSpeed = 0.1f;

    private InputSystem_Actions m_inputActions;
    private CharacterMovement m_characterMovement;
    private Vector3 m_cameraMovementDirection;


    //the position where the mouse was when rotation is enabled
    private Vector2? m_initialMousePosition;
    private Vector3 m_initialCameraRotation;
    private bool m_rotationEnabled;

    public bool RotationEnabled => m_rotationEnabled;
    private void Awake()
    {
        m_target.TryGetComponent<CharacterMovement>(out m_characterMovement);
        SubscribeToInputActions();
        m_initialMousePosition = null;
    }

    private void Update()
    {
        if(m_characterMovement != null)
        {
            transform.position += m_characterMovement.DeltaPlayerMovement;
        }
    }
    private void SubscribeToInputActions()
    {
        m_inputActions = new InputSystem_Actions();
        m_inputActions.Enable();

        m_inputActions.Camera.EnableRotation.started += OnEnableRotationAction;
        m_inputActions.Camera.EnableRotation.canceled += OnEnableRotationAction;
    }

    private void OnRotateCameraAction(InputAction.CallbackContext context)
    {
        DragCamera(context.ReadValue<Vector2>());
    }

    private void OnEnableRotationAction(InputAction.CallbackContext context)
    {
        m_rotationEnabled = context.ReadValueAsButton();
        if (m_rotationEnabled)
        {
            m_inputActions.Camera.RotateCamera.performed += OnRotateCameraAction;
        }
        else
        {
            m_initialMousePosition = null;
            m_inputActions.Camera.RotateCamera.performed -= OnRotateCameraAction;
        }
    }

    private void DragCamera(Vector2 value)
    {
        if (m_initialMousePosition == null)
        {
            m_initialMousePosition = value;
            m_initialCameraRotation = transform.eulerAngles;
        }

        float disposition = value.x - m_initialMousePosition.Value.x;
        Vector3 eulerRotation = transform.eulerAngles;
        eulerRotation.y = m_initialCameraRotation.y + disposition * m_rotationSpeed * 0.01f;
        transform.eulerAngles = eulerRotation;
    }
}
