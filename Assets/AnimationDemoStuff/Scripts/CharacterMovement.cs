using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class CharacterMovement : MonoBehaviour
{
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference jumpAction;
    private CharacterController m_characterController;

    private void Awake()
    {
        m_characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        Vector2 moveInput = moveAction.ToInputAction().ReadValue<Vector2>();

        bool jumpInput = jumpAction.ToInputAction().IsPressed();
    }


}
