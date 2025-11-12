using UnityEngine;

public class ThrowableWeapon : MonoBehaviour
{
    private Rigidbody rb;
    private bool isThrown = false;
    private Collider col;

    [Header("Pickup Replacement")]
    [SerializeField] private GameObject pickupAxePrefab; // prefab of pickup-able axe

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();

        if (rb == null)
        {
            Debug.LogError("Missing Rigidbody on " + name);
        }

        rb.isKinematic = true; // start as held
    }

    public void Throw(Vector3 force)
    {
        transform.SetParent(null);
        rb.isKinematic = false;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        Vector3 realisticForce = force + Vector3.up * (force.magnitude * 0.15f);
        rb.AddForce(realisticForce, ForceMode.VelocityChange);

        rb.AddTorque(transform.right * 25f + transform.up * 10f, ForceMode.VelocityChange);

        rb.AddTorque(Random.insideUnitSphere * 5f, ForceMode.VelocityChange);

        isThrown = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!isThrown) return;

        // Stick weapon into surface
        rb.isKinematic = true;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        isThrown = false;

        if (pickupAxePrefab != null)
        {
            Vector3 spawnPos = transform.position + Vector3.up * 0.05f;
            Quaternion spawnRot = Quaternion.identity;
            Instantiate(pickupAxePrefab, spawnPos, spawnRot);
        }

        // Remove the thrown one (so it looks like it became the pickup)
        Destroy(gameObject);
    }
}
