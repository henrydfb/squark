using UnityEngine;
using System.Collections;

public class GoombaController : SimpleEnemyController 
{
    GameObject collidingObj;

    public Sprite dieSprite;
    bool isDying;
    float dieTimer;

    // Use this for initialization
    protected override void  Start()
    {
        base.Start();
        //speed = 0.01f;
        //speed = -0.009f;
        speed = 0;
        dieTimer = 0;
        isDying = false;
    }

    // Update is called once per frame
    protected override void  Update()
    {
        if (isDying)
        {
            if (dieTimer < 3)
                dieTimer += Time.deltaTime;
            else
            {
                base.Die();
                //Cound beaten enemy
                gameController.BeatEnemy();
            }
        }
        else
        {
            if (Vector3.Distance(player.transform.position, transform.position) <= 4.5f && speed == 0)
                speed = -0.009f;

            transform.position += new Vector3(speed, 0);

            //Destroy
            if ((transform.position.y + GetComponent<Renderer>().bounds.size.y / 2 <= gameController.downLimit))
                Destroy(gameObject);
        }
    }

    protected override void  OnCollisionEnter2D(Collision2D col)
    {
        float playerDown, colObjUp;

        if(player == null)
            player = GameObject.Find(Names.Player).GetComponent<PlayerController>();

        playerDown = player.transform.position.y - player.GetComponent<BoxCollider2D>().bounds.size.y / 2;
        colObjUp = col.gameObject.GetComponent<BoxCollider2D>().transform.position.y + col.gameObject.GetComponent<BoxCollider2D>().GetComponent<BoxCollider2D>().bounds.size.y / 2;

        if (col.collider.tag == Names.Platform || col.collider.tag == Names.Question || col.collider.tag == Names.Breakable)
        {
            if (playerDown > colObjUp)
            {
                collidingObj = col.gameObject;
            }
        }
        else if (col.collider.tag == Names.Mushroom)
        {
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), col.collider);
        }
        else if (col.collider.tag == Names.Player)
        { }
        else
        {
            if (collidingObj != col.gameObject)
                speed = -speed;
        }
    }

    public override void Die()
    {
        //base.Die();
        GetComponent<SpriteRenderer>().sprite = dieSprite;
        isDying = true;
        speed = 0;
    }
}
