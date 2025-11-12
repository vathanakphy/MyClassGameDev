using UnityEngine;
using System.Collections;

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

    private Animator animator;

    private void Awake()
    {
        input = GetComponent<PlayerLocalmotionInput>();
        cam = Camera.main;

        animator = GetComponent<Animator>();
        if (animator == null)
            animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        if (currentWeapon != null && input.AttackPressed)
        {
            PerformAttack();
            input.AttackPressed = false; // Reset attack input
        }

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

        // Disable physics while held
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

        Vector3 throwDir = (cam.transform.forward + Vector3.up * 0.2f).normalized;
        Vector3 throwForceVector = throwDir * throwForce;

        currentRb.AddForce(throwForceVector, ForceMode.VelocityChange);
        currentRb.AddTorque(Random.insideUnitSphere * throwSpin, ForceMode.VelocityChange);

        var throwable = currentWeapon.GetComponent<ThrowableWeapon>();
        if (throwable != null)
        {
            throwable.Throw(throwForceVector);
        }

        // Clear reference
        currentWeapon = null;
        currentRb = null;
    }


    private void PerformAttack()
    {
        if (animator != null)
        {
            animator.SetBool("IsAttack", true);
            StartCoroutine(ResetAttackAnim(2.23f)); // Reset after short time (adjust to match your animation)
        }

        // Optional: add melee hit detection, raycast, or trigger logic here
        Debug.Log("Player performed attack with weapon!");
    }

    private IEnumerator ResetAttackAnim(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (animator != null)
            animator.SetBool("IsAttack", false);
    }
}
