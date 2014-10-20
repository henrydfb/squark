using UnityEngine;
using System.Collections;

public class RunnerPlayerController : PlayerController
{
    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }


    public void Move()
    {
        //Update horizontal movement
        rigidbody2D.velocity = new Vector2(speed, rigidbody2D.velocity.y);
    }

    protected override void HandleInput()
    {
        base.HandleInput();

        //Check if the game is running witht the EEG device
        if (!gameController.IsGameNeurosky)
        {
            //Jump button pressed
            if (Input.GetButtonDown(Names.JumpInput) && !isJumping)
                Jump();

            //Jump button button released
            if (Input.GetButtonUp(Names.JumpInput) && isJumping)
            {
                rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, rigidbody2D.velocity.y / 2);
                isJumping = false;
            }
        }
    }
}
