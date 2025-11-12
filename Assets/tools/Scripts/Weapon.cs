using UnityEngine;

public class Weapon : MonoBehaviour
{
    public float lifeTime = 5f;
    public float damage = 25f;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Damageable target = collision.collider.GetComponent<Damageable>();
        if (target != null)
        {
            target.TakeDamage(damage);
        }

        // Optional: stick or bounce effect
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.velocity = Vector3.zero;
        transform.parent = collision.transform;

        // destroy after a delay
        Destroy(gameObject, 1f);
    }
}
