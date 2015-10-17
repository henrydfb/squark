using UnityEngine;
using System.Collections;

public class BonusController : MonoBehaviour {

    protected GameController gameController;

	// Use this for initialization
	void Start () {
        gameController = GameObject.Find(Names.GameController).GetComponent<GameController>();
	}
	
	// Update is called once per frame
	void Update () {
        //Destroy
        if ((transform.position.y + GetComponent<Renderer>().bounds.size.y / 2 <= gameController.downLimit))
            Destroy(gameObject);
	}
}
