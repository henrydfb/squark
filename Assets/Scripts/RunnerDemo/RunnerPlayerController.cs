using UnityEngine;
using System.Collections;

public class RunnerPlayerController : PlayerController
{
    private GameObject colObj;

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

    public void Move()
    {
        //Update horizontal movement
        GetComponent<Rigidbody2D>().velocity = new Vector2(speed, GetComponent<Rigidbody2D>().velocity.y);
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
