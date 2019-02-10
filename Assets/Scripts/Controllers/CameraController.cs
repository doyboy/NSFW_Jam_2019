using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Controller2D))]
public class CameraController : MonoBehaviour
{

    #region Variables
    #region Camera Vars
    Camera cam;

    BoxCollider2D cameraCollider;

    Vector2 cameraTopRight, cameraBotLeft, moveCamDist, cameraDimensions;

    float camBorderLeft, camBorderRight;

    Transform leftBounds, rightBounds;

    bool camRight, camLeft, camTop, camBot;
    #endregion Camera Vars

    #region Player Vars
    BoxCollider2D playerCollider;

    Vector2 playerTopRight, playerBotLeft;
    #endregion Player Vars

    Controller2D controller;

    Player_Doy playerScript;

    Vector3 cameraVelocity;
    #endregion Variables

    // Use this for initialization
    void Start()
    {
        cameraCollider = gameObject.GetComponent<BoxCollider2D>();
        playerCollider = GameObject.FindGameObjectWithTag("Player").GetComponent<BoxCollider2D>();

        playerScript = GameObject.FindGameObjectWithTag("Player").GetComponent<Player_Doy>();

        controller = gameObject.GetComponent<Controller2D>();

        //leftBounds = GameObject.FindGameObjectWithTag("LeftBounds").GetComponent<Transform>();
        //rightBounds = GameObject.FindGameObjectWithTag("RightBounds").GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        ResizeCameraCollider();
        GetColliderPositions();
        DetectPlayerTouchingCamCol();

        //print(playerCollider);
        //print(leftBounds);

        //if (playerCollider.transform.position.x > leftBounds.position.x && playerCollider.transform.position.x < rightBounds.position.x)
            MoveCam();
        //else print("touching border");

        //Paralax();

        //print(camBorderLeft);

        //if (cameraBotLeft.x <= -30f) print("past left border");
    }

    private void ResizeCameraCollider()
    {
        cam = Camera.main;
        //height = 2f * cam.orthographicSize;
        //width = height * cam.aspect;

        cameraDimensions.x = playerCollider.bounds.size.x;
        cameraDimensions.y = playerCollider.bounds.size.y;

        //cameraDimensions.x = width * cameraColliderReduction;
        //cameraDimensions.y = height * cameraColliderReduction;

        cameraCollider.size = cameraDimensions;

        camBorderLeft = cam.transform.position.x - cam.orthographicSize;
        camBorderRight = cam.transform.position.x + cam.orthographicSize;
    }

    private void GetColliderPositions()
    {
        playerTopRight = playerCollider.size / 2 +
                    new Vector2(playerCollider.bounds.center.x, playerCollider.bounds.center.y);

        playerBotLeft = playerCollider.size / 2 -
            new Vector2(playerCollider.bounds.center.x, playerCollider.bounds.center.y);

        playerBotLeft = -playerBotLeft;

        cameraTopRight = cameraCollider.size / 2 +
            new Vector2(cameraCollider.bounds.center.x, cameraCollider.bounds.center.y);

        cameraBotLeft = cameraCollider.size / 2 -
            new Vector2(cameraCollider.bounds.center.x, cameraCollider.bounds.center.y);

        cameraBotLeft = -cameraBotLeft;
    }

    private void DetectPlayerTouchingCamCol()
    {
        camRight = playerTopRight.x >= cameraTopRight.x;
        camLeft = playerBotLeft.x <= cameraBotLeft.x;
        camTop = playerTopRight.y >= cameraTopRight.y;
        camBot = playerBotLeft.y <= cameraBotLeft.y;
    }

    private void MoveCam()
    {
        if (camRight) moveCamDist.x = playerTopRight.x - cameraTopRight.x;
        if (camLeft) moveCamDist.x = playerBotLeft.x - cameraBotLeft.x;
        if (camTop) moveCamDist.y = playerTopRight.y - cameraTopRight.y;
        if (camBot) moveCamDist.y = playerBotLeft.y - cameraBotLeft.y;
        if (!camRight && !camLeft && !camTop && !camBot) moveCamDist = Vector2.zero;

        cam.transform.Translate(moveCamDist.x, moveCamDist.y, Mathf.Epsilon);
    }
}
