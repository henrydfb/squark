using UnityEngine;
using System.Collections;
using UnityEngine.UI;

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
    public bool autoRunning = false;
    public AudioClip jumpSound;

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
    private float lastSpikePosX;
    private bool isFacingRight;
    SimpleSpikeController closestSpike;
    private bool stoppedMov;
    private bool startMoving;
    protected bool isHit;
    
    protected float hitTimer;

    protected int hits;

	// Use this for initialization
	protected virtual void Start () 
    {
        isJumping = false;
        isInAir = false;
        isCollidingDown = false;
        isCollidingLeft = false;
        isCollidingRight = false;
        isHit = false;
        horAxis = 0;
        isDead = false;
        isFacingRight = true;
        stoppedMov = false;
        startMoving = false;
        hits = 0;
        //gameController = GameObject.Find(Names.GameController).GetComponent<GameController>();
        
        jumpTime = 0.0f;
        hitTimer = 0.0f;

        audioSource = GetComponent<AudioSource>();
	}

    public bool IsFacingRight()
    {
        return isFacingRight;
    }

    protected virtual void UpdateSpikeAttention(Jump.HeightType jumpType)
    {
    }

    protected virtual void UpdateEnemyDeath(Jump.HeightType jumpType)
    {
    }

    protected virtual void UpdateEnemySpike(Jump.HeightType jumpType)
    {
    }

	// Update is called once per frame
    protected virtual void Update() 
    {
        SimpleSpikeController tempSpike;
        GameObject[] enemies;
        GameObject[] spikes;
        GameObject downCol;

        if (gameController != null)
        {
            if (gameController.IsGameRunning())
            {
                if (gameController.IsGameOver())
                    GetComponent<Rigidbody2D>().velocity = Vector3.zero;
                else
                {
                    HandleInput();

                    //Spike Enemies
                    tempSpike = FindClosestSpike();

                    if (tempSpike != null)
                    {
                        if (closestSpike == tempSpike)
                        {
                            if (transform.position.x - tempSpike.transform.position.x > 0 && lastSpikePosX < 0 || transform.position.x - tempSpike.transform.position.x < 0 && lastSpikePosX > 0)
                            {
                                UpdateSpikeAttention(tempSpike.jumpType);
                            }
                        }

                        lastSpikePosX = transform.position.x - tempSpike.transform.position.x;
                    }

                    closestSpike = tempSpike;

                    if (isHit)
                    {
                        if (hitTimer > 3)
                        {
                            enemies = GameObject.FindGameObjectsWithTag(Names.Enemy);
                            spikes = GameObject.FindGameObjectsWithTag(Names.Spike);
                            downCol = (GameObject)GameObject.Find("downCollider");

                            foreach (GameObject e in enemies)
                            {
                                Physics2D.IgnoreCollision(GetComponent<Collider2D>(), e.GetComponent<Collider2D>(), false);
                                Physics2D.IgnoreCollision(downCol.GetComponent<Collider2D>(), e.GetComponent<Collider2D>(), false);
                            }

                            foreach (GameObject s in spikes)
                            {
                                Physics2D.IgnoreCollision(GetComponent<Collider2D>(), s.GetComponent<Collider2D>(), false);
                                Physics2D.IgnoreCollision(downCol.GetComponent<Collider2D>(), s.GetComponent<Collider2D>(), false);
                            }

                            isHit = false;
                            GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
                            hitTimer = 0.0f;
                        }
                        else
                            hitTimer += Time.deltaTime;
                    }
                }
            }
        }
	}

    private SimpleSpikeController FindClosestSpike()
    {
        PlayerController player;
        GameObject[] spikeObjects;
        SimpleSpikeController spike;
        float distance;

        player = gameObject.GetComponentInParent<PlayerController>();
        spike = null;
        distance = Mathf.Infinity;

        spikeObjects = GameObject.FindGameObjectsWithTag(Names.Spike);

        if (spikeObjects.Length > 0)
        {
            foreach (GameObject s in spikeObjects)
            {
                if (Vector2.Distance(player.transform.position, s.transform.position) < distance)
                {
                    spike = s.GetComponent<SimpleSpikeController>();
                    distance = Vector2.Distance(player.transform.position, s.transform.position);
                }
            }
        }

        return spike;
    }

    /// <summary>
    /// Handles the input values and use them to move the player
    /// </summary>
    protected virtual void HandleInput()
    {
        if (IsDead())
        {
            GetComponent<Rigidbody2D>().velocity = new Vector2(0, GetComponent<Rigidbody2D>().velocity.y);
        }
        else
        {
            //Moving Right
            if (Input.GetAxis(Names.HorizontalInput) > 0)
            {
                if (Input.GetButtonUp(Names.RightInput))
                {
                    horAxis = 0;
                    if (!isInAir)
                        GetComponent<Animator>().Play("idle");

                    stoppedMov = true;
                }
                else
                {
                    if (!stoppedMov)
                    {
                        if (isCollidingRight)
                            horAxis = 0;
                        else
                        {
                            horAxis = horAxis < 1 ? horAxis + HOR_AXIS_STEP : MAX_AXIS_STEP;
                            if (Input.GetAxis(Names.HorizontalInput) < prevHorAxis)
                                horAxis = horAxis / 2;
                        }

                        GetComponent<Renderer>().transform.localScale = new Vector3(1, 1, 1);
                        if (!isInAir)
                            GetComponent<Animator>().Play("walk");
                    }
                }

                isFacingRight = true;

                if (!gameController.GameHasStarted())
                    gameController.MovePlayer();
            }
            else
                //Moving Left
                if (Input.GetAxis(Names.HorizontalInput) < 0)
                {
                    if (Input.GetButtonUp(Names.LeftInput))
                    {
                        horAxis = 0;
                        if (!isInAir)
                            GetComponent<Animator>().Play("idle");

                        stoppedMov = true;
                    }
                    else
                    {
                        if (!stoppedMov)
                        {
                            if (isCollidingLeft)
                                horAxis = 0;
                            else
                            {
                                horAxis = horAxis > -1 ? horAxis - HOR_AXIS_STEP : MIN_AXIS_STEP;
                                if (Input.GetAxis(Names.HorizontalInput) > prevHorAxis)
                                    horAxis = horAxis / 2;
                            }

                            GetComponent<Renderer>().transform.localScale = new Vector3(-1, 1, 1);
                            if (!isInAir)
                                GetComponent<Animator>().Play("walk");
                        }
                    }

                    isFacingRight = false;
                }
                //Test
                else if (autoRunning)
                {
                    if (isCollidingRight)
                        horAxis = 0;
                    else
                    {
                        //The higher the more relaxed
                        if (gameController.GetMeditation() > 50)
                        {
                            horAxis = MAX_AXIS_STEP;
                            if (!isInAir)
                                GetComponent<Animator>().Play("walk");
                        }
                        else
                        {
                            horAxis = 0;
                            if (!isInAir)
                                GetComponent<Animator>().Play("idle");
                        }
                    }
                }
                else
                //Not moving
                //if (Input.GetAxis(Names.HorizontalInput) == 0)
                {
                    horAxis = 0;
                    if (!isInAir)
                        GetComponent<Animator>().Play("idle");
                    stoppedMov = false;
                }

            //Update horizontal movement
            GetComponent<Rigidbody2D>().velocity = new Vector2(horAxis * speed, GetComponent<Rigidbody2D>().velocity.y);

            //Check if the game is running witht the EEG device
            //if (!gameController.IsGameNeurosky)
            //{
            //Jump button pressed
            if (Input.GetButtonDown(Names.JumpInput) && !isJumping)
            {
                
                GetComponent<Animator>().Play("jump-up");
                Jump();
            }

            //Jump button button released
            if (Input.GetButtonUp(Names.JumpInput) && isJumping)
            {
                GetComponent<Rigidbody2D>().velocity = new Vector2(GetComponent<Rigidbody2D>().velocity.x, GetComponent<Rigidbody2D>().velocity.y / 2);
                isJumping = false;
            }
            //}

            //Previous horizontal axis value
            prevHorAxis = Input.GetAxis(Names.HorizontalInput);
        }
    }

    public void Jump()
    {
        if (!isInAir)
        {
            audioSource.PlayOneShot(jumpSound);
            GetComponent<Rigidbody2D>().velocity = new Vector2(GetComponent<Rigidbody2D>().velocity.x, jumpImpulse);
            //GetComponent<Rigidbody2D>().AddForce(new Vector2(0, jumpImpulse));
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
        //audioSource.PlayOneShot(dieSound);
    }

    public virtual void Hit()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(Names.Enemy);
        GameObject[] spikes = GameObject.FindGameObjectsWithTag(Names.Spike);
        GameObject downCol = (GameObject)GameObject.Find("downCollider");

        if (!isHit)
        {
            GetComponent<Rigidbody2D>().velocity = new Vector2(GetComponent<Rigidbody2D>().velocity.x, 10f);
            //GetComponent<Rigidbody2D>().AddForce(new Vector2(-jumpImpulse * 3, jumpImpulse/2));
            //GetComponent<Rigidbody2D>().AddForce(new Vector2(0, jumpImpulse / 2));
            GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.4f);
            isHit = true;

            foreach (GameObject e in enemies)
            {
                Physics2D.IgnoreCollision(GetComponent<Collider2D>(), e.GetComponent<Collider2D>());
                Physics2D.IgnoreCollision(downCol.GetComponent<Collider2D>(), e.GetComponent<Collider2D>());
            }

            foreach (GameObject s in spikes)
            {
                Physics2D.IgnoreCollision(GetComponent<Collider2D>(), s.GetComponent<Collider2D>());
                Physics2D.IgnoreCollision(downCol.GetComponent<Collider2D>(), s.GetComponent<Collider2D>());
            }

            hits++;
            if (hits == 1)
                GameObject.Find("life2").GetComponent<Image>().color = new Color(1, 1, 1, 0);

            if (hits >= 2)
            {
                GameObject.Find("life1").GetComponent<Image>().color = new Color(1, 1, 1, 0);
                Die();
            }
        }
    }

    public void KillEnemy()
    {
        GetComponent<Rigidbody2D>().velocity = new Vector2(GetComponent<Rigidbody2D>().velocity.x,0);
        GetComponent<Rigidbody2D>().AddForce(new Vector2(0, jumpImpulse/2));
        //audioSource.PlayOneShot(killEnemySound);
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

    public bool IsHit()
    {
        return isHit;
    }

    protected virtual void OnCollisionEnter2D(Collision2D col)
    {
        PlatformController platform;
        RhythmFactory factory;
        SimpleSpikeController spike;
        SimpleEnemyController enemy;

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
            if (transform.position.x - GetComponent<Collider2D>().bounds.size.x / 2 > col.transform.position.x + col.collider.bounds.size.x / 2)
            {
                rightCollider = col.collider;
                isCollidingRight = true;
            }

            //Left collision
            if (transform.position.x + GetComponent<Collider2D>().bounds.size.x / 2 < col.transform.position.x - col.collider.bounds.size.x / 2)
            {
                leftCollider = col.collider;
                isCollidingLeft = true;
            }

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
            enemy = ((GameObject)col.gameObject).GetComponentInParent<SimpleEnemyController>();
            //Update attention
            UpdateEnemyDeath(enemy.jumpType);
            
            Debug.Log("Died!");
            //Die();
            Hit();
        }
        else if (col.collider.tag == Names.Spike)
        {
            spike = ((GameObject)col.gameObject).GetComponentInParent<SimpleSpikeController>();
            UpdateEnemyDeath(spike.jumpType);
            
            Debug.Log("Died!");
            //Die();
            Hit();
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
