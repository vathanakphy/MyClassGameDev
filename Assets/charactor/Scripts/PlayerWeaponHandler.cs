using UnityEngine;

public class PlayerWeaponHandler : MonoBehaviour
{
    [Header("Weapon Setup")]
    [SerializeField] private Transform handTransform;

    [SerializeField] private Vector3 handOffsetPosition;
    [SerializeField] private Vector3 handOffsetRotation;

    private GameObject currentWeapon;
    private Rigidbody currentRb;

    [Header("Throw Settings")]
    public float throwForce = 50f;
    public float throwSpin = 300f;

    private PlayerLocalmotionInput input;
    private Camera cam;

    private void Awake()
    {
        input = GetComponent<PlayerLocalmotionInput>();
        cam = Camera.main;
    }

    private void Update()
    {
        // Check throw input
        if (currentWeapon != null && input.ThrowModifierHeld && input.ThrowPressed)
        {
            ThrowWeapon();
            input.ThrowPressed = false; // reset throw input
        }
    }

    public void EquipWeapon(GameObject weaponPrefab)
    {
        if (currentWeapon != null)
            Destroy(currentWeapon);

        currentWeapon = Instantiate(weaponPrefab, handTransform, false);
        currentWeapon.transform.localPosition = handOffsetPosition;
        currentWeapon.transform.localRotation = Quaternion.Euler(handOffsetRotation);

        // Remove physics while held
        currentRb = currentWeapon.GetComponent<Rigidbody>();
        if (currentRb != null)
        {
            currentRb.isKinematic = true;
            currentRb.detectCollisions = false;
        }
    }

    public void UnequipWeapon()
    {
        if (currentWeapon != null)
        {
            Destroy(currentWeapon);
            currentWeapon = null;
        }
    }

    private void ThrowWeapon()
    {
        if (currentWeapon == null) return;

        // Detach
        currentWeapon.transform.SetParent(null);

        // Enable physics
        if (currentRb == null)
            currentRb = currentWeapon.GetComponent<Rigidbody>();

        if (currentRb == null)
            currentRb = currentWeapon.AddComponent<Rigidbody>();

        currentRb.isKinematic = false;
        currentRb.detectCollisions = true;

        // Add forward throw force + spin
        Vector3 throwDir = cam.transform.forward + Vector3.up * 0.1f;
        currentRb.AddForce(throwDir.normalized * throwForce, ForceMode.VelocityChange);
        currentRb.AddTorque(Random.insideUnitSphere * throwSpin, ForceMode.VelocityChange);

        // Clear reference
        currentWeapon = null;
        currentRb = null;
    }
}
