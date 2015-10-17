using UnityEngine;
using System.Collections;

public class MushroomController : MonoBehaviour {

    protected GameController gameController;

    protected float speed;

	// Use this for initialization
	void Start () {
        gameController = GameObject.Find(Names.GameController).GetComponent<GameController>();
        speed = 0.009f;
	}
	
	// Update is called once per frame
	void Update () 
    {

        transform.position += new Vector3(speed, 0);

        //Destroy
        if ((transform.position.y + GetComponent<Renderer>().bounds.size.y / 2 <= gameController.downLimit))
            Destroy(gameObject);	
	}

    public void Pick()
    {
        Destroy(gameObject);
        gameController.PickMushroom();
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.collider.tag == Names.Platform || col.collider.tag == Names.Question || col.collider.tag == Names.Breakable)
        {
        }
        else if (col.collider.tag == Names.Goomba || col.collider.tag == Names.Koopa)
        {
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), col.collider);
        }
        else
        {
            speed = -speed;
        }
    }
}
