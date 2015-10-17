using UnityEngine;
using System.Collections;

public class SimpleSpikeController : MonoBehaviour {

    protected GameController gameController;

	// Use this for initialization
	void Start () {
        gameController = GameObject.Find(Names.GameController).GetComponent<GameController>();
        //GetComponent<Rigidbody2D>().gravityScale = 0;
	}
	
	// Update is called once per frame
	void Update () 
    {
        //Destroy
        if ((transform.position.y + GetComponent<Renderer>().bounds.size.y / 2 <= gameController.downLimit))
            Destroy(gameObject);	
	}
}
