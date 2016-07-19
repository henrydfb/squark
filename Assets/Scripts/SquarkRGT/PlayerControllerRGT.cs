using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerControllerRGT : PlayerController
{
    private GameControllerRGT gamecontrollerRGT;

    protected override void Start()
    {
        base.Start();

        gamecontrollerRGT = GameObject.Find(Names.GameController).GetComponent<GameControllerRGT>();
        gameController = gamecontrollerRGT;
    }

    override protected void UpdateSpikeAttention(Jump.HeightType jumpType)
    {
        //Update attention
        switch (jumpType)
        {
            case global::Jump.HeightType.Long:
                gamecontrollerRGT.AddAttentionValue(GameControllerRGT.AttentionType.Hig);
                break;
            case global::Jump.HeightType.Medium:
                gamecontrollerRGT.AddAttentionValue(GameControllerRGT.AttentionType.Med);
                break;
            case global::Jump.HeightType.Short:
                gamecontrollerRGT.AddAttentionValue(GameControllerRGT.AttentionType.Low);
                break;
        }

        gamecontrollerRGT.AddAttentionValue(GameControllerRGT.AttentionType.Spk);
    }

    override protected void UpdateEnemyDeath(Jump.HeightType jumpType)
    {
        switch (jumpType)
        {
            case global::Jump.HeightType.Long:
                gamecontrollerRGT.AddDeath(GameControllerRGT.DeathType.Hig);
                break;
            case global::Jump.HeightType.Medium:
                gamecontrollerRGT.AddDeath(GameControllerRGT.DeathType.Med);
                break;
            case global::Jump.HeightType.Short:
                gamecontrollerRGT.AddDeath(GameControllerRGT.DeathType.Low);
                break;
        }

        gamecontrollerRGT.AddDeath(GameControllerRGT.DeathType.Ene);
    }

    override protected void UpdateEnemySpike(Jump.HeightType jumpType)
    {
        //Update attention
        switch (jumpType)
        {
            case global::Jump.HeightType.Long:
                gamecontrollerRGT.AddDeath(GameControllerRGT.DeathType.Hig);
                break;
            case global::Jump.HeightType.Medium:
                gamecontrollerRGT.AddDeath(GameControllerRGT.DeathType.Med);
                break;
            case global::Jump.HeightType.Short:
                gamecontrollerRGT.AddDeath(GameControllerRGT.DeathType.Low);
                break;
        }

        gamecontrollerRGT.AddDeath(GameControllerRGT.DeathType.Spk);
    }

    protected override void HandleInput()
    {
        base.HandleInput();

        //Moving Right
        if (Input.GetAxis(Names.HorizontalInput) > 0)
        {
            if (isCollidingLeft)
                horAxis = 0;
            /*else
            {
                horAxis = horAxis < 1 ? horAxis + HOR_AXIS_STEP : MAX_AXIS_STEP;
                if (Input.GetAxis(Names.HorizontalInput) < prevHorAxis)
                    horAxis = horAxis / 2;
            }*/
        }

        //Moving Left
        if (Input.GetAxis(Names.HorizontalInput) < 0)
        {
            if (isCollidingRight)
                horAxis = 0;
            /*else
            {
                horAxis = horAxis > -1 ? horAxis - HOR_AXIS_STEP : MIN_AXIS_STEP;
                if (Input.GetAxis(Names.HorizontalInput) > prevHorAxis)
                    horAxis = horAxis / 2;
            }*/
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

    protected override void Update()
    {
        base.Update();

        if (isInAir)
        {
            jumpTime += Time.deltaTime;

            if (prevVelY < 0 && GetComponent<Rigidbody2D>().velocity.y > 0 || prevVelY > 0 && GetComponent<Rigidbody2D>().velocity.y < 0)
                jumpHeight = (float)gameObject.transform.position.y - (float)prevY;

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
}
