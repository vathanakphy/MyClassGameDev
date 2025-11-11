using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-1)]
public class PlayerController : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private Camera _playerCamera;

    [Header("Base Movement")]
    public float runAcceleration = 8f;
    public float runSpeed = 3.6f;
    public float sprintSpeed = 6.5f;
    public float drag = 20f;

    [Header("Camera Settings")]
    public float lookSenseH = 3f;
    public float lookSenseV = 2f;
    public float lookLimitV = 80f;
    public float cameraDistance = 3f;
    public float cameraHeight = 1.5f;

    private PlayerLocalmotionInput _playerLocomotionInput;
    private Vector2 _cameraRotation = Vector2.zero;
    private Vector3 _currentVelocity;

    private void Awake()
    {
        _playerLocomotionInput = GetComponent<PlayerLocalmotionInput>();
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
    // --- Camera-based move direction ---
    Vector3 camForward = new Vector3(_playerCamera.transform.forward.x, 0, _playerCamera.transform.forward.z).normalized;
    Vector3 camRight = new Vector3(_playerCamera.transform.right.x, 0, _playerCamera.transform.right.z).normalized;
    Vector3 moveDir = camForward * _playerLocomotionInput.MovementInput.y + camRight * _playerLocomotionInput.MovementInput.x;
    moveDir.Normalize();

    // --- Determine if sprinting ---
    bool isSprinting = Input.GetKey(KeyCode.LeftShift);
    float targetSpeed = isSprinting ? sprintSpeed : runSpeed;

    // --- Smooth transition toward target velocity ---
    Vector3 targetVelocity = moveDir * targetSpeed;
    _currentVelocity = Vector3.Lerp(_currentVelocity, targetVelocity, Time.deltaTime * runAcceleration);

    // --- Apply movement ---
    _characterController.Move(_currentVelocity * Time.deltaTime);

    // --- Rotate character to face move direction ---
    if (moveDir.magnitude > 0.1f)
    {
        Quaternion lookRot = Quaternion.LookRotation(moveDir);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * 10f);
    }
}



    private void LateUpdate()
    {
        // Camera orbit
        _cameraRotation.x += _playerLocomotionInput.LookInput.x * lookSenseH;
        _cameraRotation.y = Mathf.Clamp(_cameraRotation.y - _playerLocomotionInput.LookInput.y * lookSenseV, -lookLimitV, lookLimitV);

        // Calculate camera position
        Quaternion camRot = Quaternion.Euler(_cameraRotation.y, _cameraRotation.x, 0f);
        Vector3 camOffset = camRot * new Vector3(0f, 0f, -cameraDistance);
        Vector3 targetPos = transform.position + Vector3.up * cameraHeight + camOffset;

        _playerCamera.transform.position = Vector3.Lerp(_playerCamera.transform.position, targetPos, Time.deltaTime * 10f);
        _playerCamera.transform.LookAt(transform.position + Vector3.up * cameraHeight);
    }
}
