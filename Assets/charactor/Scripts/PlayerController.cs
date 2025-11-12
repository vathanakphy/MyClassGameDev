using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-1)]
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private Camera _playerCamera;
    [SerializeField] private Animator _animator;

    [Header("Base Movement")]
    public float runAcceleration = 8f;
    public float runSpeed = 3.6f;
    public float sprintSpeed = 6.5f;
    public float gravity = -9.81f;
    public float groundCheckDistance = 0.3f;

    [Header("Camera Settings")]
    public float lookSenseH = 3f;
    public float lookSenseV = 2f;
    public float lookLimitV = 80f;
    public float cameraDistance = 3f;
    public float cameraHeight = 1.5f;

    private PlayerLocalmotionInput _playerLocomotionInput;
    private Vector2 _cameraRotation = Vector2.zero;
    private Vector3 _currentVelocity;
    private Vector3 _verticalVelocity;
    private bool _isSprinting;
    private bool _isGrounded;

    private void Awake()
    {
        _playerLocomotionInput = GetComponent<PlayerLocalmotionInput>();

        if (_characterController == null)
            _characterController = GetComponent<CharacterController>();
        if (_animator == null)
            _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        if (_characterController == null)
            Debug.LogError("CharacterController not assigned!");
        if (_playerCamera == null)
            Debug.LogError("Player Camera not assigned!");
        if (_playerLocomotionInput == null)
            Debug.LogError("PlayerLocomotionInput missing!");
    }

    private void Update()
    {
        HandleMovement();
        UpdateAnimator();
    }

    private void HandleMovement()
    {
        // --- Check Ground ---
        _isGrounded = Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, groundCheckDistance);

        // --- Camera-based Direction ---
        Vector3 camForward = new Vector3(_playerCamera.transform.forward.x, 0, _playerCamera.transform.forward.z).normalized;
        Vector3 camRight = new Vector3(_playerCamera.transform.right.x, 0, _playerCamera.transform.right.z).normalized;
        Vector3 moveDir = camForward * _playerLocomotionInput.MovementInput.y + camRight * _playerLocomotionInput.MovementInput.x;
        moveDir.Normalize();

        // --- Sprinting ---
        _isSprinting = (_playerLocomotionInput.IsSprinting || Input.GetKey(KeyCode.LeftShift)) && moveDir.magnitude > 0.1f;
        float targetSpeed = _isSprinting ? sprintSpeed : runSpeed;

        // --- Smooth Horizontal Velocity ---
        Vector3 targetVelocity = moveDir * targetSpeed;
        _currentVelocity = Vector3.Lerp(_currentVelocity, targetVelocity, Time.deltaTime * runAcceleration);

        // --- Gravity ---
        if (_isGrounded)
        {
            if (_verticalVelocity.y < -2f)
                _verticalVelocity.y = -2f; // small push down to keep grounded
        }
        else
        {
            _verticalVelocity.y += gravity * Time.deltaTime;
        }

        // --- Combine Velocity ---
        Vector3 finalVelocity = _currentVelocity + _verticalVelocity;

        _characterController.Move(finalVelocity * Time.deltaTime);

        // --- Rotation ---
        if (moveDir.magnitude > 0.1f)
        {
            Quaternion lookRot = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * 10f);
        }
    }

    private void UpdateAnimator()
    {
        if (_animator == null) return;

        float moveAmount = new Vector2(_playerLocomotionInput.MovementInput.x, _playerLocomotionInput.MovementInput.y).magnitude;
        bool isMoving = moveAmount > 0.1f;

        _animator.SetBool("IsMoving", isMoving);
        _animator.SetBool("IsRunning", _isSprinting);

        // Use blend tree smoothly between idle/walk/run
        float animSpeed = Mathf.Lerp(0f, _isSprinting ? 2f : 1f, moveAmount);
        _animator.SetFloat("Speed", animSpeed, 0.1f, Time.deltaTime);
    }

    private void LateUpdate()
    {
        // --- Camera Orbit ---
        _cameraRotation.x += _playerLocomotionInput.LookInput.x * lookSenseH;
        _cameraRotation.y = Mathf.Clamp(_cameraRotation.y - _playerLocomotionInput.LookInput.y * lookSenseV, -lookLimitV, lookLimitV);

        Quaternion camRot = Quaternion.Euler(_cameraRotation.y, _cameraRotation.x, 0f);
        Vector3 camOffset = camRot * new Vector3(0f, 0f, -cameraDistance);
        Vector3 targetPos = transform.position + Vector3.up * cameraHeight + camOffset;

        _playerCamera.transform.position = Vector3.Lerp(_playerCamera.transform.position, targetPos, Time.deltaTime * 10f);
        _playerCamera.transform.LookAt(transform.position + Vector3.up * cameraHeight);
    }
}
