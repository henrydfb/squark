using UnityEngine;
using System.Collections;

public class EEGPlatformPlayerController : PlayerController
{
    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void HandleInput()
    {
        base.HandleInput();

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
        GetComponent<Rigidbody2D>().velocity = new Vector2(horAxis * speed, GetComponent<Rigidbody2D>().velocity.y);

        //Check if the game is running witht the EEG device
        if (!gameController.IsGameNeurosky)
        {
            //Jump button pressed
            if (Input.GetButtonDown(Names.JumpInput) && !isJumping)
                Jump();

            //Jump button button released
            if (Input.GetButtonUp(Names.JumpInput) && isJumping)
            {
                GetComponent<Rigidbody2D>().velocity = new Vector2(GetComponent<Rigidbody2D>().velocity.x, GetComponent<Rigidbody2D>().velocity.y / 2);
                isJumping = false;
            }
        }

        //Previous horizontal axis value
        prevHorAxis = Input.GetAxis(Names.HorizontalInput);
    }
}
