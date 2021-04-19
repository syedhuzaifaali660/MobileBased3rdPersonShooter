using UnityEngine;
using UnityEngine.Rendering.PostProcessing;


public class PlayerHealth : Health
{
    Ragdoll ragdoll;
    ActiveWeapon weapons;
    CharacterAiming aiming;
    PostProcessProfile postProcessing;
    CameraManager camManagaer;

    protected override void OnStart()
    {
        ragdoll = GetComponent<Ragdoll>();
        weapons = GetComponent<ActiveWeapon>();
        aiming = GetComponent<CharacterAiming>();
        postProcessing = FindObjectOfType<PostProcessVolume>().profile;
        camManagaer = FindObjectOfType<CameraManager>();

    }
    protected override void OnDeath(Vector3 direction)
    {
        ragdoll.ActivateRagdoll();
        direction.y = 1.0f;
        ragdoll.ApplyForce(direction);
        weapons.DropWeapon();
        aiming.enabled = false;
        camManagaer.EnableKillCam();
    }

    protected override void OnDamage(Vector3 direction)
    {
        Vignette vignette;
        if(postProcessing.TryGetSettings(out vignette))
        {
            float percent = 1.0f - (currentHealth / maxHealth);
            vignette.intensity.value = percent * 0.55f;
        }
    }
}
