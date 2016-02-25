using UnityEngine;
using System.Collections;

public class RunnerPlayerController : PlayerController
{
    //protected RunnerGameController gameController;

    public AudioClip jumpSound;
    public bool autoRunning = false;
    private bool stoppedMov;
    private bool isFacingRight;
    private bool startMoving;

    private GameObject colObj;

    protected override void Start()
    {
        base.Start();
        gameController = GameObject.Find(Names.GameController).GetComponent<RunnerGameController>();
        stoppedMov =false;
        isFacingRight = true;
        startMoving = false;
    }

    protected override void Update()
    {
        base.Update();

        if (((RunnerGameController)gameController).GameStarted())
            transform.position += new Vector3(-Mathf.Abs(((RunnerGameController)gameController).WorldSpeed), 0);

        if (isInAir)
        {
            jumpTime += Time.deltaTime;

            if (prevVelY < 0 && GetComponent<Rigidbody2D>().velocity.y > 0 || prevVelY > 0 && GetComponent<Rigidbody2D>().velocity.y < 0)
            {
                jumpHeight = (float)gameObject.transform.position.y - (float)prevY;

                //print("height: " + t);
                //Debug.Log("wii ");
                //print("curr: "+ transform.position.y);
                //print("prev: "  + prevY);
                //Debug.Log(rigidbody2D.position.y);
                //print(transform.position.y);
                //Debug.Log(rigidbody2D.position.y - prevY);
                //Stop();

            }

            if (isInAir)
            {
                if (GetComponent<Rigidbody2D>().velocity.y < 0)
                    GetComponent<Animator>().Play("jump-down");
            }
            prevVelY = GetComponent<Rigidbody2D>().velocity.y;
            //print(rigidbody2D.velocity.y);
            //print("height: " + gameObject.transform.position.y);
        }
    }

    public bool IsFacingRight()
    {
        return isFacingRight;
    }

    public void Move()
    {
        //Update horizontal movement
        GetComponent<Rigidbody2D>().velocity = new Vector2(speed, GetComponent<Rigidbody2D>().velocity.y);
    }

    protected override void HandleInput()
    {
        base.HandleInput();

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
                    if(!stoppedMov)
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
                    if(!stoppedMov)
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
                audioSource.PlayOneShot(jumpSound);
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

    public void Stop()
    {
        horAxis = 0;
        
    }

    protected override void OnCollisionEnter2D(Collision2D col)
    {
        base.OnCollisionEnter2D(col);

        PlayerController player;

        player = gameObject.GetComponentInParent<PlayerController>();
        
        //This is checking if the playercollides with a platform from ouside
        //It also knows from where, left or right
        if (col.collider.tag == Names.Platform)
        {
            if (player.transform.position.x + player.GetComponent<BoxCollider2D>().bounds.size.x/2 <= col.collider.transform.position.x - col.collider.GetComponent<BoxCollider2D>().bounds.size.x/2)
            {
                isCollidingRight = true;
                colObj = col.collider.gameObject;
            }

            if (player.transform.position.x - player.GetComponent<BoxCollider2D>().bounds.size.x / 2 >= col.collider.transform.position.x + col.collider.GetComponent<BoxCollider2D>().bounds.size.x / 2)
            {
                isCollidingLeft = true;
                colObj = col.collider.gameObject;
            }
        }
    }
 
    
    protected override void OnCollisionExit2D(Collision2D col)
    {

        //If player is colliding from outside with a platform, check is has exited it
        if (colObj != null)
        {
            if (colObj == col.collider.gameObject)
            {
                if (isCollidingLeft)
                {
                    isCollidingLeft = false;
                    colObj = null;
                }

                if (isCollidingRight)
                {
                    isCollidingRight = false;
                    colObj = null;
                }
            }
        }
    }
}
