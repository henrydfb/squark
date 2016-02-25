using UnityEngine;
using System.Collections;

public class GCGameController : GameController 
{
    public const float BLOCK_SIZE = 0.32f;
    public const int GAME_TIME = 60 * 2;

    public float WorldSpeed;
    public GameObject platformPrefab;
    public GameObject enemyPrefab;
    public GameObject coinPrefab;
    public GameObject playerShadowPrefab;

    //Floor
    public GameObject downBlockPrefab;
    public GameObject downBlockBoundingPrefab;

    //Solid
    public GameObject solidBlockPrefab;
    public GameObject solidBlockBoundingPrefab;

    //Breakable
    public GameObject breakBlockPrefab;

    //Question
    public GameObject questionBlockPrefab;
    public GameObject mushroomQuestionBlockPrefab;

    //Question
    public GameObject flagPrefab;

    //Pipe
    public GameObject pipeBasePrefab;
    public GameObject pipePartPrefab;
    public GameObject pipeBoundingPrefab;

    //Enemies
    public GameObject koopaPrefab;
    public GameObject goombaPrefab;

    private const int START_WAIT_SEC = 3;

    private float startTimer;
    private PlatformController lastPlatform;
    private CameraController cameraController;
    private float previousAttentionAverage;
    private int currentSec;
    
    private float iniX;
    private int prevTime;

    protected override void Start()
    {
        time = GAME_TIME;

        base.Start();

        currLevelType = "mario";
        gameStarted = false;
        startTimer = 0;

        if(GameObject.Find(Names.FirstPlatform) != null)
            lastPlatform = GameObject.Find(Names.FirstPlatform).GetComponent<PlatformController>();
        cameraController = Camera.main.GetComponent<CameraController>();
        previousAttentionAverage = 0;
        currentSec = 0;

        //Floor
        CreateDownBlocks(downBlockPrefab,downBlockBoundingPrefab,0, 70, 0, 2);
        CreateDownBlocks(downBlockPrefab,downBlockBoundingPrefab, 72, 15, 0, 2);
        CreateDownBlocks(downBlockPrefab,downBlockBoundingPrefab, 90, 64, 0, 2);
        CreateDownBlocks(downBlockPrefab,downBlockBoundingPrefab, 156, 46, 0, 2);

        //Solid
        CreateDownBlocks(solidBlockPrefab, solidBlockBoundingPrefab, 134, 1, 1, 1);
        CreateDownBlocks(solidBlockPrefab, solidBlockBoundingPrefab, 135, 1, 2, 2);
        CreateDownBlocks(solidBlockPrefab, solidBlockBoundingPrefab, 136, 1, 3, 3);
        CreateDownBlocks(solidBlockPrefab, solidBlockBoundingPrefab, 137, 1, 4, 4);

        CreateDownBlocks(solidBlockPrefab, solidBlockBoundingPrefab, 140, 1, 4, 4);
        CreateDownBlocks(solidBlockPrefab, solidBlockBoundingPrefab, 141, 1, 3, 3);
        CreateDownBlocks(solidBlockPrefab, solidBlockBoundingPrefab, 142, 1, 2, 2);
        CreateDownBlocks(solidBlockPrefab, solidBlockBoundingPrefab, 143, 1, 1, 1);

        CreateDownBlocks(solidBlockPrefab, solidBlockBoundingPrefab, 149, 1, 1, 1);
        CreateDownBlocks(solidBlockPrefab, solidBlockBoundingPrefab, 150, 1, 2, 2);
        CreateDownBlocks(solidBlockPrefab, solidBlockBoundingPrefab, 151, 1, 3, 3);
        CreateDownBlocks(solidBlockPrefab, solidBlockBoundingPrefab, 152, 1, 4, 4);
        CreateDownBlocks(solidBlockPrefab, solidBlockBoundingPrefab, 153, 1, 4, 4);

        CreateDownBlocks(solidBlockPrefab, solidBlockBoundingPrefab, 156, 1, 4, 4);
        CreateDownBlocks(solidBlockPrefab, solidBlockBoundingPrefab, 157, 1, 4, 4);
        CreateDownBlocks(solidBlockPrefab, solidBlockBoundingPrefab, 158, 1, 3, 3);
        CreateDownBlocks(solidBlockPrefab, solidBlockBoundingPrefab, 159, 1, 2, 2);
        CreateDownBlocks(solidBlockPrefab, solidBlockBoundingPrefab, 160, 1, 1, 1);

        CreateDownBlocks(solidBlockPrefab, solidBlockBoundingPrefab, 179, 1, 1, 1);
        CreateDownBlocks(solidBlockPrefab, solidBlockBoundingPrefab, 180, 1, 2, 2);
        CreateDownBlocks(solidBlockPrefab, solidBlockBoundingPrefab, 181, 1, 3, 3);
        CreateDownBlocks(solidBlockPrefab, solidBlockBoundingPrefab, 182, 1, 4, 4);
        CreateDownBlocks(solidBlockPrefab, solidBlockBoundingPrefab, 183, 1, 5, 5);
        CreateDownBlocks(solidBlockPrefab, solidBlockBoundingPrefab, 184, 1, 6, 6);
        CreateDownBlocks(solidBlockPrefab, solidBlockBoundingPrefab, 185, 1, 7, 7);
        CreateDownBlocks(solidBlockPrefab, solidBlockBoundingPrefab, 186, 1, 8, 8);
        CreateDownBlocks(solidBlockPrefab, solidBlockBoundingPrefab, 187, 1, 8, 8);

        //Pipes
        CreatePipe(29, 2, 1);
        CreatePipe(39, 3, 2);
        CreatePipe(47, 4, 4);
        CreatePipe(58, 4, 4);
        CreatePipe(165, 2, 1);
        CreatePipe(177, 2, 1);

        //Breakables & Questions
        CreateInteractiveBlocks(questionBlockPrefab, 17, 1, 4, 1);
        CreateInteractiveBlocks(breakBlockPrefab, 21, 1, 4, 1);
        CreateInteractiveBlocks(mushroomQuestionBlockPrefab, 22, 1, 4, 1);
        CreateInteractiveBlocks(breakBlockPrefab, 23, 1, 4, 1);
        CreateInteractiveBlocks(questionBlockPrefab, 24, 1, 4, 1);
        CreateInteractiveBlocks(breakBlockPrefab, 25, 1, 4, 1);
        CreateInteractiveBlocks(questionBlockPrefab, 23, 1, 8, 1);

        CreateInteractiveBlocks(breakBlockPrefab, 78, 1, 4, 1);
        CreateInteractiveBlocks(mushroomQuestionBlockPrefab, 79, 1, 4, 1);
        CreateInteractiveBlocks(breakBlockPrefab, 80, 1, 4, 1);
        CreateInteractiveBlocks(breakBlockPrefab, 81, 8, 8, 1);
        CreateInteractiveBlocks(breakBlockPrefab, 92, 3, 8, 1);
        CreateInteractiveBlocks(questionBlockPrefab, 95, 1, 8, 1);
        CreateInteractiveBlocks(questionBlockPrefab, 95, 1, 4, 1);

        CreateInteractiveBlocks(breakBlockPrefab, 101, 1, 4, 1);
        CreateInteractiveBlocks(questionBlockPrefab, 102, 1, 4, 1);

        CreateInteractiveBlocks(questionBlockPrefab, 107, 1, 4, 1);
        CreateInteractiveBlocks(questionBlockPrefab, 110, 1, 4, 1);
        CreateInteractiveBlocks(mushroomQuestionBlockPrefab, 110, 1, 8, 1);
        CreateInteractiveBlocks(questionBlockPrefab, 113, 1, 4, 1);

        CreateInteractiveBlocks(breakBlockPrefab, 119, 1, 4, 1);
        CreateInteractiveBlocks(breakBlockPrefab, 122, 3, 8, 1);

        CreateInteractiveBlocks(breakBlockPrefab, 127, 1, 8, 1);
        CreateInteractiveBlocks(questionBlockPrefab, 128, 1, 8, 1);
        CreateInteractiveBlocks(questionBlockPrefab, 129, 1, 8, 1);
        CreateInteractiveBlocks(breakBlockPrefab, 130, 1, 8, 1);

        CreateInteractiveBlocks(breakBlockPrefab, 128, 1, 4, 1);
        CreateInteractiveBlocks(breakBlockPrefab, 129, 1, 4, 1);

        CreateInteractiveBlocks(breakBlockPrefab, 170, 2, 4, 1);
        CreateInteractiveBlocks(questionBlockPrefab, 172, 1, 4, 1);
        CreateInteractiveBlocks(breakBlockPrefab, 173, 1, 4, 1);

        //Enemies
        SetEnemy(goombaPrefab, 22, 1);
        SetEnemy(goombaPrefab, 42, 1);
        SetEnemy(goombaPrefab, 52,1);
        SetEnemy(goombaPrefab, 53,1);

        SetEnemy(goombaPrefab, 82,9);
        SetEnemy(goombaPrefab, 85,9);

        SetEnemy(goombaPrefab, 97, 1);
        SetEnemy(goombaPrefab, 98, 1);

        SetEnemy(goombaPrefab, 123, 1);
        SetEnemy(goombaPrefab, 124, 1);
        SetEnemy(goombaPrefab, 126, 1);
        SetEnemy(goombaPrefab, 127, 1);

        SetEnemy(goombaPrefab, 171, 1);
        SetEnemy(goombaPrefab, 172, 1);

        SetEnemy(koopaPrefab, 108, 1);

        //Flag
        SetFlag(196,1);
    }

    bool jidoStarted = false;

    public void SetEnemy(GameObject enemy,int iniXBlock, int iniYBlock)
    {
        Instantiate(enemy, new Vector3(iniXBlock * BLOCK_SIZE, iniYBlock * BLOCK_SIZE, 0), Quaternion.identity);
    }

    public void SetFlag(int iniXBlock,int iniYBlock)
    {
        CreateDownBlocks(solidBlockPrefab, solidBlockBoundingPrefab, iniXBlock,1, iniYBlock, 1);
        Instantiate(flagPrefab, new Vector3(iniXBlock * BLOCK_SIZE, iniYBlock * BLOCK_SIZE + flagPrefab.GetComponent<BoxCollider2D>().size.y / 2, 0), Quaternion.identity);
    }

    public void CreateInteractiveBlocks(GameObject controller, int iniXBlock, int numberOfXBlocks, int iniYBlock, int numberOfYBlocks)
    {
        GameObject f;

        for (int i = 0; i < numberOfXBlocks; i++)
        {
            for (int j = 0; j < numberOfYBlocks; j++)
                f = (GameObject)Instantiate(controller, new Vector3(iniXBlock * BLOCK_SIZE + i * BLOCK_SIZE, iniYBlock * BLOCK_SIZE - j * BLOCK_SIZE, 0), Quaternion.identity);
        }
    }

    public void CreateDownBlocks(GameObject sprite,GameObject bounding, int iniXBlock, int numberOfXBlocks, int iniYBlock, int numberOfYBlocks)
    {
        GameObject f, c;

        c = (GameObject)Instantiate(bounding, new Vector3(iniXBlock * BLOCK_SIZE - BLOCK_SIZE / 2 + (numberOfXBlocks * BLOCK_SIZE) / 2, iniYBlock * BLOCK_SIZE - BLOCK_SIZE / 2 - (numberOfYBlocks * BLOCK_SIZE) / 2 + BLOCK_SIZE, 0), Quaternion.identity);
        c.GetComponent<BoxCollider2D>().size = new Vector2(numberOfXBlocks * BLOCK_SIZE, numberOfYBlocks * BLOCK_SIZE);


        for (int i = 0; i < numberOfXBlocks; i++)
        {
            for(int j=0;j < numberOfYBlocks; j++)
                f = (GameObject)Instantiate(sprite, new Vector3(iniXBlock * BLOCK_SIZE + i * BLOCK_SIZE, iniYBlock * BLOCK_SIZE - j * BLOCK_SIZE, 0), Quaternion.identity);
        }
    }

    public void CreatePipe(int iniXBlock, int iniYBlock, int height)
    {
        GameObject f, c, sprite;

        if (height == 1)
        {
            c = (GameObject)Instantiate(pipeBoundingPrefab, new Vector3(iniXBlock * BLOCK_SIZE + BLOCK_SIZE/2, iniYBlock * BLOCK_SIZE - BLOCK_SIZE/2, 0), Quaternion.identity);
            c.GetComponent<BoxCollider2D>().size = new Vector2(2 * BLOCK_SIZE, 2 * BLOCK_SIZE);
        }
        else
        {
            c = (GameObject)Instantiate(pipeBoundingPrefab, new Vector3(iniXBlock * BLOCK_SIZE + BLOCK_SIZE / 2, iniYBlock * BLOCK_SIZE - (2 * BLOCK_SIZE + BLOCK_SIZE / 2 + (BLOCK_SIZE / 2) * (height - 1)) / 2 + BLOCK_SIZE - BLOCK_SIZE / 2, 0), Quaternion.identity);
            c.GetComponent<BoxCollider2D>().size = new Vector2(2 * BLOCK_SIZE, 2 * BLOCK_SIZE + BLOCK_SIZE/2 + (BLOCK_SIZE / 2) * (height - 1));
        }

        for (int i = 0; i < height; i++)
        {
            if (i == 0)
            {
                sprite = pipeBasePrefab;
                f = (GameObject)Instantiate(sprite, new Vector3(iniXBlock * BLOCK_SIZE + BLOCK_SIZE/2, iniYBlock * BLOCK_SIZE - BLOCK_SIZE/2, 0), Quaternion.identity);
            }
            else
            {
                sprite = pipePartPrefab;
                f = (GameObject)Instantiate(sprite, new Vector3(iniXBlock * BLOCK_SIZE + BLOCK_SIZE / 2, iniYBlock * BLOCK_SIZE - BLOCK_SIZE - i * (BLOCK_SIZE / 2) - BLOCK_SIZE / 2, 0), Quaternion.identity);
            }
        }
    }

    protected override void Update()
    {
        base.Update();

        if (isGameRunning)
        {
            //downLimit = rhythmFactory.GetLowestPosY();

            //Check if it's game over
            if (!isGameOver)
            {
                time -= Time.deltaTime;

                if (time <= 0)
                    GameOver();
            }
        }
    }

    protected override void UpdateCoinsValues()
    {
        coinsText.text = "x " + pickedCoins.ToString();
    }

    protected override void GameOver()
    {
        base.GameOver();

        if (performanceData != null)
        {
            SaveTime();
            IncreaseDeaths();

            //SavePerformance("lose");

            Debug.Log("enemies: " + performanceData.killedEnemies);
            Debug.Log("mushrooms: " + performanceData.pickedMushrooms);
            Debug.Log("bonus: " + performanceData.pickedBonus);
            Debug.Log("dead: " + performanceData.deadTimes);
            Debug.Log("broken: " + performanceData.brokenBlocks);
            Debug.Log("time: " + performanceData.completedTime);

            //Reset values
            performanceData.killedEnemies = 0;
            performanceData.pickedMushrooms = 0;
            performanceData.pickedBonus = 0;
            performanceData.brokenBlocks = 0;
            performanceData.completedTime = 0;
        }

        DisattachNeurosky();

        Application.LoadLevel(Names.GCGameOverScene);
    }
}
