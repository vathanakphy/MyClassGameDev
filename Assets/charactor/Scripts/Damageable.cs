using UnityEngine;

public class Damageable : MonoBehaviour
{
    public float health = 100f;

    [Header("Death Spawn")]
    [SerializeField] private GameObject spawnOnDeathPrefab; // prefab to spawn when this dies
    [SerializeField] private Vector3 spawnOffset = Vector3.up * 0.1f; // small lift if needed

    public void TakeDamage(float amount)
    {
        health -= amount;
        Debug.Log($"{gameObject.name} took {amount} damage. Remaining: {health}");

        if (health <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} died!");

        if (spawnOnDeathPrefab != null)
        {
            Instantiate(
                spawnOnDeathPrefab,
                transform.position + spawnOffset,
                transform.rotation
            );
        }

        Destroy(gameObject);
    }
}
