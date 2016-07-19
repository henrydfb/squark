using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class FlagController : MonoBehaviour 
{
    protected GameController gameController;

    void Start()
    {
        gameController = GameObject.Find(Names.GameController).GetComponent<GameController>();
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.collider.tag == Names.Player)
        {
            gameController.ReachGoal();

            /*
            gameController.SaveTime();
            gameController.SavePerformance("win");
            gameController.DisattachNeurosky();
             * */

            SceneManager.LoadScene("LevelComplete");
        }
    }
}
