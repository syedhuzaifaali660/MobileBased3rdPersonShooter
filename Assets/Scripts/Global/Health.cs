using UnityEngine;

public class Health : MonoBehaviour
{

    public float maxHealth;
    [HideInInspector]    public float currentHealth;


    //EMISSION VARIABLES
    public float blinkIntensity;
    public float blinkDuration;
    float blinkTimer;

    SkinnedMeshRenderer skinRender;

    UIHealthBar healthBar;
    SkinnedMeshRenderer render;

    public static bool isDead;

    void Start()
    {
        #region GETTING COMPONENTS

        currentHealth = maxHealth;
        var skinRender = GetComponentsInChildren<SkinnedMeshRenderer>();

        
        foreach (var mesh in skinRender)
        {
            render = mesh.GetComponentInChildren<SkinnedMeshRenderer>();

        }

        healthBar = GetComponentInChildren<UIHealthBar>();

        #endregion

        #region GETTING RIGIDBODIES

        var rigidBodies = GetComponentsInChildren<Rigidbody>();
        foreach (var rb in rigidBodies)        {
            AIHitBox hitBox = rb.gameObject.AddComponent<AIHitBox>();
            hitBox.health = this;  
            if(hitBox.gameObject != gameObject)
            {
                hitBox.gameObject.layer = LayerMask.NameToLayer("Hitbox");
            }
        }

        #endregion

        OnStart();
    }

    public void TakeDamage(float amount, Vector3 direction)    {
        currentHealth -= amount;
        if (healthBar != null)
        {
            healthBar.SetHealthPercentage(currentHealth / maxHealth);
        }
        OnDamage(direction);
        if(currentHealth <= 0.0f)        {
            Die(direction);
            
        }

        blinkTimer = blinkDuration;
    }

    public bool IsDead() {
        return currentHealth <= 0;
    }

    void Die(Vector3 direction)    {
        OnDeath(direction);
        isDead = true;
    }

    private void Update()
    {
        blinkTimer -= Time.deltaTime;
        float lerp = Mathf.Clamp01(blinkTimer / blinkDuration);
        float intensity = (lerp * blinkIntensity) + 1.0f;
        IntensityManipulator(intensity);

    }

    private void IntensityManipulator(float intensity)
    {

        for (int i = 0; i < 2; i++)
        {
            render.material.color = Color.white * intensity;
        }
    }

    protected virtual void OnStart()
    {

    }     
    protected virtual void OnDeath(Vector3 direction)
    {

    }     
    
    protected virtual void OnDamage(Vector3 direction)
    {

    } 
}
