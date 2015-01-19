using UnityEngine;
using System.Collections;

public class RunnerGameController : GameController {

    public float WorldSpeed;
    public GameObject platformPrefab;
    public GameObject enemyPrefab;
    public GameObject coinPrefab;

    private const int START_WAIT_SEC = 5;

    private bool gameStarted;
    private float startTimer;
    private PlatformController lastPlatform;
    private CameraController cameraController;
    private float previousAttentionAverage;

    protected override void Start()
    {
        time = MIN_TIME;

        base.Start();

        gameStarted = false;
        startTimer = 0;

        
        lastPlatform = GameObject.Find(Names.FirstPlatform).GetComponent<PlatformController>();
        cameraController = Camera.main.GetComponent<CameraController>();
        previousAttentionAverage = 0;

        /*for (int i = 0; i < 150; i++)
        {
            GameObject obj = (GameObject)Instantiate(platformPrefab, new Vector3(lastPlatform.transform.position.x - lastPlatform.transform.renderer.bounds.size.x/2 +  platformPrefab.renderer.bounds.size.x / 2 + platformPrefab.renderer.bounds.size.x * i, -0.1f, 0), Quaternion.identity);
            if (i % 2 == 0)
                obj.renderer.material.color = new Color(1, 0, 0);

        }*/
    }

    bool jidoStarted = false;

    protected override void Update()
    {
        base.Update();

        int waitTime;

        if (isGameRunning)
        {
            //Check if it's game over
            if (!isGameOver)
            {
                /*if (gameStarted)
                {
                    time += Time.deltaTime;

                    if (time >= 1)
                    {
                        (player.GetComponent<RunnerPlayerController>()).autoRunning = false;
                        (player.GetComponent<RunnerPlayerController>()).Stop();
                    }
                }

                if (startTimer >= START_WAIT_SEC)
                {
                    if (!jidoStarted)
                    {
                        (player.GetComponent<RunnerPlayerController>()).autoRunning = true;
                        jidoStarted = true;
                    }
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
                }*/
            }
        }
    }

    public void CreateNewPlatforms()
    {
        PlatformController newPlatform;
        CoinController newCoin;
        SimpleEnemyController enemy;
        GameObject newPlatObj, newEnemyObj, newCoinObj;

        float newY, newX, yDiff, xSep, attentionLevel;
        int newWidth, newHeight, meditationLevel;

        newX = lastPlatform.transform.position.x + lastPlatform.renderer.bounds.size.x/2;
        //if (Random.Range(0, 100) >= 50)
            attentionLevel = cameraController.GetAverageAttention() * 100; //attention1;// Random.Range(0, 100);
            //Debug.Log("ATT: " + attentionLevel);
        //else
            //attentionLevel = Random.Range(0, 100);
        //Low
        //if (attentionLevel >= 0 && attentionLevel < 100 / 3)
        if (attentionLevel < previousAttentionAverage)
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
            /*if (lastPlatform.transform.position.y - 1 > downLimit)
                yDiff = -1;
            else
            {
                if (attentionLevel > (100 / 3) / 2)
                    yDiff = 0;
                else
                    yDiff = 1;
            }*/
        }
        //Medium
        //else if (attentionLevel >= 100 / 3 && attentionLevel < 2 * (100 / 3))
        else if (attentionLevel > previousAttentionAverage)
        {
            yDiff = 1;
            if (lastPlatform.transform.position.y + 1 < upLimit - 1)
                yDiff = 1;
            else
            {
                if (attentionLevel > ((2 * (100 / 3)) - 100 / 2) / 2)
                    yDiff = 0;
                else
                    yDiff = -1;
            }
        }
        //High
        else
        {
            yDiff = 0;
            /*if (lastPlatform.transform.position.y + 1 < upLimit - 1)
                yDiff = 1;
            else
            {
                if (attentionLevel > ((2 * (100 / 3)) - 100 / 2) / 2)
                    yDiff = 0;
                else
                    yDiff = -1;
            }*/
        }

        //Width
        if (Random.Range(0, 100) >= 50)
            newWidth = 3 + attention1 * 20 / 100;
        else
            newWidth = Random.Range(3, 20);

        //Width
        if (Random.Range(0, 100) >= 50)
            xSep = 0.5f + meditation1 * 2 / 100;
        else
            xSep = Random.Range(0.5f, 2f);
        
        
        newY = lastPlatform.transform.position.y + yDiff;
        
        newPlatObj = (GameObject)Instantiate(platformPrefab, new Vector3(newX, newY, 0), Quaternion.identity);
        newPlatform = newPlatObj.GetComponent<PlatformController>();
        newPlatform.Contruct(newWidth);
        newPlatObj.transform.position += new Vector3((newPlatObj.renderer.bounds.size.x / 2) + xSep, 0, 0);
        lastPlatform = newPlatform;

        //Enemy
        if (newWidth >= 10)
        {
            if (attentionLevel > previousAttentionAverage)
            {
                newEnemyObj = (GameObject)Instantiate(enemyPrefab, new Vector3(newPlatObj.transform.position.x, newPlatObj.transform.position.y, 0), Quaternion.identity);
                newEnemyObj.transform.position += new Vector3(0, (newPlatObj.collider2D.bounds.size.y + newEnemyObj.collider2D.bounds.size.y) / 2);
            }
        }

        //Coins
        if (newWidth >= 5)
        {
            if (attentionLevel < previousAttentionAverage)
            {
                for (int i = 1; i <= 3; i++)
                {
                    newCoinObj = (GameObject)Instantiate(coinPrefab, new Vector3(newPlatObj.transform.position.x, newPlatObj.transform.position.y, 0), Quaternion.identity);
                    newCoinObj.transform.position += new Vector3(0, (newPlatObj.collider2D.bounds.size.y + newCoinObj.collider2D.bounds.size.y * i * 2) / 2);
                }
            }
        }

        previousAttentionAverage = attentionLevel;
    }

    protected override void OnUpdateBlink(int value)
    {
        //base.OnUpdateBlink(value);
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
