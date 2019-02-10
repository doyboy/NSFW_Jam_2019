using System.Collections;


using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Controller2D))]
public class Player_Doy : MonoBehaviour
{
    #region Variables
    #region Input Vars
    float horizontalInput, verticalInput;

    bool jumping = false;

    [Range(0, 100)]
    [SerializeField]
    float jumpPower = 50;
    #endregion Input Vars

    #region Physics Vars
    float gravity;

    bool grounded;

    float accelerationTimeGrounded, accelerationTimeAirborne;

    [Range(1, 100)]
    [SerializeField]
    float groundedTraction = 50, airialTraction = 20;

    [Range(1, 300)]
    [SerializeField]
    float fallSpeed = 100;
    #endregion Physics Vars

    #region Speed Vars
    [Range(0.1f, 100f)]
    [SerializeField]
    float speed = 10f;
    #endregion Speed Vars

    //#region Shooting Vars
    //[Range(0.01f, 10f)]
    //[SerializeField]
    //float shootWait = 0.2f;
    //private bool isShooting;
    //float shootTime;
    //private bool justShot;
    //#endregion Shooting Vars

    #region Jizz Vars
    GameObject[] jizzObjs;

    [Range(0.001f, 2f)]
    [SerializeField]
    float jizzLifetime = 1f, jizzOffset = 1f;

    [Range(1f, 60f)]
    [SerializeField]
    float rateOfFire = 20f, jizzSpeed = 20f;

    private float jizzCooldownCurrent = Mathf.Epsilon;
    bool canFireJizz = true;
    #endregion Jizz Vars

    #endregion Variables

    public enum PlayerState {walking, idle, shooting}
    public PlayerState playerState;

    [HideInInspector] public Vector2 velocity;
    Controller2D controller;

    BoxCollider2D playerCol;
    SpriteRenderer sRenderer;

    GameObject collisionObj;

    public Animator animator;

    public bool isFacingRight = true;

    private void Start()
    {
        controller = GetComponent<Controller2D>();
        playerCol = GetComponent<BoxCollider2D>();
        sRenderer = GetComponent<SpriteRenderer>();

        playerCol.size = sRenderer.bounds.size;
        playerState = PlayerState.idle;

        animator = GetComponent<Animator>();

        jizzObjs = GameObject.FindGameObjectsWithTag("Jizz");

        for (int i = 0; i < jizzObjs.Length; i++)
            jizzObjs[i].SetActive(false);
    }

    private void Update()
    {
        BasicMovement();
        DetectJumping();
        ColPhysChecks();
        FlipSprite();
        UpdatePlayerState();
        FireJizz();
    }

    private Vector2 BasicMovement()
    {
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        velocity.x = input.x * speed;

        return input;
    }

    private void DetectJumping()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            if (controller.collisions.below && !jumping)
            {
                jumping = true;
                velocity.y = jumpPower;
            }
        }
    }

    private void FireJizz()
    {
        if (Input.GetKey(KeyCode.Z) && canFireJizz)
        {
            GameObject currentJizzObj;
            Jizz currentJizzScript;

            Vector3 moveAccomidation;

            for (int i = 0; i < jizzObjs.Length; i++)
            {
                jizzObjs[i].SetActive(true);

                if (isFacingRight) moveAccomidation = Vector3.right;
                else moveAccomidation = Vector3.left;

                transform.position += moveAccomidation * jizzOffset;
                currentJizzObj = Instantiate(jizzObjs[i], transform);
                currentJizzObj.transform.parent = null;
                transform.position -= moveAccomidation * jizzOffset;

                currentJizzScript = currentJizzObj.GetComponent<Jizz>();
                currentJizzScript.enabled = true;

                currentJizzScript.jizzLifetime = jizzLifetime;
                currentJizzScript.jizzSpeed = jizzSpeed;

                jizzObjs[i].SetActive(false);
            }

            canFireJizz = false;
        }

        if (!canFireJizz)
        {
            jizzCooldownCurrent += Time.deltaTime;

            if (jizzCooldownCurrent >= 1 / rateOfFire)
            {
                canFireJizz = true;
                jizzCooldownCurrent = Mathf.Epsilon;
            }
        }
    }

    private void ColPhysChecks()
    {
        grounded = controller.collisions.below;

        gravity = -fallSpeed * 2f;
        accelerationTimeGrounded = 1f / groundedTraction;
        accelerationTimeAirborne = 1f / airialTraction;

        velocity.y += gravity * Time.deltaTime;

        if (controller.collisions.isAirborne()) jumping = false;

        if ((controller.collisions.below && !jumping) ||
            controller.collisions.above)
        {
            velocity.y = 0;
        }
        
        controller.Move(velocity * Time.deltaTime);

        print(controller.collisions.isAirborne());
    }

    private void FlipSprite()
    {
        if (velocity.x > 0)
        {
            isFacingRight = true;
            sRenderer.flipX = false;
        }
        else if (velocity.x < 0) 
        { 
            sRenderer.flipX = true;
            isFacingRight = false;
        }
    }

    private void UpdatePlayerState()
    {
        if (!canFireJizz)
        {
            playerState = PlayerState.shooting;
            animator.SetTrigger("isShooting");
            animator.Play("DickShoot", 0, 0.5f);
        }
        else if (Mathf.Abs(velocity.x) > 0)
        {
            playerState = PlayerState.walking;
            animator.SetBool("isWalking", true);
        }
        else
        {
            playerState = PlayerState.idle;
            animator.SetBool("isWalking", false);
        }
    }
}
