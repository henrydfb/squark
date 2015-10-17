using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GCPlayerController : PlayerController
{
    public enum State
    { 
        Normal,
        Big
    }

    //protected RunnerGameController gameController;

    public bool autoRunning = false;

    public Sprite normalStateSprite;
    public Sprite bigStateSprite;

    State state;
    PlayerController player;
    GameObject colDownObj;
    List<Collider2D> disabledObjs;
    List<Collider2D> avoidedObjs;
    bool beingHit;
    float hitTimer;

    protected override void Start()
    {
        base.Start();
        player = gameObject.GetComponentInParent<PlayerController>();
        disabledObjs = new List<Collider2D>();
        avoidedObjs = new List<Collider2D>();
        state = State.Normal;
        beingHit = false;
        hitTimer = 0;
       // gameController = GameObject.Find(Names.GameController).GetComponent<GameController>();
    }

    protected override void Update()
    {
        base.Update();

        float playerX, playerY,playerW,playerH, playerLeft, playerRight, playerUp, playerDown;

        playerX = player.transform.position.x;
        playerY = player.transform.position.y;
        playerW = player.GetComponent<BoxCollider2D>().bounds.size.x;
        playerH = player.GetComponent<BoxCollider2D>().bounds.size.y;
        playerLeft = player.transform.position.x - player.GetComponent<BoxCollider2D>().bounds.size.x / 2;
        playerRight = player.transform.position.x + player.GetComponent<BoxCollider2D>().bounds.size.x / 2;
        playerUp = player.transform.position.y + player.GetComponent<BoxCollider2D>().bounds.size.y / 2;
        playerDown = player.transform.position.y - player.GetComponent<BoxCollider2D>().bounds.size.y / 2;

        if (gameController.GameStarted())
            transform.position += new Vector3(-Mathf.Abs(((GCGameController)gameController).WorldSpeed), 0);

        if (isInAir)
        {
            jumpTime += Time.deltaTime;

            if (prevVelY < 0 && GetComponent<Rigidbody2D>().velocity.y > 0 || prevVelY > 0 && GetComponent<Rigidbody2D>().velocity.y < 0)
            {
                jumpHeight = (float)gameObject.transform.position.y - (float)prevY;
            }

            prevVelY = GetComponent<Rigidbody2D>().velocity.y;
        }

        if (beingHit)
        {
            if (hitTimer < 3)
                hitTimer += Time.deltaTime;
            else
            {
                hitTimer = 0;
                beingHit = false;
                GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
                foreach(Collider2D c in avoidedObjs)
                    Physics2D.IgnoreCollision(GetComponent<Collider2D>(), c,false);

                avoidedObjs.Clear();
            }
        }
    }

    protected override void OnCollisionEnter2D(Collision2D col)
    {
        TurtleController turtle;
        GoombaController goomba;
        QuestionController question;
        MushroomController mushroom;
        
        float playerX, playerY, playerLeft, playerRight, playerUp, playerDown, colObjX, colObjY, colObjLeft, colObjRight, colObjUp, colObjDown;

        playerX = player.transform.position.x;
        playerY = player.transform.position.y;
        playerLeft = player.transform.position.x - player.GetComponent<BoxCollider2D>().bounds.size.x / 2;
        playerRight = player.transform.position.x + player.GetComponent<BoxCollider2D>().bounds.size.x / 2;
        playerUp = player.transform.position.y + player.GetComponent<BoxCollider2D>().bounds.size.y / 2;
        playerDown = player.transform.position.y - player.GetComponent<BoxCollider2D>().bounds.size.y / 2;

        if (col.gameObject.tag == Names.Platform || col.gameObject.tag == Names.Breakable || col.gameObject.tag == Names.Question || col.gameObject.tag == Names.Pipe)
        {
            colObjX = col.gameObject.GetComponent<BoxCollider2D>().transform.position.x;
            colObjY = col.gameObject.GetComponent<BoxCollider2D>().transform.position.y;
            colObjLeft = col.gameObject.GetComponent<BoxCollider2D>().transform.position.x - col.gameObject.GetComponent<BoxCollider2D>().GetComponent<BoxCollider2D>().bounds.size.x / 2;
            colObjRight = col.gameObject.GetComponent<BoxCollider2D>().transform.position.x + col.gameObject.GetComponent<BoxCollider2D>().GetComponent<BoxCollider2D>().bounds.size.x / 2;
            colObjUp = col.gameObject.GetComponent<BoxCollider2D>().transform.position.y + col.gameObject.GetComponent<BoxCollider2D>().GetComponent<BoxCollider2D>().bounds.size.y / 2;
            colObjDown = col.gameObject.GetComponent<BoxCollider2D>().transform.position.y - col.gameObject.GetComponent<BoxCollider2D>().GetComponent<BoxCollider2D>().bounds.size.y / 2;
            if (playerDown > colObjUp)
            {
                if (colDownObj == null)
                {
                    colDownObj = col.gameObject;
                    isInAir = false;
                }
                else
                {
                    col.collider.enabled = false;
                    disabledObjs.Add(col.collider);
                }
            }
            else if (playerUp < colObjDown)
            {
                if (col.gameObject.tag == Names.Question)
                {
                    question = col.gameObject.GetComponent<QuestionController>();
                    question.Hit();
                    gameController.SetCurrentAction("bonus");
                }
                else if (col.gameObject.tag == Names.Breakable)
                {
                    if (state == State.Big)
                    {
                        gameController.BreakBlock();
                        Destroy(col.gameObject);
                        gameController.SetCurrentAction("break");
                    }
                }
            }
            else if (playerLeft > colObjRight)
            {
                isCollidingLeft = true;
                leftColObj = col.gameObject;
            }
            else if (playerRight < colObjLeft)
            {
                isCollidingRight = true;
                rightColObj = col.gameObject;
            }
        }
        else if (col.collider.tag == Names.Koopa)
        {
            turtle = col.gameObject.GetComponent<TurtleController>();
            colObjX = col.gameObject.GetComponent<BoxCollider2D>().transform.position.x;
            colObjY = col.gameObject.GetComponent<BoxCollider2D>().transform.position.y;
            colObjLeft = col.gameObject.GetComponent<BoxCollider2D>().transform.position.x - col.gameObject.GetComponent<BoxCollider2D>().GetComponent<BoxCollider2D>().bounds.size.x / 2;
            colObjRight = col.gameObject.GetComponent<BoxCollider2D>().transform.position.x + col.gameObject.GetComponent<BoxCollider2D>().GetComponent<BoxCollider2D>().bounds.size.x / 2;
            colObjUp = col.gameObject.GetComponent<BoxCollider2D>().transform.position.y + col.gameObject.GetComponent<BoxCollider2D>().GetComponent<BoxCollider2D>().bounds.size.y / 2;

            if (playerDown > colObjUp)
            {
                KillJump();
                turtle.Die();
                gameController.SetCurrentAction("hit koopa");
            }
            else if (playerLeft > colObjRight)
            {
                if (turtle.GetState() == TurtleController.State.Attacking || turtle.GetState() == TurtleController.State.Moving)
                {
                    if (state == State.Big)
                    {
                        ChangeState(State.Normal);
                        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), col.collider, true);
                        beingHit = true;
                        GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);
                        GetComponent<Rigidbody2D>().velocity = Vector3.zero;
                        GetComponent<Rigidbody2D>().AddForce(new Vector2(0, jumpImpulse / 2));
                        avoidedObjs.Add(col.collider);
                        gameController.SetCurrentAction("hurt koopa");
                    }
                    else
                    {
                        if (beingHit)
                        {
                            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), col.collider);
                            avoidedObjs.Add(col.collider);
                        }
                        else
                            Die();
                    }
                }
                else
                {
                    turtle.Die();
                    gameController.SetCurrentAction("kill koopa");
                }
            }
            else if (playerRight < colObjLeft)
            {
                if (turtle.GetState() == TurtleController.State.Attacking || turtle.GetState() == TurtleController.State.Moving)
                {
                    if (state == State.Big)
                    {
                        ChangeState(State.Normal);
                        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), col.collider, true);
                        beingHit = true;
                        GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);
                        GetComponent<Rigidbody2D>().velocity = Vector3.zero;
                        GetComponent<Rigidbody2D>().AddForce(new Vector2(0, jumpImpulse / 2));
                        avoidedObjs.Add(col.collider);
                        gameController.SetCurrentAction("hurt koopa");
                    }
                    else
                    {
                        if (beingHit)
                        {
                            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), col.collider);
                            avoidedObjs.Add(col.collider);
                        }
                        else
                            Die();
                    }
                }
                else
                {
                    turtle.Die();
                    gameController.SetCurrentAction("kill koopa");
                }
            }
        }
        else if (col.collider.tag == Names.Goomba)
        {
            goomba = col.gameObject.GetComponent<GoombaController>();
            colObjX = col.gameObject.GetComponent<BoxCollider2D>().transform.position.x;
            colObjY = col.gameObject.GetComponent<BoxCollider2D>().transform.position.y;
            colObjLeft = col.gameObject.GetComponent<BoxCollider2D>().transform.position.x - col.gameObject.GetComponent<BoxCollider2D>().GetComponent<BoxCollider2D>().bounds.size.x / 2;
            colObjRight = col.gameObject.GetComponent<BoxCollider2D>().transform.position.x + col.gameObject.GetComponent<BoxCollider2D>().GetComponent<BoxCollider2D>().bounds.size.x / 2;
            colObjUp = col.gameObject.GetComponent<BoxCollider2D>().transform.position.y + col.gameObject.GetComponent<BoxCollider2D>().GetComponent<BoxCollider2D>().bounds.size.y / 2;

            if (playerDown > colObjUp)
            {
                KillJump();
                goomba.Die();
                Physics2D.IgnoreCollision(GetComponent<Collider2D>(), col.collider, true);
                gameController.SetCurrentAction("kill goomba");
            }
            else if (playerLeft > colObjRight)
            {
                if (state == State.Big)
                {
                    ChangeState(State.Normal);
                    Physics2D.IgnoreCollision(GetComponent<Collider2D>(), col.collider,true);
                    beingHit = true;
                    GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);
                    GetComponent<Rigidbody2D>().velocity = Vector3.zero;
                    GetComponent<Rigidbody2D>().AddForce(new Vector2(0, jumpImpulse / 2));
                    avoidedObjs.Add(col.collider);
                    gameController.SetCurrentAction("hurt goomba");
                }
                else
                {
                    if (beingHit)
                    {
                        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), col.collider,true);
                        avoidedObjs.Add(col.collider);
                    }
                    else
                        Die();
                }
            }
            else if (playerRight < colObjLeft)
            {
                if (state == State.Big)
                {
                    ChangeState(State.Normal);
                    Physics2D.IgnoreCollision(GetComponent<Collider2D>(), col.collider,true);
                    beingHit = true;
                    GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);
                    GetComponent<Rigidbody2D>().velocity = Vector3.zero;
                    GetComponent<Rigidbody2D>().AddForce(new Vector2(0, jumpImpulse / 2));
                    avoidedObjs.Add(col.collider);
                    gameController.SetCurrentAction("hurt goomba");
                }
                else
                {
                    if (beingHit)
                    {
                        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), col.collider);
                        avoidedObjs.Add(col.collider);
                    }
                    else
                        Die();
                }
            }
        }
        else if (col.collider.tag == Names.Mushroom)
        {
            mushroom = col.gameObject.GetComponent<MushroomController>();
            mushroom.Pick();
            if(state == State.Normal)
                ChangeState(State.Big);

            gameController.SetCurrentAction("mushroom");
        }
    }

    public override void Die()
    {
        if(!beingHit)
            base.Die();
    }

    public void ChangeState(State state)
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        switch (state)
        { 
            case State.Normal:
                spriteRenderer.sprite = normalStateSprite;
                break;
            case State.Big:
                spriteRenderer.sprite = bigStateSprite;
                break;
        }

        this.state = state;
    }

    protected void KillJump()
    {
        GetComponent<Rigidbody2D>().velocity = new Vector2(GetComponent<Rigidbody2D>().velocity.x, 4);
        //GetComponent<Rigidbody2D>().AddForce(new Vector2(0, jumpImpulse / 2));
    }

    protected override void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject.tag == Names.Platform || col.gameObject.tag == Names.Breakable || col.gameObject.tag == Names.Question || col.gameObject.tag == Names.Pipe)
        {
            if (colDownObj != null)
            {
                colDownObj = null;
                foreach (Collider2D c in disabledObjs)
                {
                    if(c != null)
                        c.enabled = true;
                }

                disabledObjs.Clear();
            }

            if (isCollidingLeft)
            {
                if (leftColObj == col.gameObject)
                    isCollidingLeft = false;
            }

            if (isCollidingRight)
            {
                if (rightColObj == col.gameObject)
                    isCollidingRight = false;
            }
        }
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
                if (isCollidingRight)
                    horAxis = 0;
                else
                {
                    horAxis = horAxis < 1 ? horAxis + HOR_AXIS_STEP : MAX_AXIS_STEP;
                    if (Input.GetAxis(Names.HorizontalInput) < prevHorAxis)
                        horAxis = horAxis / 2;
                }
                
                GetComponent<Renderer>().transform.localScale = new Vector3(1, 1, 1);

                if (Input.GetButtonDown("Horizontal"))
                    gameController.SetCurrentAction("right");
            }

            //Moving Left
            if (Input.GetAxis(Names.HorizontalInput) < 0)
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

                if (Input.GetButtonDown("Horizontal"))
                    gameController.SetCurrentAction("left");
            }

            //Not moving
            if (Input.GetAxis(Names.HorizontalInput) == 0)
                horAxis = 0;

            //Update horizontal movement
            GetComponent<Rigidbody2D>().velocity = new Vector2(horAxis * speed, GetComponent<Rigidbody2D>().velocity.y);

            //Jump button pressed
            if (Input.GetButtonDown(Names.JumpInput) && !isJumping)
            {
                Jump();
                gameController.SetCurrentAction("jump");
            }

            //Jump button button released
            if (Input.GetButtonUp(Names.JumpInput) && isJumping)
            {
                GetComponent<Rigidbody2D>().velocity = new Vector2(GetComponent<Rigidbody2D>().velocity.x, GetComponent<Rigidbody2D>().velocity.y / 2);
                isJumping = false;
            }
            
            //Previous horizontal axis value
            prevHorAxis = Input.GetAxis(Names.HorizontalInput);
        }
    }

    public void StopJump()
    {
        GetComponent<Rigidbody2D>().velocity = new Vector2(GetComponent<Rigidbody2D>().velocity.x,0);
    }

    public void Stop()
    {
        horAxis = 0;   
    }
}
