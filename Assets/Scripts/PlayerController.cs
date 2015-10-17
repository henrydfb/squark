using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    protected const float MAX_AXIS_STEP = 1;
    protected const float MIN_AXIS_STEP = -1;
    //How fast will you start moving the player: from MIN_AXIS_STEP to MAX_AXIS_STEP
    protected const float HOR_AXIS_STEP = 0.05f;

    /// <summary>
    /// How fast the player will run
    /// </summary>
    public float speed;
    /// <summary>
    /// How high will the player jump
    /// </summary>
    public float jumpImpulse;

    public AudioClip killEnemySound;
    public AudioClip dieSound;

    //Flag to know if the player is jumping, when he presses the button
    protected bool isJumping;
    //Flag to know if the player is in the air (different from jumping)
    protected bool isInAir;
    //A value to do the horizontal movement smooth
    protected float horAxis;
    //Previous horizontal movement (to know if the player changed directions)
    protected float prevHorAxis;

    //Collision flags
    protected bool isCollidingDown;
    protected bool isCollidingLeft;
    protected bool isCollidingRight;

    //Colliders
    protected Collider2D downCollider;
    protected Collider2D leftCollider;
    protected Collider2D rightCollider;

    protected GameController gameController;


    private bool isDead;

    protected float prevVelY;
    protected float prevY;
    public float iniX;

    protected float jumpTime;
    protected float jumpHeight;

    protected GameObject leftColObj;
    protected GameObject rightColObj;
    protected AudioSource audioSource;

	// Use this for initialization
	protected virtual void Start () 
    {
        isJumping = false;
        isInAir = false;
        isCollidingDown = false;
        isCollidingLeft = false;
        isCollidingRight = false;
        horAxis = 0;
        isDead = false;

        gameController = GameObject.Find(Names.GameController).GetComponent<GameController>();

        jumpTime = 0.0f;

        audioSource = GetComponent<AudioSource>();
	}

	// Update is called once per frame
    protected virtual void Update() 
    {
        if (gameController.IsGameRunning())
        {
            if (gameController.IsGameOver())
                GetComponent<Rigidbody2D>().velocity = Vector3.zero;
            else
            {
                HandleInput();
                //Clamp player's X position
                /*if (transform.position.x + renderer.bounds.size.x / 2 >= gameController.rightLimit || transform.position.x - renderer.bounds.size.x / 2 <= gameController.leftLimit)
                {
                    if (transform.position.x + renderer.bounds.size.x / 2 >= gameController.rightLimit)
                        transform.position = new Vector3(gameController.rightLimit - renderer.bounds.size.x / 2, transform.position.y);
                    if (transform.position.x - renderer.bounds.size.x / 2 <= gameController.leftLimit)
                    {
                        transform.position = new Vector3(gameController.leftLimit + renderer.bounds.size.x / 2, transform.position.y);
                        iniX = transform.position.x;
                    }
                }*/
            }
        }
	}

    /// <summary>
    /// Handles the input values and use them to move the player
    /// </summary>
    protected virtual void HandleInput()
    {}

    public void Jump()
    {
        if (!isInAir)
        {
            GetComponent<Rigidbody2D>().AddForce(new Vector2(0, jumpImpulse));
            isJumping = true;
            isInAir = true;
            prevVelY = GetComponent<Rigidbody2D>().velocity.y;
            prevY = gameObject.transform.position.y;
            //print("ini: " + prevY);
            jumpTime = 0.0f;
        }
    }

    public bool IsDead()
    {
        return isDead;
    }

    public virtual void Die()
    {
        //rigidbody2D.gravityScale *= 2;
        
        GetComponent<Rigidbody2D>().velocity = Vector3.zero;
        GetComponent<Rigidbody2D>().AddForce(new Vector2(0, jumpImpulse/2));
        isDead = true;
        GetComponent<Collider2D>().enabled = false;
        audioSource.PlayOneShot(dieSound);
    }

    public void KillEnemy()
    {
        GetComponent<Rigidbody2D>().velocity = new Vector2(GetComponent<Rigidbody2D>().velocity.x,0);
        GetComponent<Rigidbody2D>().AddForce(new Vector2(0, jumpImpulse/2));
        audioSource.PlayOneShot(killEnemySound);
    }

    public void TouchPlatformDown()
    {
        isCollidingDown = true;
        isInAir = false;

        //print("height: " + jumpHeight + " time: " + jumpTime);
        iniX = transform.position.x;
    }

    public void UnTouchPlatformDown()
    {
        isCollidingDown = false;
        isInAir = true;
    }

    protected virtual void OnCollisionEnter2D(Collision2D col)
    {
        PlatformController platform;
        RhythmFactory factory;

        if (col.collider.tag == Names.Platform)
        {
            isInAir = false;
            GetComponent<Rigidbody2D>().velocity = new Vector2(GetComponent<Rigidbody2D>().velocity.x, 0);
            GetComponent<Animator>().Play("idle");
            platform = col.collider.GetComponent<PlatformController>();

            //Down collision
            /*if (transform.position.y - collider2D.bounds.size.y / 2 > col.transform.position.y + col.collider.bounds.size.y / 2)
            {
                downCollider = col.collider;
                isCollidingDown = true;
                isInAir = false;

                print("height: " + jumpHeight + " time: " + jumpTime);
            }*/

            //Right collision
            /*if (transform.position.x - collider2D.bounds.size.x / 2 > col.transform.position.x + col.collider.bounds.size.x / 2)
            {
                rightCollider = col.collider;
                isCollidingRight = true;
            }

            //Left collision
            if (transform.position.x + collider2D.bounds.size.x / 2 < col.transform.position.x - col.collider.bounds.size.x / 2)
            {
                leftCollider = col.collider;
                isCollidingLeft = true;
            }*/

            //Create the new rhythm
            if (platform.GetLast())
            {
                Debug.Log("Generatenew rhythm");
                /*factory = GameObject.Find(Names.RhythmFactory).GetComponent<RhythmFactory>();
                if (factory != null)
                {
                    factory.GenerateRhythm();
                    platform.SetLast(false);
                }*/
            }
        }
        else if (col.collider.tag == Names.Enemy)
        {
            Debug.Log("Died!");
            Die();
        }
    }

    protected virtual void OnCollisionExit2D(Collision2D col)
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

    public void SetCollidingLeft(bool collidingLeft, GameObject gameObj)
    {
        this.isCollidingLeft = collidingLeft;

        if (collidingLeft)
            leftColObj = gameObj;
        else
            leftColObj = null;
    }

    public void SetCollidingRight(bool collidingRight, GameObject gameObj)
    {
        this.isCollidingRight = collidingRight;

        if (collidingRight)
            rightColObj = gameObj;
        else
            rightColObj = null;
    }
}
