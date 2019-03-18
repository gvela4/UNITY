using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class FPSController : NetworkBehaviour
{
    private Transform firstPerson_view;
    private Transform firstPerson_camera;
    private Vector3 firstPerson_view_rotation = Vector3.zero;

    public float walkSpeed = 6.75f;
    public float runSpeed = 10f;
    public float crouchSpeed = 4f;
    public float jumpSpeed = 8f;
    public float gravity = 20f;

    private float speed;
    private bool isMoving, isGrounded, isCrouching;
    private float inputX, inputY;
    private float inputX_Set, inputY_Set;
    private float inputModifyFactor;

    private bool limitDiagonalSpeed = true;
    private float antiBumpFactor = 0.75f;

    private CharacterController charController; // to move the player
    private Vector3 moveDirection = Vector3.zero; // where to move the player

    public LayerMask groundLayer; // to see which layer are we on
    private float rayDistance; // to see how far we are from the ground
    private float default_ControllerHeight;
    private Vector3 default_CamPos;
    private float camHeight;

    private FPSPlayerAnimations playerAnimation;

    [SerializeField]
    private WeaponManager weapon_manager;

    private FPSWeapon current_weapon;
    private float fireRate = 15f;
    private float nextTimeToFire = 0f;

    [SerializeField]
    private WeaponManager handsWeapon_Manager;
    private FPSHandsWeapon current_Hands_Weapon;

    public GameObject playerHolder, weaponsHolder;
    public GameObject[] weapons_FPS;
    private Camera mainCam;
    public FPSMouseLook[] mouseLook;


	// Use this for initialization
	void Start ()
    {
        // same as GameOject.Find("FPS View") goes inside the Hierarchy/scene
        firstPerson_view = transform.Find("FPS View").transform; // goes inside children of parent game object
        charController = GetComponent<CharacterController>();
        speed = walkSpeed;
        isMoving = false;

        rayDistance = charController.height * 0.5f + charController.radius; // half of the character controller height
        default_ControllerHeight = charController.height;
        default_CamPos = firstPerson_view.localPosition;

        playerAnimation = GetComponent<FPSPlayerAnimations>();

        weapon_manager.weapons[0].SetActive(true);
        current_weapon = weapon_manager.weapons[0].GetComponent<FPSWeapon>();

        handsWeapon_Manager.weapons[0].SetActive(true);
        current_Hands_Weapon = handsWeapon_Manager.weapons[0].GetComponent<FPSHandsWeapon>();

        if (isLocalPlayer)
        {
            playerHolder.layer = LayerMask.NameToLayer("Player");

            foreach (Transform child in playerHolder.transform)
            {
                child.gameObject.layer = LayerMask.NameToLayer("Player");
            }
            for (int i = 0; i < weapons_FPS.Length; i++)
            {
                weapons_FPS[i].layer = LayerMask.NameToLayer("Player");
            }

            weaponsHolder.layer = LayerMask.NameToLayer("Enemy");

            foreach (Transform child in weaponsHolder.transform)
            {
                child.gameObject.layer = LayerMask.NameToLayer("Enemy");
            }
        }

        if (!isLocalPlayer)
        {
            playerHolder.layer = LayerMask.NameToLayer("Enemy");

            foreach (Transform child in playerHolder.transform)
            {
                child.gameObject.layer = LayerMask.NameToLayer("Enemy");
            }
            for (int i = 0; i < weapons_FPS.Length; i++)
            {
                weapons_FPS[i].layer = LayerMask.NameToLayer("Enemy");
            }

            weaponsHolder.layer = LayerMask.NameToLayer("Player");

            foreach (Transform child in weaponsHolder.transform)
            {
                child.gameObject.layer = LayerMask.NameToLayer("Player");
            }
        }

        if (!isLocalPlayer)
        {
            for (int i = 0; i < mouseLook.Length; i++)
            {
                mouseLook[i].enabled = false;
            }
        }

        mainCam = transform.Find("FPS View").Find("FPS Camera").GetComponent<Camera>();
        mainCam.gameObject.SetActive(false);

    }

    public override void OnStartLocalPlayer()
    {
        tag = "Player";
    }

    // Update is called once per frame
    void Update ()
    {
        if (isLocalPlayer)
        {
            if (!mainCam.gameObject.activeInHierarchy)
            {
                mainCam.gameObject.SetActive(true);
            }
        }
        if (!isLocalPlayer)
        {
            // if true, and we execute the return code
            // all code that is written below the return
            // will not be executed.
            return;
        }
        PlayerMovement();
        SelectWeapon();

    }

    void PlayerMovement()
    {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S))
        {
            if (Input.GetKey(KeyCode.W)) // UP
            {
                inputY_Set = 1f;
            }
            else
            { // DOWN
                inputY_Set = -1f;
            }
        }
        else
        {
            inputY_Set = 0f;
        }

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
            if (Input.GetKey(KeyCode.A)) // moving to the left
            {
                inputX_Set = -1f;
            }
            else
            {
                inputX_Set = 1f; // moving to the right
            }
        }
        else
        {
            inputX_Set = 0f;
        }
        // goes from inputY to inputY_Set in 19f time
        inputY = Mathf.Lerp(inputY, inputY_Set, Time.deltaTime * 19f);
        inputX = Mathf.Lerp(inputX, inputX_Set, Time.deltaTime * 19f);

        // to limit diagonal speed
        inputModifyFactor = Mathf.Lerp(inputModifyFactor,
            (inputY_Set != 0 && inputX_Set != 0 && limitDiagonalSpeed) ? 0.75f : 1.0f, 
            Time.deltaTime * 19f);

        firstPerson_view_rotation = Vector3.Lerp(firstPerson_view_rotation, Vector3.zero,
            Time.fixedDeltaTime * 5f);
        // localEulerAngles will be relative to the parent gameobject
        // it will move with the parent gameobject // VR
        firstPerson_view.localEulerAngles = firstPerson_view_rotation;

        if (isGrounded)
        { // gonna call crouch and sprint
            PlayerCrouchingAndSprinting();

            moveDirection = new Vector3(inputX * inputModifyFactor, -antiBumpFactor, inputY * inputModifyFactor);
            moveDirection = transform.TransformDirection(moveDirection) * speed;

            // gonna call jump
            PlayerJump();
        }

        moveDirection.y -= gravity * Time.deltaTime; // moving down

        isGrounded = (charController.Move(moveDirection * Time.deltaTime) & CollisionFlags.Below) != 0;
        isMoving = charController.velocity.magnitude > 0.15f; // magnitude returns length of vector

        HandleAnimations();
    }

    void PlayerCrouchingAndSprinting()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (!isCrouching)
            {
                isCrouching = true;
            }
            else
            {
                if (CanGetUp())
                {
                    isCrouching = false;
                }
            }

            StopCoroutine(MoveCameraCrouch());
            StartCoroutine(MoveCameraCrouch());
        }
        if (isCrouching)
        {
            speed = crouchSpeed;
        }
        else
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                speed = runSpeed;
            }
            else
            {
                speed = walkSpeed;
            }
        }

        playerAnimation.PlayerCrouch(isCrouching);
    }

    bool CanGetUp()
    {
        Ray groundRay = new Ray(transform.position, transform.up);
        RaycastHit groundHit;

        if (Physics.SphereCast(groundRay, charController.radius + 0.05f, out groundHit, rayDistance, groundLayer))
        {
            if (Vector3.Distance(transform.position, groundHit.point) < 2.3f)
            {
                return false;
            }
        }
        return true;
    }

    IEnumerator MoveCameraCrouch()
    {
        charController.height = isCrouching ? default_ControllerHeight / 1.5f : default_ControllerHeight;

        charController.center = new Vector3(0f, charController.height / 2f, 0f);

        camHeight = isCrouching ? default_CamPos.y / 1.5f : default_CamPos.y;

        // absolute value of camheight
        while (Mathf.Abs(camHeight - firstPerson_view.localPosition.y) > 0.01f)
        {
            firstPerson_view.localPosition = Vector3.Lerp(firstPerson_view.localPosition,
                new Vector3(default_CamPos.x, camHeight, default_CamPos.z), Time.deltaTime * 11f);

            yield return null;
        }
    }

    void PlayerJump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isCrouching)
            {
                if (CanGetUp())
                {
                    isCrouching = false;

                    playerAnimation.PlayerCrouch(isCrouching);

                    StopCoroutine(MoveCameraCrouch());
                    StartCoroutine(MoveCameraCrouch());
                }
            }
            else
            {
                moveDirection.y = jumpSpeed;
            }
        }
    }

    void HandleAnimations()
    {
        playerAnimation.Movement(charController.velocity.magnitude);
        playerAnimation.PlayerJump(charController.velocity.y);

        if (isCrouching && charController.velocity.magnitude > 0f)
        {
            playerAnimation.PlayerCrouchWalk(charController.velocity.magnitude);
        }

        // shooting
        if (Input.GetMouseButtonDown(0) && Time.time > nextTimeToFire)
        {
            nextTimeToFire = Time.time + 1f / fireRate; // time between shoots

            if (isCrouching)
            {
                playerAnimation.Shoot(false);
            }
            else
            {
                playerAnimation.Shoot(true);
            }

            current_weapon.Shoot();
            current_Hands_Weapon.Shoot();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            playerAnimation.Reload();
            current_Hands_Weapon.Reload();
        }
    }

    void SelectWeapon()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (!handsWeapon_Manager.weapons[0].activeInHierarchy)
            {
                for (int i = 0; i < handsWeapon_Manager.weapons.Length; i++)
                {
                    handsWeapon_Manager.weapons[i].SetActive(false);
                }
                current_Hands_Weapon = null;
                handsWeapon_Manager.weapons[0].SetActive(true);
                current_Hands_Weapon = handsWeapon_Manager.weapons[0].GetComponent<FPSHandsWeapon>();
            }

            if (!weapon_manager.weapons[0].activeInHierarchy)
            {
                for (int i = 0; i < weapon_manager.weapons.Length; i++)
                {
                    weapon_manager.weapons[i].SetActive(false);
                }
                current_weapon = null;
                weapon_manager.weapons[0].SetActive(true);
                current_weapon = weapon_manager.weapons[0].GetComponent<FPSWeapon>();

                playerAnimation.ChangeController(true);
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (!handsWeapon_Manager.weapons[1].activeInHierarchy)
            {
                for (int i = 0; i < handsWeapon_Manager.weapons.Length; i++)
                 {
                     handsWeapon_Manager.weapons[i].SetActive(false);
                 }
                current_Hands_Weapon = null;
                handsWeapon_Manager.weapons[1].SetActive(true);
                current_Hands_Weapon = handsWeapon_Manager.weapons[1].GetComponent<FPSHandsWeapon>();
            }

            if (!weapon_manager.weapons[1].activeInHierarchy)
            {
                for (int i = 0; i < weapon_manager.weapons.Length; i++)
                {
                    weapon_manager.weapons[i].SetActive(false);
                }
                current_weapon = null;
                weapon_manager.weapons[1].SetActive(true);
                current_weapon = weapon_manager.weapons[1].GetComponent<FPSWeapon>();

                playerAnimation.ChangeController(false);
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (!handsWeapon_Manager.weapons[2].activeInHierarchy)
            {
                for (int i = 0; i < handsWeapon_Manager.weapons.Length; i++)
                 {
                     handsWeapon_Manager.weapons[i].SetActive(false);
                 }
                current_Hands_Weapon = null;
                handsWeapon_Manager.weapons[2].SetActive(true);
                current_Hands_Weapon = handsWeapon_Manager.weapons[2].GetComponent<FPSHandsWeapon>();
            }

            if (!weapon_manager.weapons[2].activeInHierarchy)
            {
                for (int i = 0; i < weapon_manager.weapons.Length; i++)
                {
                    weapon_manager.weapons[i].SetActive(false);
                }
                current_weapon = null;
                weapon_manager.weapons[2].SetActive(true);
                current_weapon = weapon_manager.weapons[2].GetComponent<FPSWeapon>();

                playerAnimation.ChangeController(false);
            }
        }
    }
}
