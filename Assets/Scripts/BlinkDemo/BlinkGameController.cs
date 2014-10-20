using UnityEngine;
using System.Collections;

public class BlinkGameController : GameController 
{

    protected override void Start()
    {
        time = MAX_TIME;

        base.Start();
    }

    protected override void Update()
    {
        base.Update();

        if (isGameRunning)
        {
            //Check if it's game over
            if (!isGameOver)
            {
                if (time > 0)
                    time -= Time.deltaTime;
                else
                    isGameOver = true;
            }
        }
    }

    protected override void GameOver()
    {
        base.GameOver();

        Application.LoadLevel(Names.BlinkGameOverScene);
    }
}
