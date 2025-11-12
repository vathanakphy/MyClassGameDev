using UnityEngine;

[DefaultExecutionOrder(1)]
public class PlayerAimController : MonoBehaviour
{
    [Header("References")]
    public PlayerController playerController; // Reference to your existing PlayerController
    public Camera playerCamera;
    public Animator animator;
    public Transform weaponSpawnPoint;

    [Header("Aim Settings")]
    public float normalFOV = 60f;
    public float aimFOV = 40f;
    public float fovSmooth = 10f;
    public float aimRange = 200f;

    [Header("Camera Offset (relative to player)")]
    public Vector3 normalOffset = new Vector3(0f, 1.6f, -3.5f);
    public Vector3 aimOffset = new Vector3(0.5f, 1.7f, -2.2f); // Right shoulder
    public float offsetSmooth = 8f;

    [Header("Rotation Smoothness")]
    public float aimRotSmooth = 12f;

    private bool isAiming;
    private Vector3 aimPoint;

    void Start()
    {
        if (!playerCamera) playerCamera = Camera.main;
        if (!playerController)
            playerController = FindObjectOfType<PlayerController>();
    }

    void Update()
    {
        isAiming = Input.GetKey(KeyCode.LeftControl);

        HandleFOV();
        HandleCameraOffset();
        HandleAimRaycast();
        HandleWeaponAlignment();
        SyncAnimator();
    }

    void HandleFOV()
    {
        float target = isAiming ? aimFOV : normalFOV;
        playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, target, Time.deltaTime * fovSmooth);
    }

    void HandleCameraOffset()
    {
        if (playerController == null) return;

        // Smooth transition between shoulder offsets
        Vector3 targetOffset = isAiming ? aimOffset : normalOffset;
        playerController.cameraDistance = Mathf.Lerp(
            playerController.cameraDistance,
            Mathf.Abs(targetOffset.z),
            Time.deltaTime * offsetSmooth);

        playerController.cameraHeight = Mathf.Lerp(
            playerController.cameraHeight,
            targetOffset.y,
            Time.deltaTime * offsetSmooth);
    }

    void HandleAimRaycast()
    {
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if (Physics.Raycast(ray, out RaycastHit hit, aimRange))
            aimPoint = hit.point;
        else
            aimPoint = ray.origin + ray.direction * aimRange;
    }

    void HandleWeaponAlignment()
    {
        if (!isAiming || weaponSpawnPoint == null) return;

        Vector3 dir = (aimPoint - weaponSpawnPoint.position).normalized;
        Quaternion targetRot = Quaternion.LookRotation(dir);
        weaponSpawnPoint.rotation = Quaternion.Slerp(weaponSpawnPoint.rotation, targetRot, Time.deltaTime * aimRotSmooth);
    }

    void SyncAnimator()
    {
        if (animator)
            animator.SetBool("isAiming", isAiming);
    }

    void OnDrawGizmosSelected()
    {
        if (!playerCamera) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f)));
    }
}
