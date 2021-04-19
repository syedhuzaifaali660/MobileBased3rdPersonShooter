using System.Collections.Generic;
using UnityEngine;


public class RaycastWeapon : MonoBehaviour
{

    class Bullet
    {
        public float time;
        public Vector3 initialPosition;
        public Vector3 initialVelocity;
        public TrailRenderer tracer;
        public int bounce;
    }

    #region Variable Declaration
    [Header("Gun Controllers")]
    public int fireRate = 25; 
    public float bulletSpeed = 1000.0f;
    public float bulletDrop = 0.0f;
    public int maxBounces = 0;
    public bool isFiring = false;

    public MeshSockets.SocketId holsterSocket; //--------------
    public int ammoCount;
    public int clipSize;
    public float damage=10;

    [Header("Gun Slot")]
    public ActiveWeapon.WeaponSlot weaponSlot;
    [Header("Gun Particle Effects")]
    public ParticleSystem[] muzzleFlash;
    public ParticleSystem hitEffect;
    public TrailRenderer tracerEffect;

    [Header("Raycast Transforms")]
    public Transform raycastOrigin;

    [HideInInspector] public WeaponRecoil recoil;
    public GameObject magazine;

    [Header("Weapon Name's")]
    public string weaponName;

    [Header("Animators")]
    public RuntimeAnimatorController animator;

    public LayerMask layerMask;

    //PRIVATE VARIABLES
    Ray ray;
    RaycastHit hitInfo;
    float accumulatedTime;
    List<Bullet> bullets = new List<Bullet>();
    float maxLifeTime = 3.0f;

    GameObject holder;
    UiButtonFuctionsScript btnToggler; //CUSTOM UI SCRIPT REFERENCE
    DekstopControlls desktopControlls;

    #endregion


    private void Awake()
    {
        recoil = GetComponent<WeaponRecoil>();
    }

    private void Start()
    {

        //btnToggler = GameObject.FindGameObjectWithTag("ScriptHolder").GetComponent<UiButtonFuctionsScript>();
        //joyButton = FindObjectOfType<Joybutton>();
        //a = FindObjectOfType<Test>();
        desktopControlls = GameObject.FindGameObjectWithTag("ScriptHolder").GetComponent<DekstopControlls>();

    }



    //GET POSITION OF THE BULLET
    Vector3 GetPosition(Bullet bullet)
    {
        // EQUATION
        //p + v*t + 0.5 * g *t *t

        //CALCULATING GRAVITY
        Vector3 gravity = Vector3.down * bulletDrop;
        return (bullet.initialPosition) + (bullet.initialVelocity * bullet.time) + (0.5f * gravity * bullet.time * bullet.time);
    }

    //CREATING BULLETS
    private Bullet CreateBullet(Vector3 position, Vector3 velocity)
    {
        Bullet bullet = new Bullet();
        bullet.initialPosition = position;
        bullet.initialVelocity = velocity;
        bullet.time = 0.0f;
        bullet.tracer = Instantiate(tracerEffect, position, Quaternion.identity);
        bullet.tracer.AddPosition(position);

        bullet.bounce = maxBounces;
        //Color color = Random.ColorHSV(0.46f, 0.61f);
        //float intensity = 20.0f;
        //Color rgb = new Color(color.r * intensity, color.g * intensity, color.b * intensity, color.a * intensity);
        return bullet;

    }

    public void StartFiring()
    {
        
        
        isFiring = true;
        if (accumulatedTime > 0.0f) {
            accumulatedTime = 0.0f;
        }
        recoil.Reset();

    }

    public void UpdateWeapon(float deltaTime, Vector3 target)
    {
        if (isFiring) {
            UpdateFiring(deltaTime,target);
        }

        //NEED TO KEEP TRACK OF COOLDOWN EVEN WHEN NOT FIRING TO PREVENT CLICK SPAM.
        accumulatedTime += deltaTime;

        UpdateBullets(deltaTime);
        if (Input.GetButtonUp("Fire1")) {
            StopFiring();
        }
    }

    //WEAPON UPDATE METHOD
    public void UpdateFiring(float deltaTime, Vector3 target)
    {

        //accumulatedTime += deltaTime;
        float fireInterval = 1.0f / fireRate;
        while(accumulatedTime>= 0.0f)
        {
            FireBullet(target);
            accumulatedTime -= fireInterval;
        }
    }

    //UPDATING BULLETS
    public void UpdateBullets(float deltaTime)
    {
        SimulateBullets(deltaTime);
        DestroyBullets();
    }

    //SIMULATING BULLETS
    void SimulateBullets(float deltaTime)
    {
        bullets.ForEach(bullet =>
        {
            Vector3 p0 = GetPosition(bullet);
            bullet.time += deltaTime;
            Vector3 p1 = GetPosition(bullet);
            RaycastSegment(p0,p1,bullet);
        });
    }

    //DESTROYING BULLETS AFTER SOMETIME
    void DestroyBullets()
    {
        bullets.RemoveAll(bullet => bullet.time >= maxLifeTime);
    }

    //RAYCASTING FROM GUN POINT TO HIT POINT
    void RaycastSegment(Vector3 start, Vector3 end, Bullet bullet)
    {
        Vector3 direction = (end - start);
        float distance = direction.magnitude;
        ray.origin = start;
        ray.direction = end - start;
        if (Physics.Raycast(ray, out hitInfo, distance, layerMask)) 
        {
            hitEffect.transform.position = hitInfo.point;
            hitEffect.transform.forward = hitInfo.normal;
            hitEffect.Emit(1);

            bullet.time = maxLifeTime;
            end = hitInfo.point;
            
            
            // bullet rocochet
            if(bullet.bounce > 0)
            {
                bullet.time = 0;
                bullet.initialPosition = hitInfo.point;
                bullet.initialVelocity = Vector3.Reflect(bullet.initialVelocity,hitInfo.normal);
                bullet.bounce--;

            }

            //collision impulse
            var rb2d = hitInfo.collider.GetComponent<Rigidbody>();
            if (rb2d)
            {
                rb2d.AddForceAtPosition(ray.direction * 20, hitInfo.point, ForceMode.Impulse);
            }
            //collision impulse
            var hitBox = hitInfo.collider.GetComponent<AIHitBox>();
            if (hitBox)
            {
                hitBox.OnRaycastHit(this,ray.direction);
            }
            
            
        }



        else
        {
            if (bullet.tracer == null) { return; }
            else
            {
                bullet.tracer.transform.position = end;
            }

           
        }

    }

    private void FireBullet(Vector3 target)
    {
        if(ammoCount <= 0) { return; }
        ammoCount--;

        foreach (var particle in muzzleFlash)
        {
            particle.Emit(1);
        }
        Vector3 velocity = (target - raycastOrigin.position).normalized * bulletSpeed;
        var bullet = CreateBullet(raycastOrigin.position, velocity);
        bullets.Add(bullet);

        recoil.GenerateRecoil(weaponName);
    }

    public void StopFiring()
    {
        isFiring = false;
    }
    

    private void Update()
    {
        if (!desktopControlls.DesktopControlls)
        {
            //Debug.Log(desktopControlls);
            #region CODE WORKING FOR MOBILE INPUTS

            //if (UIRaycaster.pressed && Input.GetButtonDown("Fire1") && !FindObjectOfType<ActiveWeapon>().rigController.GetBool("holster_weapon"))
            //{
            //    StartFiring();
            //}
            //if (isFiring)
            //{
            //    UpdateFiring(Time.deltaTime);

            //}
            //if (Input.GetButtonUp("Fire1") && !UIRaycaster.pressed)
            //{
            //    StopFiring();
            //}

            #endregion
        }
        else
        {
            #region CODE FOR DESKTOP
            //if (Input.GetButtonDown("Fire1") && !FindObjectOfType<ActiveWeapon>().rigController.GetBool("holster_weapon"))
            //{

            //    StartFiring();
            //}
            //if (isFiring)
            //{
            //    UpdateFiring(Time.deltaTime);
            //}
            //if (Input.GetButtonUp("Fire1"))
            //{
            //    StopFiring();
            //}
            #endregion
        }
    }



}
