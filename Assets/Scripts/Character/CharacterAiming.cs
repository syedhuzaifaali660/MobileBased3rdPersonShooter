using UnityEngine;
using UnityEngine.Animations.Rigging;
using Cinemachine;

public class CharacterAiming : MonoBehaviour
{
    #region VARIABLE INITIALIZATION
    [Header("ADJUSTMENT VARIABLES")]
    public float turnSpeed = 15;
    public float aimDuration = 0.3f;

    [Header("GAMEOBJECT TRANSFORM INPUTS")]
    public Transform cameraLookAt;

    [Header("GAMEOBJECT BOOLEAN INPUTS")]
    public bool isAiming;

    [Header("Cinemachine Variables")]
    public Cinemachine.AxisState xAxis;
    public Cinemachine.AxisState yAxis;

    //Private Variables
    Camera mainCamera;
    RaycastWeapon weapon;
    Animator animator;
    ActiveWeapon activeWeapon;
    int isAimingParam = Animator.StringToHash("isAiming");
    
    
    UiButtonFuctionsScript btnCountChecker;
    #endregion\

    DekstopControlls desktopControlls;
    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        //Cursor.visible = false;
        //Cursor.lockState = CursorLockMode.Locked;
        animator = GetComponent<Animator>();
        activeWeapon = GetComponent<ActiveWeapon>();

        //USED FOR AIMING BUTTON
        btnCountChecker = GameObject.FindGameObjectWithTag("ScriptHolder").GetComponent<UiButtonFuctionsScript>();
        
        desktopControlls = GameObject.FindGameObjectWithTag("ScriptHolder").GetComponent<DekstopControlls>();

    }



    private void Update()
    {


        #region recoil
        //isAiming = Input.GetMouseButton(1);
        animator.SetBool(isAimingParam, isAiming);

        var weapon = activeWeapon.GetActiveWeapon();
        if (weapon)
        {
            weapon.recoil.recoilModifier = isAiming ? 0.3f : 1.0f;
        }
        #endregion
        if (desktopControlls.DesktopControlls)
        {
            //Debug.Log(desktopControlls.DesktopControlls);
            #region Camera Aiming Desktop

            xAxis.Update(Time.fixedDeltaTime);
            yAxis.Update(Time.fixedDeltaTime);

            cameraLookAt.eulerAngles = new Vector3(yAxis.Value, xAxis.Value, 0);


            //Rotation of camera in Y axis
            float yawCamera = mainCamera.transform.rotation.eulerAngles.y;

            //Blend from current rotation towards the cams rotatiion only in y axis
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, yawCamera, 0), turnSpeed * Time.fixedDeltaTime);
            #endregion
        }
        else
        {
            #region MOBILE AIMING CODE WORKING
            if (Input.touchCount > 0)
            {
                if ((Input.touches[0].position.x > Screen.width / 2 && Input.touches[0].phase == TouchPhase.Moved)
                    ||
                    (Input.touches[1].position.x > Screen.width / 2 && Input.touches[1].phase == TouchPhase.Moved))
                {
                    #region TESTING CODE 5 

                    xAxis.Update(Time.fixedDeltaTime);
                    yAxis.Update(Time.fixedDeltaTime);

                    cameraLookAt.eulerAngles = new Vector3(yAxis.Value, xAxis.Value, 0);

                    //Rotation of camera in Y axis
                    float yawCamera = mainCamera.transform.rotation.eulerAngles.y;

                    //Blend from current rotation towards the cams rotatiion only in y axis
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, yawCamera, 0), turnSpeed * Time.fixedDeltaTime);

                    #endregion
                }

            }
            #endregion
        }
    }


    

    #region BUTTON REFERENCES

    public void AimButton()
    {
        isAiming = btnCountChecker.ButtonPressCountChecker(isAiming);
    }

    #endregion
}

