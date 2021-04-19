using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    public RaycastWeapon weaponFab;

    private void OnTriggerEnter(Collider other)
    {
        ActiveWeapon activeWeapon = other.gameObject.GetComponent<ActiveWeapon>();
        if (activeWeapon) {
            RaycastWeapon newWeapon = Instantiate(weaponFab);
            activeWeapon.Equip(newWeapon);
            Destroy(gameObject);
        }

        AIWeapons aiWepons = other.gameObject.GetComponent<AIWeapons>();
        if (aiWepons) {
            RaycastWeapon newWeapon = Instantiate(weaponFab);
            aiWepons.Equip(newWeapon);
            Destroy(gameObject);

        }



    }
    

}
