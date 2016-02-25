using UnityEngine;
using System.Collections;

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
            Debug.Log("Level Complete!");
            gameController.SavePersistentData();
            gameController.SaveRhythmPersistentData();

            /*
            gameController.SaveTime();
            gameController.SavePerformance("win");
            gameController.DisattachNeurosky();
             * */
            Application.LoadLevel("LevelComplete");
        }
    }
}
