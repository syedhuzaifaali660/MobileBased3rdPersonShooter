
using UnityEngine;

public class AIHitBox : MonoBehaviour
{
    public Health health;


    public void OnRaycastHit(RaycastWeapon weapon,Vector3 direction)
    {
        health.TakeDamage(weapon.damage,direction);
    }
}
