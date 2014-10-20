using UnityEngine;
using System.Collections;

public class GoalController : MonoBehaviour 
{

    private GameController gameController;

	// Use this for initialization
	void Start () 
    {
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == Names.Player && gameController.WonGame())
            Application.LoadLevel(Names.BlinkWinGameScene);
    }
}
