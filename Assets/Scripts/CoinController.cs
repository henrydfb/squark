using UnityEngine;
using System.Collections;

public class CoinController : MonoBehaviour 
{
    protected GameController gameController;

	// Use this for initialization
	void Start () 
    {
        gameController = GameObject.Find(Names.GameController).GetComponent<GameController>();
	}
	
	// Update is called once per frame
	void Update () {
        float worldSpeed = 0; //=gameController.WorldSpeed

        if (gameController.GameStarted())
        {
            transform.position += new Vector3(-Mathf.Abs(worldSpeed), 0);
            //Destroy
            if (transform.position.x + GetComponent<Renderer>().bounds.size.x / 2 <= Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.WorldToScreenPoint(Camera.main.transform.position).x - Screen.width / 2, 0)).x)
                Destroy(gameObject);
        }
	}

    void OnTriggerEnter2D(Collider2D col) 
    {
        if (col.tag == Names.Player)
        {
            gameController.PickCoin();
            //audio.PlayOneShot(audio.clip);
            Destroy(gameObject);
        }
    }
}
