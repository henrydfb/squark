using UnityEngine;
using System.Collections;

public class RunnerPlayerController : PlayerController
{
    //protected RunnerGameController gameController;

    public bool autoRunning = false;

    protected override void Start()
    {
        base.Start();
        gameController = GameObject.Find(Names.GameController).GetComponent<RunnerGameController>();
    }

    protected override void Update()
    {
        base.Update();

        if (((RunnerGameController)gameController).GameStarted())
            transform.position += new Vector3(-Mathf.Abs(((RunnerGameController)gameController).WorldSpeed), 0);

        if (isInAir)
        {
            jumpTime += Time.deltaTime;

            if (prevVelY < 0 && rigidbody2D.velocity.y > 0 || prevVelY > 0 && rigidbody2D.velocity.y < 0)
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

            prevVelY = rigidbody2D.velocity.y;
            //print(rigidbody2D.velocity.y);
            //print("height: " + gameObject.transform.position.y);
        }
    }


    public void Move()
    {
        //Update horizontal movement
        rigidbody2D.velocity = new Vector2(speed, rigidbody2D.velocity.y);
    }

    protected override void HandleInput()
    {
        base.HandleInput();

        if (IsDead())
        {
            rigidbody2D.velocity = new Vector2(0, rigidbody2D.velocity.y);
        }
        else
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

            //Test
            if (autoRunning)
            {
                if (isCollidingLeft)
                    horAxis = 0;
                else
                {
                    horAxis = MAX_AXIS_STEP;
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

            //Check if the game is running witht the EEG device
            //if (!gameController.IsGameNeurosky)
            //{
            //Jump button pressed
            if (Input.GetButtonDown(Names.JumpInput) && !isJumping)
                Jump();

            //Jump button button released
            if (Input.GetButtonUp(Names.JumpInput) && isJumping)
            {
                rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, rigidbody2D.velocity.y / 2);
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
}
