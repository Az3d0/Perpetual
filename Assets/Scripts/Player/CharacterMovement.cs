using JetBrains.Annotations;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class CharacterMovement : MonoBehaviour
{
    private InputSystem_Actions m_inputActions;
    private CharacterController m_characterController;
    private Animator m_animator;
    private Ray m_slopeCheckRay;

    private Vector3 m_playerMovementDirection;
    private Vector3 m_lastPlayerPosition;
    private float m_slopeAngle;

    private bool m_isMoving;

    [SerializeField] private float m_walkSpeed = 1;
    [SerializeField] private float m_rotationSpeed = 1;

    public Vector3 PlayerMovementDirection => m_playerMovementDirection;

    [HideInInspector] 
    public Vector3 DeltaPlayerMovement;
    public float WalkSpeed => m_walkSpeed;
    public float RotationSpeed => m_rotationSpeed;
    public bool IsMoving => m_isMoving;
    private void Awake()
    {
        m_characterController = GetComponent<CharacterController>();
        m_animator = GetComponent<Animator>();
        m_lastPlayerPosition = transform.position;

        SubscribeToInputActions();
    }
    private void Update()
    {
        m_slopeAngle = UpdateSlopeAngle();

        m_characterController.Move(AdjustedVelocityToSlope(m_playerMovementDirection) * m_walkSpeed * Time.deltaTime);

        DeltaPlayerMovement = transform.position - m_lastPlayerPosition;
        m_lastPlayerPosition = transform.position;

        HandleRotation();
        HandleGravity();

    }
    public float UpdateSlopeAngle()
    {
        m_slopeCheckRay = new Ray(transform.position, Vector3.down);
        if (Physics.Raycast(m_slopeCheckRay, out RaycastHit hitInfo, m_characterController.height / 2 + 0.2f))
        {
            Debug.DrawRay(hitInfo.point, hitInfo.normal, Color.yellow);
            return Mathf.Atan(Mathf.Sqrt(Mathf.Pow(hitInfo.normal.x, 2) + Mathf.Pow(hitInfo.normal.z, 2)) / hitInfo.normal.y) * Mathf.Rad2Deg;
        }
        return 180;
    }

    private void SubscribeToInputActions()
    {
        m_inputActions = new InputSystem_Actions();
        m_inputActions.Enable();

        m_inputActions.Player.Move.performed += OnMoveAction;
        m_inputActions.Player.Move.canceled += OnMoveAction;
        m_inputActions.Player.Jump.started += OnJumpAction;
    }

    private void OnJumpAction(InputAction.CallbackContext context)
    {
        bool isJumping = context.ReadValueAsButton();
    }

    private void OnMoveAction(InputAction.CallbackContext context)
    {
        HandleMovement(context.ReadValue<Vector2>());
    }

    //change "handler" naming?
    private void HandleMovement(Vector2 movementInput)
    {
        m_playerMovementDirection = new Vector3(movementInput.x, 0, movementInput.y).ToIso();

        m_isMoving = movementInput.x != 0 || movementInput.y != 0;
        m_animator.SetBool("isWalking", IsMoving);
    }
    private void HandleRotation()
    {
        Quaternion currentRotation = transform.rotation;

        if(m_isMoving)
        {
            Quaternion targetRotation = Quaternion.LookRotation(m_playerMovementDirection);

            transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, m_rotationSpeed * Time.deltaTime);
        }
    }

    private void HandleGravity()
    {
        float gravity;
        if(m_characterController.isGrounded)
        {
            gravity = -.05f;
            m_playerMovementDirection.y = gravity;
        }
        else
        {
            gravity = -0.1f;
            m_playerMovementDirection.y += gravity;
        }
    }


    private Vector3 AdjustedVelocityToSlope(Vector3 velocity)
    {
        if(Physics.Raycast(m_slopeCheckRay, out RaycastHit hitInfo, m_characterController.height / 2 + 0.2f))
        {
            var slopeRotation = Quaternion.FromToRotation(Vector3.up, hitInfo.normal);
            var adjustedVelocity = slopeRotation * velocity;

            Debug.DrawRay(hitInfo.point, adjustedVelocity, Color.red);
            if (adjustedVelocity.y < 0)
                 return adjustedVelocity; 
        }
        return velocity;
    }
}
