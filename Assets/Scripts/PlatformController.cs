using UnityEngine;
using System.Collections;

public class PlatformController : MonoBehaviour 
{
    protected RunnerGameController gameController;

	// Use this for initialization
	void Start () 
    {
        gameController = GameObject.Find(Names.GameController).GetComponent<RunnerGameController>();
	}

	// Update is called once per frame
	void Update () 
    {
        if (gameController.GameStarted())
        {
            transform.position += new Vector3(-Mathf.Abs(gameController.WorldSpeed), 0);
            //Destroy
            if (transform.position.x + renderer.bounds.size.x / 2 <= Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.WorldToScreenPoint(Camera.main.transform.position).x - Screen.width / 2, 0)).x)
                Destroy(gameObject);
        }
	}

    public bool CompletelyEnteredCamera()
    {
        return transform.position.x + renderer.bounds.size.x / 2 <= Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.WorldToScreenPoint(Camera.main.transform.position).x + Screen.width / 2, 0)).x;
    }

    public void Contruct(int numberOfBlock)
    { 
        transform.localScale = new Vector3(numberOfBlock,1);
    }
}
