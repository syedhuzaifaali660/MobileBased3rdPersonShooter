using System.Collections;
using UnityEngine;

public class ActiveWeapon : MonoBehaviour
{
    public enum WeaponSlot
    {
        Primary = 0,
        Secondary = 1
    }

    #region VARIABLE DECLARATION
    [Header("Aiming References")]
    public Transform crossHairTarget;
    public Transform[] weaponSlots;
    public CharacterAiming characterAiming;

    [Header("Animator")]
    public Animator rigController;

    public AmmoWidget ammoWidget;
    public bool isChangingWeapon;


    RaycastWeapon[] equipped_weapons = new RaycastWeapon[2];
    public int activeWeaponIndex;
    bool isHolstered = true;

    bool weaponEquiped;
    ReloadWeapon reload;

    #endregion
    //TESTING VARIABLES
    UiButtonFuctionsScript btnCountChecker;
    DekstopControlls desktopControlls;

    void Start()
    {
        reload = GetComponent<ReloadWeapon>();
        RaycastWeapon existingWeapon = GetComponentInChildren<RaycastWeapon>();

        if (existingWeapon)
        {
            Equip(existingWeapon);
        }
        
        //TESTING 
        btnCountChecker = GameObject.FindGameObjectWithTag("ScriptHolder").GetComponent<UiButtonFuctionsScript>();
        desktopControlls = GameObject.FindGameObjectWithTag("ScriptHolder").GetComponent<DekstopControlls>();
    }
    
    public bool IsFiring()
    {
        RaycastWeapon currentWeapon = GetActiveWeapon();
        if (!currentWeapon)
        {
            return false;
        }
        return currentWeapon.isFiring;
    }

    public RaycastWeapon GetActiveWeapon()
    {
        return GetWeapon(activeWeaponIndex);
    }

    RaycastWeapon GetWeapon(int index)
    {
        if(index < 0 || index >= equipped_weapons.Length)
        {
            return null;
        }
        return equipped_weapons[index];
    }


    void Update()
    {
        
        var weapon = GetWeapon(activeWeaponIndex);
        bool notSprinting = rigController.GetCurrentAnimatorStateInfo(2).shortNameHash == Animator.StringToHash("notSprinting");
        bool canFire = !isHolstered && notSprinting && !reload.isReloading;
        bool canFireMobile = !isHolstered && notSprinting && !reload.isReloading && UIRaycaster.pressed;

        //---------------------------------------------------------NEW CODE
        if (!desktopControlls.DesktopControlls)
        {

            if (weapon)
            {
                if (Input.GetButton("Fire1") && canFireMobile && !weapon.isFiring)
                {
                    weapon.StartFiring();
                }
                if (Input.GetButtonUp("Fire1") || !canFireMobile)
                {
                    weapon.StopFiring();
                }
                weapon.UpdateWeapon(Time.deltaTime, crossHairTarget.position);
            }
        }
        else
        {
            if (weapon)
            {
                if (Input.GetButton("Fire1") && canFire && !weapon.isFiring)
                {
                    weapon.StartFiring();
                }
                if (Input.GetButtonUp("Fire1") || !canFire)
                {
                    weapon.StopFiring();
                }
                weapon.UpdateWeapon(Time.deltaTime, crossHairTarget.position);
            }
        }

        #region ORIGINAL CODE DESKTOP
        //if (weapon /*&& !isHolstered && notSprinting*/)        {
        //    if(Input.GetButton("Fire1") && canFire && !weapon.isFiring)
        //    {
        //        weapon.StartFiring();
        //    }
        //    if(Input.GetButtonUp("Fire1") || !canFire)
        //    {
        //        weapon.StopFiring();
        //    }
        //    //weapon.UpdateBullets(Time.deltaTime); //old code
        //    weapon.UpdateWeapon(Time.deltaTime,crossHairTarget.position);
        //}
        #endregion
        //-----------------------------------------------------------
        #region Keyboard Functionality
        //if (Input.GetKeyDown(KeyCode.X))            {
        //    ToggleActiveWeapon();
        //}

        //if (Input.GetKeyDown(KeyCode.Alpha1))        {
        //    SetActiveWeapon(WeaponSlot.Primary);
        //}

        //if (Input.GetKeyDown(KeyCode.Alpha2))        {
        //    SetActiveWeapon(WeaponSlot.Secondary);

        //}
        #endregion
    }

    #region FIRING PROBLEM SOLVED FINALLY !!!!!
    void FiringOffForHolsteredWeapon(int currentWeaponIndex)
    {
        if (weaponEquiped)
        {

            if (currentWeaponIndex == 1)            {

                GameObject go2 = GameObject.FindWithTag("Gun_P");
                if (go2 != null)                {
                    GameObject.FindWithTag("Gun_P").GetComponent<RaycastWeapon>().enabled = true;
                    //Debug.Log("Current Weapon Index Custom " + currentWeaponIndex);
                }else { return; }
                
                GameObject go = GameObject.FindWithTag("Gun_R");
                if (go != null) { 
                    GameObject.FindWithTag("Gun_R").GetComponent<RaycastWeapon>().enabled = false;
                }  else { return; }

            }  
            else if (currentWeaponIndex == 0)    {
                
                GameObject go2 = GameObject.FindWithTag("Gun_R");
                if (go2 != null)
                {
                    GameObject.FindWithTag("Gun_R").GetComponent<RaycastWeapon>().enabled = true;
                    //Debug.Log("Current Weapon Index Custom " + currentWeaponIndex);
                }   else { return; }
                

                GameObject go = GameObject.FindWithTag("Gun_P");
                if (go != null)                {
                    GameObject.FindWithTag("Gun_P").GetComponent<RaycastWeapon>().enabled = false;
                } else { return; }
            }            else { return; }
        }
        else { return; }
    }
    #endregion

    //TESTING
    int currentIndexWeapon;
    int WeaponIndexChecker(int weaponIndexCheck)
    {
        return weaponIndexCheck;
    }

    public void Equip(RaycastWeapon newWeapon)
    {
        int weaponSlotIndex = (int)newWeapon.weaponSlot;
        var weapon = GetWeapon(weaponSlotIndex);
        if (weapon)        {
            Destroy(weapon.gameObject);
        }
        weapon = newWeapon; 
        weapon.recoil.characterAiming = characterAiming;
        weapon.recoil.rigController = rigController;
        weapon.transform.SetParent (weaponSlots[weaponSlotIndex],false);
        equipped_weapons[weaponSlotIndex] = weapon;

        SetActiveWeapon(newWeapon.weaponSlot);       
        weaponEquiped = true;
        ammoWidget.Refresh(weapon.ammoCount);
        
        //TESTING
        currentIndexWeapon = WeaponIndexChecker(weaponSlotIndex);
        //Debug.Log(currentIndexWeapon);
    }

    void SetActiveWeapon(WeaponSlot weaponSlot)
    {

        int holsterIndex = activeWeaponIndex;
        int activateIndex = (int)weaponSlot;

        if(holsterIndex == activateIndex)
        {
            holsterIndex = -1;
        }
        StartCoroutine(SwitchWeapon(holsterIndex, activateIndex));
    }

    void ToggleActiveWeapon()
    {
        bool isHolstered = rigController.GetBool("holster_weapon");
        if (isHolstered)        {
            StartCoroutine(ActivateWeapon(activeWeaponIndex));
        } else        {
            StartCoroutine(HolsterWeapon(activeWeaponIndex));   
        }
    }

    IEnumerator SwitchWeapon (int holsterIndex, int activateIndex)
    {
        rigController.SetInteger("weapon_index", activateIndex);
        yield return StartCoroutine(HolsterWeapon(holsterIndex));
        yield return StartCoroutine(ActivateWeapon(activateIndex));

        activeWeaponIndex = activateIndex;
    }

    IEnumerator HolsterWeapon (int index)
    {
        isChangingWeapon = true;
        isHolstered = true;


        var weapon = GetWeapon(index);
        if(weapon)
        {
            rigController.SetBool("holster_weapon", true);
            do
            {
                yield return new WaitForEndOfFrame();
            }
            while (rigController.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f);
        }
        isChangingWeapon = false;
    }

    IEnumerator ActivateWeapon (int index)
    {
        isChangingWeapon = true;

        FiringOffForHolsteredWeapon(index);

        var weapon = GetWeapon(index);
        if(weapon)
        {
            rigController.SetBool("holster_weapon", false);
            rigController.Play("equip_" + weapon.weaponName);
            do
            {
                yield return new WaitForEndOfFrame();
            } 
            while (rigController.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f);
            isHolstered = false;

        }
        isChangingWeapon = false;
    }

    public void DropWeapon()
    {
        var currentWeapon = GetActiveWeapon();
        if (currentWeapon)
        {
            currentWeapon.transform.SetParent(null);
            currentWeapon.gameObject.GetComponent<BoxCollider>().enabled = true;
            currentWeapon.gameObject.AddComponent<Rigidbody>();
            equipped_weapons[activeWeaponIndex] = null;

        }
    }


    #region BUTTON REFERENCES
    bool btn1Pressed;
    bool btn2Pressed;
    bool weaponEquipedCheckerRifle;
    bool weaponEquipedCheckerPistol;
    public void ButtonGun1Primary()
    {
        btn1Pressed = true;
        btn2Pressed = false;

        if (btn1Pressed && !btn2Pressed)
        {
            #region
            ////Switch weapon
            //SetActiveWeapon(WeaponSlot.Primary);
            //weaponEquipedCheckerRifle = true;
            //Debug.Log("Switch");
            #endregion

            if (weaponEquipedCheckerRifle)
            {
                ToggleActiveWeapon();
                Debug.Log("Holstered");
                weaponEquipedCheckerRifle = false;
            }
            else
            {
                //Switch weapon
                SetActiveWeapon(WeaponSlot.Primary);
                weaponEquipedCheckerRifle = true;
                weaponEquipedCheckerPistol = false;
                Debug.Log("Switch");
            }



        }
        else if (!btn1Pressed && btn2Pressed)
        {
            //Switch
            Debug.Log("Switch");
            SetActiveWeapon(WeaponSlot.Primary);
            weaponEquipedCheckerRifle = true;
            weaponEquipedCheckerPistol = false;

            btn2Pressed = false;
        }
        else if (btn1Pressed && btn2Pressed)
        {
            //both true Check what is happening
            Debug.Log("both true check what happens");
        }
        else
        {

            #region
            ////both false equip to weapon
            //if (weaponEquipedCheckerRifle)
            //{
            //    ToggleActiveWeapon();
            //    weaponEquipedCheckerRifle = false;
            //    Debug.Log(btn1Pressed + " Btn1 " + btn2Pressed + "Btn2" + "equiped rifle" + weaponEquipedCheckerRifle);
            //}
            //else if (!weaponEquipedCheckerRifle)
            //{
            //    SetActiveWeapon(WeaponSlot.Primary);
            //    weaponEquipedCheckerRifle = true;
            //    Debug.Log(btn1Pressed + " Btn1 " + btn2Pressed + "Btn2" + "equiped rifle" + weaponEquipedCheckerRifle);
            //}
            //else
            //{
            //    Debug.Log(btn1Pressed + " Btn1 " + btn2Pressed + "Btn2" + "equiped rifle" + weaponEquipedCheckerRifle);
            //}
            #endregion
            Debug.Log(btn1Pressed + " Btn1 " + btn2Pressed + "Btn2" + "equiped rifle" + weaponEquipedCheckerRifle);


        }

    }
    public void ButtonHolsterWeaponButton()
    {

         ToggleActiveWeapon();

    }
    public void ButtonGun2Secondary()
    {
        btn1Pressed = false;
        btn2Pressed = true;

        if (btn2Pressed && !btn1Pressed)
        {
            #region
            ////Switch weapon
            //SetActiveWeapon(WeaponSlot.Secondary);
            //weaponEquipedCheckerPistol = true;
            //Debug.Log("Switch" + "Pistol Equiped " + weaponEquipedCheckerPistol);
            #endregion

            if (weaponEquipedCheckerPistol)
            {
                ToggleActiveWeapon();
                Debug.Log("Holstered");
                weaponEquipedCheckerPistol = false;
            }
            else
            {
                //Switch weapon
                SetActiveWeapon(WeaponSlot.Secondary);
                weaponEquipedCheckerPistol = true;
                weaponEquipedCheckerRifle = false;

                Debug.Log("Switch" + "Pistol Equiped " + weaponEquipedCheckerPistol);
            }
        }
        else if (!btn2Pressed && btn1Pressed)
        {
            //Switch
            Debug.Log("Switch");
            SetActiveWeapon(WeaponSlot.Secondary);
            weaponEquipedCheckerPistol = true;
            weaponEquipedCheckerRifle = false;
            btn1Pressed = false;
        }
        else if (btn2Pressed && btn1Pressed)
        {
            //both true Check what is happening
            Debug.Log("both true check what happens");
        }
        else
        {
            #region
            ////both false equip to weapon
            //if(weaponEquipedCheckerPistol)
            //{
            //    ToggleActiveWeapon();
            //    weaponEquipedCheckerPistol = false;
            //    Debug.Log(btn1Pressed + " Btn1 " + btn2Pressed + "Btn2" + "Equiped Pistol " + weaponEquipedCheckerPistol);
            //}
            //else if (!weaponEquipedCheckerPistol)
            //{
            //    SetActiveWeapon(WeaponSlot.Secondary);
            //    weaponEquipedCheckerPistol = true;
            //    Debug.Log(btn1Pressed + " Btn1 " + btn2Pressed + "Btn2" + "Equiped Pistol " + weaponEquipedCheckerPistol);
            //}else
            //{
            //    Debug.Log(btn1Pressed + " Btn1 " + btn2Pressed + "Btn2" + "Equiped Pistol " + weaponEquipedCheckerPistol);
            //}
            //Debug.Log("both false equip Weapon Secondary");
            #endregion
            Debug.Log(btn1Pressed + " Btn1 " + btn2Pressed + "Btn2" + "Equiped Pistol " + weaponEquipedCheckerPistol);
        }

    }


    #endregion

}
