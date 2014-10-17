using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    private const float MAX_AXIS_STEP = 1;
    private const float MIN_AXIS_STEP = -1;
    //How fast will you start moving the player: from MIN_AXIS_STEP to MAX_AXIS_STEP
    private const float HOR_AXIS_STEP = 0.05f;

    /// <summary>
    /// How fast the player will run
    /// </summary>
    public float speed;
    /// <summary>
    /// How high will the player jump
    /// </summary>
    public float jumpImpulse;

    //Flag to know if the player is jumping, when he presses the button
    private bool isJumping;
    //Flag to know if the player is in the air (different from jumping)
    private bool isInAir;
    //A value to do the horizontal movement smooth
    private float horAxis;
    //Previous horizontal movement (to know if the player changed directions)
    private float prevHorAxis;

    //Collision flags
    private bool isCollidingDown;
    private bool isCollidingLeft;
    private bool isCollidingRight;

    //Colliders
    private Collider2D downCollider;
    private Collider2D leftCollider;
    private Collider2D rightCollider;

    TGCConnectionController controller;
    private int poorSignal1;
    private int attention1;
    private int meditation1;
    private int blink;
    private int indexSignalIcons = 1;
    private float delta;
    private GameController gameController;

	// Use this for initialization
	void Start () 
    {
        isJumping = false;
        isInAir = false;
        isCollidingDown = false;
        isCollidingLeft = false;
        isCollidingRight = false;
        horAxis = 0;

        controller = GameObject.Find("NeuroSkyTGCController").GetComponent<TGCConnectionController>();
        gameController = GameObject.Find("GameController").GetComponent<GameController>();

        controller.UpdatePoorSignalEvent += OnUpdatePoorSignal;
        controller.UpdateAttentionEvent += OnUpdateAttention;
        controller.UpdateMeditationEvent += OnUpdateMeditation;
        controller.UpdateBlinkEvent += OnUpdateBlink;

        controller.UpdateDeltaEvent += OnUpdateDelta;
	}

    void OnUpdatePoorSignal(int value)
    {
        poorSignal1 = value;
        if (value < 25)
        {
            indexSignalIcons = 0;
        }
        else if (value >= 25 && value < 51)
        {
            indexSignalIcons = 4;
        }
        else if (value >= 51 && value < 78)
        {
            indexSignalIcons = 3;
        }
        else if (value >= 78 && value < 107)
        {
            indexSignalIcons = 2;
        }
        else if (value >= 107)
        {
            indexSignalIcons = 1;
        }
    }
    void OnUpdateAttention(int value)
    {
        attention1 = value;
    }
    void OnUpdateMeditation(int value)
    {
        meditation1 = value;
    }

    void OnUpdateBlink(int value)
    {
        blink = value;
        if(!isInAir)
            Jump();
        Debug.Log("blink!");
    }
    void OnUpdateDelta(float value)
    {
        delta = value;
    }

	// Update is called once per frame
	void Update () 
    {
        if (gameController.IsGameOver())
            rigidbody2D.velocity = Vector3.zero;
        else
        {
            HandleInput();
            //Clamp player's X position
            if (transform.position.x + renderer.bounds.size.x / 2 >= gameController.rightLimit || transform.position.x - renderer.bounds.size.x / 2 <= gameController.leftLimit)
            {
                if (transform.position.x + renderer.bounds.size.x / 2 >= gameController.rightLimit)
                    transform.position = new Vector3(gameController.rightLimit - renderer.bounds.size.x / 2, transform.position.y);
                if (transform.position.x - renderer.bounds.size.x / 2 <= gameController.leftLimit)
                    transform.position = new Vector3(gameController.leftLimit + renderer.bounds.size.x / 2, transform.position.y);
            }
        }
	}

    void OnGUI()
    {
        GUILayout.Label("PoorSignal1:" + poorSignal1);
        GUILayout.Label("Attention1:" + attention1);
        GUILayout.Label("Meditation1:" + meditation1);
        GUILayout.Label("Blink:" + blink);
        GUILayout.Label("Delta:" + delta);
    }
    
    /// <summary>
    /// Handles the input values and use them to move the player
    /// </summary>
    private void HandleInput()
    {
        //Moving Right
        if (Input.GetAxis(Names.HorizontalInput) > 0)
        {
            if (isCollidingLeft)
                horAxis = 0;
            else
            {
                horAxis = horAxis < 1 ? horAxis + HOR_AXIS_STEP : MAX_AXIS_STEP;
                if (Input.GetAxis(Names.HorizontalInput) < prevHorAxis)
                    horAxis = horAxis / 2;
            }
        }

        //Moving Left
        if (Input.GetAxis(Names.HorizontalInput) < 0)
        {
            if (isCollidingRight)
                horAxis = 0;
            else
            {
                horAxis = horAxis > -1 ? horAxis - HOR_AXIS_STEP : MIN_AXIS_STEP;
                if (Input.GetAxis(Names.HorizontalInput) > prevHorAxis)
                    horAxis = horAxis / 2;
            }
        }

        //Not moving
        if (Input.GetAxis(Names.HorizontalInput) == 0)
            horAxis = 0;

        //Update horizontal movement
        rigidbody2D.velocity = new Vector2(horAxis * speed, rigidbody2D.velocity.y);

        //Jump button pressed
        if (Input.GetButtonDown(Names.JumpInput) && !isJumping && !isInAir)
            Jump();

        //Jump button button released
        if (Input.GetButtonUp(Names.JumpInput) && isJumping)
        {
            rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, rigidbody2D.velocity.y/2);
            isJumping = false;
        }

        //Previous horizontal axis value
        prevHorAxis = Input.GetAxis(Names.HorizontalInput);
    }

    private void Jump()
    {
        rigidbody2D.AddForce(new Vector2(0, jumpImpulse));
        isJumping = true;
        isInAir = true;
    }

    public void Die()
    {
        rigidbody2D.gravityScale = 0;
        rigidbody2D.velocity = Vector3.zero;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.collider.tag == Names.Platform)
        {
            //Down collision
            if (transform.position.y - collider2D.bounds.size.y / 2 > col.transform.position.y + col.collider.bounds.size.y / 2)
            {
                downCollider = col.collider;
                isCollidingDown = true;
                isInAir = false;
            }

            //Right collision
            if (transform.position.x - collider2D.bounds.size.x / 2 > col.transform.position.x + col.collider.bounds.size.x / 2)
            {
                rightCollider = col.collider;
                isCollidingRight = true;
            }

            //Left collision
            if (transform.position.x + collider2D.bounds.size.x / 2 < col.transform.position.x - col.collider.bounds.size.x / 2)
            {
                leftCollider = col.collider;
                isCollidingLeft = true;
            }
        }
    }

    void OnCollisionExit2D(Collision2D col)
    {
        //Down collision
        if (downCollider != null)
        {
            if (col.collider == downCollider)
            {
                if (col.gameObject.tag == Names.Platform)
                {
                    isInAir = true;
                    isCollidingDown = false;
                    downCollider = null;
                }
            }
        }

        //Left collision
        if (leftCollider != null)
        {
            if (col.collider == leftCollider)
            {
                isCollidingLeft = false;
                leftCollider = null;
            }
        }

        //Right collision
        if (rightCollider != null)
        {
            if (col.collider == rightCollider)
            {
                isCollidingRight = false;
                rightCollider = null;
            }
        }
    }
}
