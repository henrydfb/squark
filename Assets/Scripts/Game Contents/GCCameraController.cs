using UnityEngine;
using System.Collections;

public class GCCameraController : MonoBehaviour 
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

    private bool hooked = false;


    public Material mat;
    private Vector3 startVertex;
    private Vector3 mousePos;

    //Camera movement
    public float interpVelocity;
    public float minDistance;
    public float followDistance;
    public GameObject target;
    public Vector3 offset;
    Vector3 targetPos;

	// Use this for initialization
	void Start () 
    {
        player = GameObject.FindGameObjectWithTag(Names.Player).GetComponent<PlayerController>();
        gameController = GameObject.Find(Names.GameController).GetComponent<GameController>();
        movingCamera = false;
        targetPos = transform.position;
	}

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 posNoZ = transform.position;
        Vector3 targetDirection, lerp;
        PlayerController player;
        if (target)
        {
            player = target.GetComponent<PlayerController>();

            if (!player.IsDead())
            {
                posNoZ = transform.position;
                posNoZ.z = target.transform.position.z;

                targetDirection = (target.transform.position - posNoZ);

                interpVelocity = targetDirection.magnitude * 5f;

                targetPos = transform.position + (targetDirection.normalized * interpVelocity * Time.deltaTime);

                lerp = Vector3.Lerp(transform.position, player.transform.position, 0.25f);// targetPos + offset, 0.25f);
                transform.position = new Vector3(lerp.x,transform.position.y,transform.position.z);
            }
        }
    }
}
