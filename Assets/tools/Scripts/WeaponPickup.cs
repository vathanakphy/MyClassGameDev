using UnityEngine;

[RequireComponent(typeof(Collider))]
public class WeaponPickup : MonoBehaviour
{
    [Header("Pickup Settings")]
    [SerializeField] private GameObject weaponPrefab;
    public GameObject WeaponPrefab => weaponPrefab;
    [SerializeField] private float pickupRange = 2f;

    private bool isInRange = false;
    private PlayerWeaponHandler playerWeaponHandler;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerWeaponHandler = other.GetComponent<PlayerWeaponHandler>();
            isInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerWeaponHandler = null;
            isInRange = false;
        }
    }

    private void Update()
    {
        if (isInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (playerWeaponHandler != null)
            {
                playerWeaponHandler.EquipWeapon(weaponPrefab);
                Destroy(gameObject);
            }
        }
    }
}
