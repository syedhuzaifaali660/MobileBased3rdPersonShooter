using System.Threading;
using UnityEngine;

public class CharacterLocomotion : MonoBehaviour
{
    #region VARIABLE INITIALIZATION
    [Header("ANIMATORS")]
    public Animator rigController;

    [Header("PLAYER CONTROLES")]
    public float jumpHeight;
    public float gravity;
    public float stepDown;
    public float airControl;
    public float jumpDamp;
    public float groundSpeed;
    public float pushPower = 2.0F;

    CharacterAiming characterAiming;
    CharacterController cc;
    ActiveWeapon activeWeapon;
    ReloadWeapon reloadWeapon;
    Vector3 velocity;
    bool isJumping;
    int isSprintingParam = Animator.StringToHash("isSprinting");

    Animator animator;
    Vector2 input;
    Vector3 rootMotion;

    //Testing
    UiButtonFuctionsScript btnScriptTapCounterHelper;
    #endregion

    #region THIS IS FOR MOVEMENT OF PLAYER WITH JOYSTICK
    [Header("JOYSTICK INPUTS FOR PLAYER MOVEMENT AND ROTATION")]
    private FixedJoystick Leftjoystick;


    #endregion

    bool mobileSprintingButtonPressed;

    DekstopControlls desktopControlls;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        cc = GetComponent<CharacterController>();
        activeWeapon = GetComponent<ActiveWeapon>();
        reloadWeapon = GetComponent<ReloadWeapon>();
        characterAiming = GetComponent<CharacterAiming>();
        
        #region  Movement with Joystick
        
        Leftjoystick = FindObjectOfType<FixedJoystick>();


        #endregion
        btnScriptTapCounterHelper = GameObject.FindGameObjectWithTag("ScriptHolder").GetComponent<UiButtonFuctionsScript>();
        desktopControlls = GameObject.FindGameObjectWithTag("ScriptHolder").GetComponent<DekstopControlls>();

    }



    // Update is called once per frame
    void Update()
    {
        input.x = Input.GetAxis("Horizontal");
        input.y = Input.GetAxis("Vertical");


        //animator.SetFloat("InputX", (Leftjoystick.Horizontal * 200f) * Time.fixedDeltaTime + input.x);
        //animator.SetFloat("InputY", (Leftjoystick.Vertical * 200f) * Time.fixedDeltaTime + input.y);

        UpdateIsSprinting();

        #region DESKTOP CONTROLLERS
        if (desktopControlls.DesktopControlls)
        {
            animator.SetFloat("InputX", input.x);
            animator.SetFloat("InputY", input.y);

            //Debug.Log(desktopControlls.DesktopControlls);
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Jump();
            }

            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                mobileSprintingButtonPressed = true;
            }
            if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                mobileSprintingButtonPressed = false;
            }
        }
        else {
            animator.SetFloat("InputX", (Leftjoystick.Horizontal * 200f) * Time.fixedDeltaTime);
            animator.SetFloat("InputY", (Leftjoystick.Vertical * 200f) * Time.fixedDeltaTime);

        }

        #endregion


    }
    //int count = 0;
    bool IsSprinting()
    {
        //bool isSprinting = Input.GetKey(KeyCode.LeftShift);
        bool isSprinting = mobileSprintingButtonPressed;

        //if (mobileSprintingButtonPressed != false && count == 0) { Debug.Log(mobileSprintingButtonPressed); count = 1; }
        //if (mobileSprintingButtonPressed == false && count == 1) { Debug.Log(mobileSprintingButtonPressed); count = 0; }
        bool isFiring = activeWeapon.IsFiring();
        bool isReloading = reloadWeapon.isReloading;
        bool isChangingWeapon = activeWeapon.isChangingWeapon;
        bool isAiming = characterAiming.isAiming;
        return isSprinting && !isFiring && !isReloading && !isChangingWeapon && !isAiming;
    }
    private void UpdateIsSprinting()
    {
        bool isSprinting = IsSprinting();
        animator.SetBool(isSprintingParam, isSprinting);
        rigController.SetBool(isSprintingParam, isSprinting);
    }


    private void FixedUpdate()
    {
        if (isJumping /*&& playerInput.PlayerMain.Jump.triggered*/) //is in air state
        {
            UpdaeInAir();
        }
        else
        {
            //is grounded state
            UpdateOnGround();

        }
        cc.Move(rootMotion);
        rootMotion = Vector3.zero;
    }

    private void UpdateOnGround()
    {
        Vector3 stepForwardAmount = rootMotion * groundSpeed;
        Vector3 stepDownAmount = Vector3.down * stepDown;

        cc.Move(stepForwardAmount + stepDownAmount);
        rootMotion = Vector3.zero;

        if (!cc.isGrounded)
        {
            SetInAir(0);
        }
    }

    private void UpdaeInAir()
    {
        velocity.y -= gravity * Time.fixedDeltaTime;
        Vector3 displacement = velocity * Time.fixedDeltaTime;
        displacement += CalculateAircontrol();
        cc.Move(displacement);
        isJumping = !cc.isGrounded;
        rootMotion = Vector3.zero;
        animator.SetBool("isJumping", isJumping);
    }

    public void OnAnimatorMove()
    {
        rootMotion += animator.deltaPosition;
    }

   
    void Jump()
    {
        if (!isJumping)
        {
            float jumpVelocity = Mathf.Sqrt(2 * gravity * jumpHeight);
            SetInAir(jumpVelocity);
        }
    }

    private void SetInAir(float jumpVelocity)
    {
        isJumping = true;
        velocity = animator.velocity * jumpDamp * groundSpeed;
        velocity.y = jumpVelocity;
        animator.SetBool("isJumping", true);
    }

    Vector3 CalculateAircontrol()
    {
        return ((transform.forward * input.y) + (transform.right * input.x)) * (airControl/100);
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody body = hit.collider.attachedRigidbody;

        // no rigidbody
        if (body == null || body.isKinematic)
            return;

        // We dont want to push objects below us
        if (hit.moveDirection.y < -0.3f)
            return;

        // Calculate push direction from move direction,
        // we only push objects to the sides never up and down
        Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);

        // If you know how fast your character is trying to move,
        // then you can also multiply the push velocity by that.

        // Apply the push
        body.velocity = pushDir * pushPower;
    }

    #region Button References
    public void ButtonJump()
    {
        Jump();
    }

    public void ButtonSprinting()
    {
        mobileSprintingButtonPressed = btnScriptTapCounterHelper.ButtonPressCountChecker(mobileSprintingButtonPressed);
    }
    #endregion
}


