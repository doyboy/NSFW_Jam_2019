using System.Collections;


using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Controller2D))]
public class Player_MoToKo : MonoBehaviour
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

    #region Shooting Vars
    [Range(0.01f, 10f)]
    [SerializeField]
    float shootWait = 0.2f;
    private bool isShooting;
    float shootTime;
    private bool justShot;
    #endregion Shooting Vars


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

    }

    private void Update()
    {
        BasicMovement();
        DetectJumping();
        ColPhysChecks();
        FlipSprite();
        Shooting();
        updatePlayerState();
    }

    private Vector2 BasicMovement()
    {
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        velocity.x = input.x * speed;

        return input;
    }

    private void DetectJumping()
    {
        if (Input.GetKey(KeyCode.UpArrow))
        {
            if (controller.collisions.below && !jumping)
            {
                jumping = true;
                velocity.y = jumpPower;
            }
        }
    }

    private void Shooting(){
        if (justShot){
            justShot = false;
        }
        if (Input.GetKey(KeyCode.Space))
        {
            if (!isShooting)
            {
                isShooting = true;
                shootTime = 0f;
                justShot = true;

            }
        }
        if (isShooting)
        {
            shootTime += Time.deltaTime;

            if (shootTime > shootWait)
            {
                isShooting = false;
                shootTime = 0f;
            }
        }

        if (justShot){
            //taking jizz out of pool to shoot
            GameObject obj = NewObjectPullerScript.current.GetPooledObject();
            if (obj == null) return;
            obj.SetActive(true);
            obj.transform.position = transform.position;
            obj.transform.rotation = transform.rotation;
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

    private void updatePlayerState(){
        if (justShot)
        {
            playerState = PlayerState.shooting;
            animator.SetTrigger("isShooting");
        } else if (Mathf.Abs(velocity.x)>0){
            playerState = PlayerState.walking;
            animator.SetBool("isWalking", true);
        } else{
            playerState = PlayerState.idle;
            animator.SetBool("isWalking", false);
        }

    }
}
