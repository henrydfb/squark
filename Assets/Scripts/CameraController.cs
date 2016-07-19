using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour 
{
    private const float MAX_CAMERA_LIMIT = 1;

    /// <summary>
    /// Positive value between 0..1 that determines where is the limit of the camera
    /// </summary>
    public float cameraLimit;

    /// <summary>
    /// 
    /// </summary>
    public Vector3 graphPosition;

    /// <summary>
    /// 
    /// </summary>
    public float graphSize;

    public bool showGraph = false;

    public Material mat;

    //Camera movement
    public float interpVelocity;
    public float minDistance;
    public float followDistance;
    public float speed;
    public GameObject target;
    public Vector3 offset;

    //Player's reference
    protected PlayerController player;

    //Game Controller's reference
    protected GameController gameController;

    //Flag to determine if the camera is moving
    private bool movingCamera;
    //Player's position when is out the limits
    private Vector3 playerEnteredCamera;
    //Camera's position when the player is out the limits
    private Vector3 cameraStartPosition;

    private Vector3[] attentionLevels;

    private bool hooked = false;

    
    private Vector3 mousePos;

    private float averageAttention;

    
    Vector3 targetPos;

	// Use this for initialization
	protected virtual void Start () 
    {
        player = GameObject.FindGameObjectWithTag(Names.Player).GetComponent<PlayerController>();
        gameController = GameObject.Find(Names.GameController).GetComponent<GameController>();
        movingCamera = false;

        attentionLevels = new Vector3[GameController.NUMBER_OF_ATTENTION_POINT];
        for (int i = 0; i < attentionLevels.Length; i++)
            attentionLevels[i] = new Vector3(i * (1.0f / attentionLevels.Length), 0, 0);
        
        if(!gameController.IsGameNeurosky)
            InvokeRepeating("OnUpdateAttention", TGCConnectionController.NEUROSKY_INITIAL_TIME, TGCConnectionController.NEUROSKY_REPEAT_RATE);

        averageAttention = 0;


        targetPos = transform.position;
	}

    protected void OnUpdateAttention()
    {
        for (int i = 0; i < attentionLevels.Length; i++)
        {
            if (i == attentionLevels.Length - 1)
                attentionLevels[i].y = Random.Range(0.0f, 1.0f);
            else
                attentionLevels[i].y = attentionLevels[i + 1].y;
        }
    }

    public void OnUpdateAttention(int value)
    {
        if (attentionLevels != null)
        {
            for (int i = 0; i < attentionLevels.Length; i++)
            {
                if (i == attentionLevels.Length - 1)
                    attentionLevels[i].y = (float)value / 100;
                else
                    attentionLevels[i].y = attentionLevels[i + 1].y;
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 posNoZ = transform.position;
        Vector3 targetDirection;

        if (target)
        {
            if (!player.IsDead())
            {
                posNoZ = transform.position;
                posNoZ.z = target.transform.position.z;

                targetDirection = (target.transform.position - posNoZ);

                interpVelocity = targetDirection.magnitude * speed;

                targetPos = transform.position + (targetDirection.normalized * interpVelocity * Time.deltaTime);

                
                if(((PlayerController)player).IsFacingRight())
                    transform.position = new Vector3(Vector3.Lerp(transform.position, targetPos + offset, 0.25f).x, transform.position.y, transform.position.z);
                else
                    transform.position = new Vector3(Vector3.Lerp(transform.position, targetPos - offset, 0.25f).x, transform.position.y, transform.position.z);
            }
        }
    }

    public float GetAverageAttention()
    {
        return averageAttention;
    }

    void OnPostRender()
    {
        if (!mat)
        {
            Debug.LogError("Please Assign a material on the inspector");
            return;
        }

        if (showGraph)
        {
            GL.PushMatrix();
            mat.SetPass(0);
            GL.LoadOrtho();
            GL.Begin(GL.LINES);
            
            //background grid
            /*GL.Color(Color.white * 0.1f);
            for (int i = 0; i < 10; i++)
            {
                GL.Vertex(new Vector3(i * (float)(1f / 10f), 0, 0));
                GL.Vertex(new Vector3(i * (float)(1f / 10f), 1, 0));
            }*/
            
            GL.Color(Color.white);
            //Horizontal
            GL.Vertex(graphPosition + new Vector3(0, 0, 0) * graphSize);
            GL.Vertex(graphPosition + new Vector3(1, 0, 0) * graphSize);
            GL.Color(Color.white * 0.5f);
            //Middle lines
            for (int i = 1; i <= 10; i++)
            {
                GL.Vertex(graphPosition + new Vector3(0, 0.1f * i, 0) * graphSize);
                GL.Vertex(graphPosition + new Vector3(1, 0.1f * i, 0) * graphSize);
            }

            GL.Color(Color.white);
            //Vertical
            GL.Vertex(graphPosition + new Vector3(0, 0, 0) * graphSize);
            GL.Vertex(graphPosition + new Vector3(0, 1, 0) * graphSize);

            averageAttention = attentionLevels[0].y;
            GL.Color(Color.blue);
            //Attention curve
            for (int i = 1; i < attentionLevels.Length; i++)
            {
                GL.Vertex(graphPosition + attentionLevels[i - 1] * graphSize);
                GL.Vertex(graphPosition + attentionLevels[i] * graphSize);

                averageAttention += attentionLevels[i].y;
            }
            //Average
            GL.Color(Color.green);
            GL.Vertex(graphPosition + new Vector3(0, averageAttention / attentionLevels.Length, 0) * graphSize);
            GL.Vertex(graphPosition + new Vector3(1, averageAttention / attentionLevels.Length, 0) * graphSize);
            
            GL.End();
            GL.PopMatrix();
        }
    }
}
