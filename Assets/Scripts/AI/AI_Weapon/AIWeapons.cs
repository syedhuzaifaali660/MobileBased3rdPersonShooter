using System.Collections;
using UnityEngine;

public class AIWeapons : MonoBehaviour
{

    RaycastWeapon currentWeapon;
    Animator animator;
    MeshSockets sockets;
    [HideInInspector]public WeaponIK weaponIk;
    Transform currentTarget;
    bool weaponActive = false;
    public float inaccuracy = 0.0f;

    bool isReloading = false;
    public float dropForce = 1.5f;
    GameObject magazineHand;
    
    private void Awake()
    {
        animator = GetComponent<Animator>();
        sockets = GetComponent<MeshSockets>();
        weaponIk = GetComponent<WeaponIK>();
    }

    public void Equip(RaycastWeapon weapon)
    {
        currentWeapon = weapon;
        sockets.Attach(weapon.transform, weapon.holsterSocket);
    }

    private void Update() {
        if(currentTarget && currentWeapon && weaponActive) {
            Vector3 target = currentTarget.position + weaponIk.targetOffset;
            target += Random.insideUnitSphere * inaccuracy;
            currentWeapon.UpdateWeapon(Time.deltaTime, target);
        }
    }
    public void SetFiring(bool enabled)
    {
        if (enabled) { currentWeapon.StartFiring(); } 
        else { currentWeapon.StopFiring(); }
        Debug.Log(enabled);
    }
    
    public void ActivateWeapon() {
        //animator.SetBool("Equip", true);
        StartCoroutine(EquipWeapon());
    }

    IEnumerator EquipWeapon()
    {
        // ask
        //animator.SetInteger("WeaponSlot", (int)currentWeapon.weaponSlot);
        animator.runtimeAnimatorController = currentWeapon.animator;
        animator.SetBool("Equip", true);
        yield return new WaitForSeconds(0.5f);
        while (animator.GetCurrentAnimatorStateInfo(1).normalizedTime < 1.0f) {
            yield return null;
        }

        weaponIk.SetAimTransform(currentWeapon.raycastOrigin);
        weaponActive = true;
    }

    public void DeactivateWeapon()
    {
        SetTarget(null);
        SetFiring(false);
        StartCoroutine(HolsterWeapon());
    }

    public void ReloadWeapon()
    {

    }
    IEnumerator HolsterWeapon()
    {
        // ask
        weaponActive = false;
        //animator.SetInteger("WeaponSlot", (int)currentWeapon.weaponSlot);
        animator.SetBool("Equip", false);

        yield return new WaitForSeconds(0.5f);
        while (animator.GetCurrentAnimatorStateInfo(1).normalizedTime < 1.0f) {
            yield return null;
        }

        weaponIk.SetAimTransform(currentWeapon.raycastOrigin);
    }

    IEnumerator ReloadWeaponAnimation()
    {
        isReloading = true;
        animator.SetBool("Equip", false);

        yield return new WaitForSeconds(0.5f);
        while (animator.GetCurrentAnimatorStateInfo(1).normalizedTime < 1.0f)
        {
            yield return null;
        }

        weaponIk.SetAimTransform(currentWeapon.raycastOrigin);
        isReloading = false;
    }
    public void DropWeapon()
    {
        if (currentWeapon)
        {
            currentWeapon.transform.SetParent(null);
            currentWeapon.gameObject.GetComponent<BoxCollider>().enabled = true;
            currentWeapon.gameObject.AddComponent<Rigidbody>();
            currentWeapon = null;

        }
    }
    public bool HasWeapon()
    {
        return currentWeapon != null;
    }

    public void OnAnimationEvent(string eventName)
    {
        switch (eventName)
        {
            case "equipWeapon":
                AttachWeapon();
                break;
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

    public void SetTarget(Transform target)
    {
        weaponIk.SetTargetTransform(target);
        currentTarget = target;
    }


    #region RELOADING WEAPON

    void AttachWeapon()
    {
        bool equipping = animator.GetBool("Equip");
        if (equipping)
        {
            sockets.Attach(currentWeapon.transform, MeshSockets.SocketId.RightHand);
        }
        else
        {
            sockets.Attach(currentWeapon.transform, currentWeapon.holsterSocket);
        }
    }

    void DetachMagazine()
    {
        var leftHand = animator.GetBoneTransform(HumanBodyBones.LeftHand);
        RaycastWeapon weapon = currentWeapon;
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
        RaycastWeapon weapon = currentWeapon;
        weapon.magazine.SetActive(true);
        Destroy(magazineHand);
        weapon.ammoCount = weapon.clipSize;
        animator.ResetTrigger("reload_weapon");

        isReloading = false;

    }

    #endregion
}
