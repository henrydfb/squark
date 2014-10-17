using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour 
{
    private const float MAX_CAMERA_LIMIT = 1;

    /// <summary>
    /// Positive value between 0..1 that determines where is the limit of the camera
    /// </summary>
    public float cameraLimit;

    //Player's reference
    private PlayerController player;
    //Flag to determine if the camera is moving
    private bool movingCamera;
    //Player's position when is out the limits
    private Vector3 playerEnteredCamera;
    //Camera's position when the player is out the limits
    private Vector3 cameraStartPosition;

    private GameController gameController;

	// Use this for initialization
	void Start () {
        player = GameObject.FindGameObjectWithTag(Names.Player).GetComponent<PlayerController>();
        gameController = GameObject.Find(Names.GameController).GetComponent<GameController>();
        movingCamera = false;
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 playerPos, cameraPos, leftMinLimit,rightMinLimit, cameraTransform;

        playerPos = Camera.main.WorldToViewportPoint(player.transform.position);
        leftMinLimit = Camera.main.WorldToScreenPoint(new Vector3(gameController.leftLimit, gameController.downLimit));
        rightMinLimit = Camera.main.WorldToScreenPoint(new Vector3(gameController.rightLimit, gameController.downLimit));

        //The player is outside the limits and is moving towards the right direction
        if ((playerPos.x > cameraLimit && player.rigidbody2D.velocity.x > 0) || (playerPos.x < (MAX_CAMERA_LIMIT - cameraLimit) && player.rigidbody2D.velocity.x < 0))
        {
            if (!movingCamera)
            {
                cameraStartPosition = Camera.main.transform.position;
                playerEnteredCamera = player.transform.position;
                movingCamera = true;
            }
        }
        else
            movingCamera = false;

        //Move the camera with the player
        if (movingCamera)
            cameraPos = new Vector3(cameraStartPosition.x + (player.transform.position.x - playerEnteredCamera.x), cameraStartPosition.y, cameraStartPosition.z);
        else
            cameraPos = Camera.main.transform.position;

        //Check if the camera passes the limit (left and right) on the level
        cameraTransform = Camera.main.WorldToScreenPoint(cameraPos);
        if (cameraTransform.x - Camera.main.pixelWidth / 2 <= leftMinLimit.x)
            cameraPos.x = Camera.main.ScreenToWorldPoint(new Vector2(leftMinLimit.x + Camera.main.pixelWidth / 2,0)).x;

        if (cameraTransform.x + Camera.main.pixelWidth / 2 >= rightMinLimit.x)
            cameraPos.x = Camera.main.ScreenToWorldPoint(new Vector2(rightMinLimit.x - Camera.main.pixelWidth / 2, 0)).x;

        //Updates position
        Camera.main.transform.position = cameraPos;
	}
}
