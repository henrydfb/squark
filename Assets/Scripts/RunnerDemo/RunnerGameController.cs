using UnityEngine;
using System.Collections;

public class RunnerGameController : GameController {

    public float WorldSpeed;
    public GameObject platformPrefab;

    private const int START_WAIT_SEC = 3;

    private bool gameStarted;
    private float startTimer;
    private PlatformController lastPlatform;


    protected override void Start()
    {
        time = MIN_TIME;

        base.Start();

        gameStarted = false;
        startTimer = 0;

        lastPlatform = GameObject.Find(Names.FirstPlatform).GetComponent<PlatformController>();
    }

    protected override void Update()
    {
        base.Update();

        int waitTime;

        if (isGameRunning)
        {
            //Check if it's game over
            if (!isGameOver)
            {
                if(gameStarted)
                    time += Time.deltaTime;

                if (startTimer >= START_WAIT_SEC)
                {
                    feedbackMessage.Hide();
                    gameStarted = true;
                    if (lastPlatform != null)
                    {
                        if (lastPlatform.CompletelyEnteredCamera())
                            CreateNewPlatforms();
                    }
                    //player.GetComponent<RunnerPlayerController>().Move();
                }
                else
                {
                    startTimer += Time.deltaTime;
                    waitTime = (int)(START_WAIT_SEC + 1 - (startTimer % 60));
                    feedbackMessage.Show(waitTime.ToString());
                }
            }
        }
    }

    public void CreateNewPlatforms()
    {
        PlatformController newPlatform;
        GameObject newPlatObj;
        float newY, newX, yDiff;
        int newWidth, newHeight, attentionLevel;

        newX = lastPlatform.transform.position.x + lastPlatform.renderer.bounds.size.x/2;
        attentionLevel = Random.Range(0,100);
        //Low
        if (attentionLevel >= 0 && attentionLevel < 100 / 3)
        {
            if (lastPlatform.transform.position.y - 1 > downLimit)
                yDiff = -1;
            else
            {
                if (attentionLevel > (100 / 3) / 2)
                    yDiff = 0;
                else
                    yDiff = 1;
            }
        }
        //Medium
        else if (attentionLevel >= 100 / 3 && attentionLevel < 2 * (100 / 3))
            yDiff = 0;
        //High
        else
        {
            if (lastPlatform.transform.position.y + 1 < upLimit)
                yDiff = 1;
            else
            {
                if (attentionLevel > ((2 * (100 / 3)) - 100/2) /2)
                    yDiff = 0;
                else
                    yDiff = -1;
            }
        }

        newWidth = Random.Range(3, 20);
        newY = lastPlatform.transform.position.y + yDiff;
        
        newPlatObj = (GameObject)Instantiate(platformPrefab, new Vector3(newX, newY, 0), Quaternion.identity);
        newPlatform = newPlatObj.GetComponent<PlatformController>();
        newPlatform.Contruct(newWidth);
        newPlatObj.transform.position += new Vector3((newPlatObj.renderer.bounds.size.x / 2) + Random.Range(0.5f, 2f), 0, 0);
        lastPlatform = newPlatform;
    }

    public bool GameStarted()
    {
        return gameStarted;
    }

    protected override void GameOver()
    {
        base.GameOver();

        Application.LoadLevel(Names.RunnerGameOverScene);
    }
}
