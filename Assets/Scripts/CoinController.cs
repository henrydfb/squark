using UnityEngine;
using System.Collections;

public class CoinController : MonoBehaviour 
{
    protected RunnerGameController gameController;

	// Use this for initialization
	void Start () 
    {
        gameController = GameObject.Find(Names.GameController).GetComponent<RunnerGameController>();
	}
	
	// Update is called once per frame
	void Update () {
        if (gameController.GameStarted())
        {
            transform.position += new Vector3(-Mathf.Abs(gameController.WorldSpeed), 0);
            //Destroy
            if (transform.position.x + renderer.bounds.size.x / 2 <= Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.WorldToScreenPoint(Camera.main.transform.position).x - Screen.width / 2, 0)).x)
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
