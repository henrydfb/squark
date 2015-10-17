using UnityEngine;
using System.Collections;

public class TurtleController : SimpleEnemyController 
{
    public enum State
    { 
        Moving,
        Waiting,
        Attacking
    }

    public Sprite attackSprite;
    public Sprite hitSprite;

    private State state;

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        //speed = 0.01f;
        speed = 0;
        state = State.Moving;
    }

    // Update is called once per frame
    protected override void Update()
    {
        transform.position += new Vector3(speed, 0);

        //Destroy
        if ((transform.position.y + GetComponent<Renderer>().bounds.size.y / 2 <= gameController.downLimit))
        {
            //Count beaten enemy
            if(state == State.Attacking)
                gameController.BeatEnemy();

            Destroy(gameObject);
        }

        
        if(speed > 0)
            GetComponent<Renderer>().transform.localScale = new Vector3(-1, 1, 1);
        else if(speed < 0)
            GetComponent<Renderer>().transform.localScale = new Vector3(1, 1, 1);

        if (Vector3.Distance(player.transform.position, transform.position) <= 4.5f && speed == 0 && state == State.Moving)
            speed = -0.009f;

        //Debug.Log(state);
    }

    protected override void OnCollisionEnter2D(Collision2D col)
    {
        TurtleController koopa;
        GoombaController goomba;

        if (col.collider.tag == Names.Platform)
        {
            if (platform == null)
                platform = col.collider.GetComponent<PlatformController>();
            else
            {
                if (col.collider.name != "downblockbounding")
                {
                    speed = -speed;
                }
            }
        }
        else
        {
            if (col.gameObject.tag == Names.Goomba)
            {
                goomba = col.gameObject.GetComponent<GoombaController>();

                if (state == State.Attacking)
                {
                    Physics2D.IgnoreCollision(GetComponent<Collider2D>(), col.collider);
                    Physics2D.IgnoreCollision(player.GetComponent<Collider2D>(), col.collider);
                    goomba.Die();
                }
                else
                    speed = -speed;
            }
            else if (col.gameObject.tag == Names.Koopa)
            {
                koopa = col.gameObject.GetComponent<TurtleController>();
                if (state == State.Attacking)
                {
                    koopa.Kill();
                }
                else
                    speed = -speed;
            }
            else if (col.collider.tag == Names.Mushroom)
            {
                Physics2D.IgnoreCollision(GetComponent<Collider2D>(), col.collider);
            }
            else
            {
                if(col.gameObject.tag != Names.Player)
                    speed = -speed;
            }
        }
    }
    public State GetState()
    {
        return state;
    }

    public void Kill()
    {
        //Count beaten enemy
        gameController.BeatEnemy();
        Destroy(gameObject);
    }

    public override void  Die()
    {
        SpriteRenderer ren;

        ren = GetComponent<SpriteRenderer>();
        switch(state)
        {
            case State.Attacking:
                speed = 0;
                state = State.Waiting;
                ren.sprite = hitSprite;
                break;
            case State.Moving:
                speed = 0;
                state = State.Waiting;
                ren.sprite = hitSprite;
                break;
            case State.Waiting:
                if (player.transform.position.x < transform.position.x)
                {
                    speed = 0.05f;
                    transform.position += new Vector3(2*speed, 0);
                }
                else
                {
                    speed = -0.05f;
                    transform.position += new Vector3(2 * speed, 0);
                }
                state = State.Attacking;
                ren.sprite = attackSprite;
                break;
        }
    }
}
