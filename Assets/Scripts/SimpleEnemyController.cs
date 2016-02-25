using UnityEngine;
using System.Collections;

public class SimpleEnemyController : MonoBehaviour 
{
    public Jump.HeightType jumpType;

    protected PlayerController player;

    protected GameController gameController;

    protected float speed;

    protected PlatformController platform;

    protected Vector3 offset;

	// Use this for initialization
	protected virtual void Start () 
    {
        gameController = GameObject.Find(Names.GameController).GetComponent<GameController>();
        player = GameObject.Find(Names.Player).GetComponent<PlayerController>();
        //speed = 0.025f;
        speed = 0;
        offset = new Vector3();
        //GetComponent<Rigidbody2D>().gravityScale = 0;
	}
	
	// Update is called once per frame
    protected virtual void Update() 
    {
        if (gameController.GameStarted())
        {
            
            
            /*if (platform == null)
                transform.position += new Vector3(-Mathf.Abs(gameController.WorldSpeed), 0);
            else
            {
                transform.position = new Vector3(platform.transform.position.x,transform.position.y) + offset;

                if (offset.x - collider2D.bounds.size.x / 2 <= -platform.collider2D.bounds.size.x / 2 || offset.x + collider2D.bounds.size.x / 2 >= platform.collider2D.bounds.size.x / 2)
                    speed = -speed;

                offset += new Vector3(speed, 0, 0);
                //transform.position += new Vector3(speed, 0);
            }*/

            //Destroy
            if ((transform.position.y + GetComponent<Renderer>().bounds.size.y / 2 <= gameController.downLimit))
                Destroy(gameObject);
        }	
	}

    protected virtual void OnCollisionEnter2D(Collision2D col)
    {
        if (col.collider.tag == Names.Platform)
        {
            if (platform == null)
                platform = col.collider.GetComponent<PlatformController>();
            else
            {
                if (platform != col.collider.GetComponent<PlatformController>())
                    speed = -speed;
            }
        }
    }

    protected virtual void OnCollisionExit2D(Collision2D col)
    {
       
    }

    public virtual void Die()
    {
        GameObject.Destroy(gameObject);
    }
}
