using UnityEngine;

public class ReloadWeapon : MonoBehaviour
{

    public Animator rigcontroller;
    public WeaponAnimationEvents animaitonEvents;
    public ActiveWeapon activeWeapon;
    public Transform leftHand;
    public AmmoWidget ammoWidget;

    public bool isReloading;

    GameObject magazineHand;
    
    // Start is called before the first frame update
    void Start()
    {
        animaitonEvents.WeaponAnimationEvent.AddListener(OnAnimationEvent);
    }

    // Update is called once per frame
    void Update()
    {
        RaycastWeapon weapon = activeWeapon.GetActiveWeapon();
        if (weapon)
        {

            if (Input.GetKeyDown(KeyCode.R) || weapon.ammoCount <= 0)
            {
                rigcontroller.SetTrigger("reload_weapon");
                isReloading = true;
            }
            if (weapon.isFiring)
            {
                ammoWidget.Refresh(weapon.ammoCount);
            }
        }
    }

    void OnAnimationEvent(string eventName)
    {
        Debug.Log(eventName);
        switch (eventName)
        {
            case "detach_magazine":
                DetachMagazine();
                break;
            case "drop_magazine":
                DropMagazine();
                break;
            case "refill_magazine":
                RefillMagazine();
                break;
            case "attach_magazine":
                AttachMagazine();
                break;


        }
    }



    void DetachMagazine()
    {
        RaycastWeapon weapon = activeWeapon.GetActiveWeapon();
        magazineHand = Instantiate(weapon.magazine, leftHand, true);
        weapon.magazine.SetActive(false);
    }
    void DropMagazine()
    {
        GameObject dropperMagazine = Instantiate(magazineHand, magazineHand.transform.position, magazineHand.transform.rotation);
        dropperMagazine.AddComponent<Rigidbody>();
        dropperMagazine.AddComponent<BoxCollider>();
        dropperMagazine.AddComponent<Destroyer>();

        magazineHand.SetActive(false);

    }
    void RefillMagazine()
    {
        magazineHand.SetActive(true);
    }
    void AttachMagazine()
    {
        RaycastWeapon weapon = activeWeapon.GetActiveWeapon();
        weapon.magazine.SetActive(true);
        Destroy(magazineHand);
        weapon.ammoCount = weapon.clipSize;
        rigcontroller.ResetTrigger("reload_weapon");

        ammoWidget.Refresh(weapon.ammoCount);

        isReloading = false;

    }    
    



    
}
