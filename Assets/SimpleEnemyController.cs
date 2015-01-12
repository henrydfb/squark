using UnityEngine;
using System.Collections;

public class SimpleEnemyController : MonoBehaviour 
{

    protected RunnerGameController gameController;

    private float speed;

    private PlatformController platform;

    private Vector3 offset;

	// Use this for initialization
	void Start () 
    {
        gameController = GameObject.Find(Names.GameController).GetComponent<RunnerGameController>();
        speed = 0.025f;
        offset = new Vector3();
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (gameController.GameStarted())
        {
            
            
            if (platform == null)
                transform.position += new Vector3(-Mathf.Abs(gameController.WorldSpeed), 0);
            else
            {
                transform.position = new Vector3(platform.transform.position.x,transform.position.y) + offset;

                if (offset.x - collider2D.bounds.size.x / 2 <= -platform.collider2D.bounds.size.x / 2 || offset.x + collider2D.bounds.size.x / 2 >= platform.collider2D.bounds.size.x / 2)
                    speed = -speed;

                offset += new Vector3(speed, 0, 0);
                //transform.position += new Vector3(speed, 0);
            }

            //Destroy
            if (transform.position.x + renderer.bounds.size.x / 2 <= Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.WorldToScreenPoint(Camera.main.transform.position).x - Screen.width / 2, 0)).x)
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
}
