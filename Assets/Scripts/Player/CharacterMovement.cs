using System;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class CharacterMovement : MonoBehaviour
{
    private InputSystem_Actions m_inputActions;
    private CharacterController m_characterController;
    private Animator m_animator;

    private Vector3 m_playerMovementDirection;
    private Vector3 m_lastPlayerPosition;
    private bool m_isMoving;

    [SerializeField] private float m_walkSpeed = 1;
    [SerializeField] private float m_rotationSpeed = 1;

    public Vector3 PlayerMovementDirection => m_playerMovementDirection;
    public Vector3 DeltaPlayerMovement;
    public float WalkSpeed => m_walkSpeed;
    public float RotationSpeed => m_rotationSpeed;
    public bool IsMoving => m_isMoving;
    private void Awake()
    {
        m_characterController = GetComponent<CharacterController>();
        m_animator = GetComponent<Animator>();
        m_lastPlayerPosition = transform.position;

        SubscribeToEventActions();
    }

    private void Update()
    {
        m_characterController.Move(AdjustedVelocityToSlope(m_playerMovementDirection) * m_walkSpeed * Time.deltaTime);

        DeltaPlayerMovement = transform.position - m_lastPlayerPosition;
        m_lastPlayerPosition = transform.position;

        HandleRotation();
        HandleGravity();
    }

    private void SubscribeToEventActions()
    {
        m_inputActions = new InputSystem_Actions();
        m_inputActions.Enable();

        m_inputActions.Player.Move.performed += OnMoveActionPerformed;
        m_inputActions.Player.Move.canceled += OnMoveActionPerformed;
        m_inputActions.Player.Jump.started += OnJumpActionStarted;

    }

    private void OnJumpActionStarted(InputAction.CallbackContext context)
    {
        bool isJumping = context.ReadValueAsButton();
    }

    private void OnMoveActionPerformed(InputAction.CallbackContext context)
    {

        HandleMovement(context.ReadValue<Vector2>());
    }

    private void HandleMovement(Vector2 movementInput)
    {
        m_playerMovementDirection = new Vector3(movementInput.x, 0, movementInput.y);
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
        var ray = new Ray(transform.position, Vector3.down);

        if(Physics.Raycast(ray, out RaycastHit hitInfo, m_characterController.height / 2 + 0.2f))
        {
            var slopeRotation = Quaternion.FromToRotation(Vector3.up, hitInfo.normal);
            var adjustedVelocity = slopeRotation * velocity;

            if (adjustedVelocity.y < 0)
            { return adjustedVelocity; }
        }
        return velocity;
    }
}
