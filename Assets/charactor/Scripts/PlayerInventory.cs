using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    private WeaponPickup _nearbyWeapon;
    public GameObject currentWeapon { get; private set; }

    [SerializeField] private PlayerWeaponHandler weaponHandler;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            TryPickupNearestWeapon();
        }
    }

    private void TryPickupNearestWeapon()
    {
        WeaponPickup[] allPickups = FindObjectsOfType<WeaponPickup>();

        float nearestDistance = float.MaxValue;
        WeaponPickup nearest = null;

        foreach (var pickup in allPickups)
        {
            float dist = Vector3.Distance(transform.position, pickup.transform.position);
            if (dist < 3f && dist < nearestDistance)
            {
                nearestDistance = dist;
                nearest = pickup;
            }
        }

        if (nearest != null)
        {
            CollectWeapon(nearest);
        }
    }

    private void CollectWeapon(WeaponPickup weaponPickup)
    {
        if (weaponPickup == null) return;

        // Unequip existing weapon
        if (currentWeapon != null)
        {
            weaponHandler.UnequipWeapon();
            currentWeapon = null;
        }

        // Let weapon handler handle the actual equip + instantiate
        weaponHandler.EquipWeapon(weaponPickup.WeaponPrefab);

        // Destroy pickup object
        Destroy(weaponPickup.gameObject);
    }
}
