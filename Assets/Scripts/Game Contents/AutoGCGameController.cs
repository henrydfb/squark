using UnityEngine;
using System.Collections;
using System.IO;

public class AutoGCGameController : GameController 
{
    public enum SolidBlockSide
    {
        Left,
        Right
    }

    public const float BLOCK_SIZE = 0.32f;

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

    //Constraints
    const int LEVEL_WIDTH = 200;
    const int LEVEL_HEIGHT = 8;
    const int NUMBER_OF_INITIAL_BLOCKS = 10;
    const int NUMBER_OF_FINAL_BLOCKS = 10;

    const int MIN_GAPS_WIDTH = 2;
    const int MAX_GAPS_WIDTH = 3;

    const int MIN_GAPS = 0;
    const int MAX_GAPS = 8;

    const int MIN_SOLID = 1;
    const int MAX_SOLID = 4;

    const int MIN_SOLID_WIDTH = 4;
    const int MAX_SOLID_WIDTH = 9;

    const int MIN_SOLID_HEIGHT = 4;
    const int MAX_SOLID_HEIGHT = 8;

    const int MIN_PIPES = 3;
    const int MAX_PIPES = 10;

    const int MIN_PIPES_HEIGHT = 2;
    const int MAX_PIPES_HEIGHT = 4;

    const int MIN_PIPES_SEP = 6;
    const int MAX_PIPES_SEP = 10;

    const int MIN_BREAKABLES = 15;
    const int MAX_BREAKABLES = 45;

    const int MIN_BONUS = 12;
    const int MAX_BONUS = 36;

    const int MIN_MUSHROOMS = 3;
    const int MAX_MUSHROOMS = 6;

    const int MIN_ENEMIES = 15;
    const int MAX_ENEMIES = 30;

    const int MIN_INTERACTIVE_ROW = 1;
    const int MAX_INTERACTIVE_ROW = 5;

    const int MIN_INTERACTIVE_Y = 4;
    const int MAX_INTERACTIVE_Y = 8;

    int currHighestY;
    int currHighestX;
    bool prevLeftSolid;

    protected override void Start()
    {
        string path;
        string perFileName = Globals.AUX_FILE_NAME;

        time = 60 * 2;

        base.Start();

        currLevelType = "auto";
        gameStarted = false;
        startTimer = 0;

        if(GameObject.Find(Names.FirstPlatform) != null)
            lastPlatform = GameObject.Find(Names.FirstPlatform).GetComponent<PlatformController>();
        cameraController = Camera.main.GetComponent<CameraController>();
        previousAttentionAverage = 0;
        currentSec = 0;
        currHighestY = 0;
        currHighestX = 0;
        prevLeftSolid = false;

        if (performanceData == null)
        {
            path = Globals.ROOT_FOLDER + "\\" + Globals.SUBJECT_NAME + "\\" + perFileName;

            if (File.Exists(path))
            {
                var sr = new StreamReader(path);
                var fileContents = sr.ReadToEnd();
                sr.Close();
                var lines = fileContents.Split("\n"[0]);

                BuildLevel(int.Parse(lines[0]), int.Parse(lines[1]), int.Parse(lines[2]), int.Parse(lines[3]), int.Parse(lines[4]), int.Parse(lines[5]));
            }
            else
            {
                Debug.Log("file does'nt exist");
                BuildLevel(MIN_ENEMIES, 2, MIN_BONUS, 0, MIN_BREAKABLES, 60);
            }

            performanceData = ((GameObject)Instantiate(performanceDataPref, new Vector3(), Quaternion.identity)).GetComponent<PerformanceData>();
            DontDestroyOnLoad(performanceData);
        }
        else
        {
            Debug.Log("enemies: " + performanceData.killedEnemies);
            Debug.Log("mushrooms: " + performanceData.pickedMushrooms);
            Debug.Log("bonus: " + performanceData.pickedBonus);
            Debug.Log("dead: " + performanceData.deadTimes);
            Debug.Log("broken: " + performanceData.brokenBlocks);
            Debug.Log("time: " + performanceData.completedTime);

            BuildLevel(performanceData.killedEnemies, performanceData.pickedMushrooms, performanceData.pickedBonus, performanceData.deadTimes, performanceData.brokenBlocks, performanceData.completedTime);

            //Reset values
            if (performanceData.firstTime)
            {
                performanceData.deadTimes = 0;
                performanceData.firstTime = false;
            }

            performanceData.killedEnemies = 0;
            performanceData.pickedMushrooms = 0;
            performanceData.pickedBonus = 0;
            performanceData.brokenBlocks = 0;
            performanceData.completedTime = 0;
        }

        //Flag
        SetFlag(196,1);
    }

    bool jidoStarted = false;

    public void BuildLevel(int killedEnemies, int pickedMushrooms,int pickedBonus, int deadTimes, int brokenBlocks, int completedTime)
    {
        float enemiesPer, mushroomsPer, bonusPer, blocksPer, deadPer, timePer, totalPer, segmentPer;
        int numberOfGaps, gapsBlock, floorEndX, lastFloorX, gapW, numBlocks, numberOfPipes, numberOfSolid, pipesPerSeg, solidPerSeg, leftBlocks, rightBlocks, numberOfBreak, numberOfBonus, numberOfMushrooms;
        int setPipes, setSolids, setMushrooms, setBonus, setBreak, interX, setInter, numberOfEnemies, enemiesX, newEnemies;
        int[] lastUsedBlocks;

        //The higher % the better the player performed
        //0%...100%
        enemiesPer = killedEnemies * 1 / MIN_ENEMIES; //Min enemies is the max in the first level
        mushroomsPer = pickedMushrooms * 1 / MIN_MUSHROOMS; //Min mushrooms is the max in the first level
        bonusPer = pickedBonus * 1 / MIN_BONUS; //Min bonus is the max in the first level
        blocksPer = brokenBlocks * 1 / MIN_BREAKABLES;
        timePer = completedTime * 1 / GCGameController.GAME_TIME;
        deadPer = 1 / (deadTimes + 1);

        //Performance %
        totalPer = (enemiesPer + mushroomsPer + bonusPer + blocksPer + timePer + deadPer) / 6;

        //Number of elements to build
        numberOfGaps = MIN_GAPS + (int)Mathf.Ceil((MAX_GAPS - MIN_GAPS) * totalPer);
        numberOfPipes = MIN_PIPES + (int)Mathf.Ceil((MAX_PIPES - MIN_PIPES) * totalPer);
        numberOfSolid = MIN_SOLID + (int)Mathf.Ceil((MAX_SOLID - MIN_SOLID) * totalPer);
        numberOfBonus = MIN_BONUS + (int)Mathf.Ceil((MAX_BONUS - MIN_BONUS) * totalPer);
        numberOfMushrooms = MIN_MUSHROOMS + (int)Mathf.Ceil((MAX_MUSHROOMS - MIN_MUSHROOMS) * totalPer);
        numberOfBreak = MIN_BREAKABLES + (int)Mathf.Ceil((MAX_BREAKABLES - MIN_BREAKABLES) * totalPer);
        numberOfEnemies = MIN_ENEMIES + (int)Mathf.Ceil((MAX_ENEMIES - MIN_ENEMIES) * totalPer);

        /*Debug.Log("gaps " + numberOfGaps);
        Debug.Log("pipes " + numberOfPipes);
        Debug.Log("solids " + numberOfSolid);
        Debug.Log("bonus " + numberOfBonus);
        Debug.Log("mushrooms " + numberOfMushrooms);
        Debug.Log("break " + numberOfBreak);
        Debug.Log("enes " + numberOfEnemies);*/

        if (numberOfGaps <= 0)
            numberOfGaps = 1;

        //Floor
        gapsBlock = (int)Mathf.Ceil((NUMBER_OF_BLOCKS - (NUMBER_OF_INITIAL_BLOCKS + NUMBER_OF_FINAL_BLOCKS))  / numberOfGaps);

        lastFloorX = NUMBER_OF_INITIAL_BLOCKS;
        numBlocks = 0;
        setPipes = 0;
        setSolids = 0;
        setMushrooms = 0;
        setBonus = 0;
        setBreak = 0;

        for (int i = 0; i <= numberOfGaps; i++)
        {
            gapW = Random.Range(MIN_GAPS_WIDTH, MAX_GAPS_WIDTH + 1);

            floorEndX = Random.Range(1,gapsBlock);

            if (i == 0)
                numBlocks = floorEndX + NUMBER_OF_INITIAL_BLOCKS;
            else if (i == numberOfGaps)
                numBlocks = NUMBER_OF_BLOCKS - lastFloorX + NUMBER_OF_FINAL_BLOCKS;
            else
                numBlocks = floorEndX;

            segmentPer = (float)numBlocks / (float)NUMBER_OF_BLOCKS;

            currHighestX = 0;
            currHighestY = 0;
            
            //Floor
            CreateDownBlocks(downBlockPrefab, downBlockBoundingPrefab, lastFloorX - NUMBER_OF_INITIAL_BLOCKS,numBlocks, 0, 2);

            if (setPipes >= numberOfPipes)
                pipesPerSeg = 0;
            else
            {
                pipesPerSeg = Mathf.RoundToInt(numberOfPipes * segmentPer);
                setPipes += pipesPerSeg;
            }

            if (setSolids >= numberOfSolid)
                solidPerSeg = 0;
            else
            {
                if (segmentPer * Random.Range(0, 10) > 0.5)
                {
                    solidPerSeg = 1;// Mathf.RoundToInt(numberOfSolid * segmentPer);
                    setSolids += solidPerSeg;
                }
                else
                    solidPerSeg = 0;
            }

            lastUsedBlocks = null;
            leftBlocks = 0;
            rightBlocks = 0;

            if (i == 0)
            {
                //Pipes
                if ((numBlocks - NUMBER_OF_INITIAL_BLOCKS) >= 2 * pipesPerSeg + (pipesPerSeg - 1) * MAX_PIPES_SEP && pipesPerSeg != 0)
                    lastUsedBlocks = BuildPipes(pipesPerSeg, lastFloorX, numBlocks - NUMBER_OF_INITIAL_BLOCKS);

                //Solids
                if (lastUsedBlocks != null && solidPerSeg != 0)
                {
                    leftBlocks = lastUsedBlocks[0];
                    rightBlocks = numBlocks - lastUsedBlocks[1];

                    SetSolidBlocks(pipesPerSeg, lastFloorX, numBlocks - NUMBER_OF_INITIAL_BLOCKS, lastUsedBlocks, leftBlocks, rightBlocks, i == numberOfGaps, i == 0);
                }
                else
                    prevLeftSolid = false;

                //Interactives
                //Fit in the left side
                if (Random.Range(0, 100) >= 30)
                {
                    if (leftBlocks > rightBlocks)
                    {
                        interX = lastFloorX + Random.Range(0, 3);
                    }
                    else
                    {
                        if(lastUsedBlocks == null)
                            interX = lastFloorX + Random.Range(0, 3);
                        else
                            interX = lastUsedBlocks[1] + Random.Range(0, 3);
                    }

                    if (currHighestY >= MIN_INTERACTIVE_Y)
                    {
                        BuildInteractives(currHighestX + Random.Range(2, 4), MAX_INTERACTIVE_Y, Random.Range(1, 3), Random.Range(2, 5), Random.Range(1, 1));
                        BuildInteractives(currHighestX + Random.Range(5, 8), MIN_INTERACTIVE_Y, 0, 1, 0);
                    }
                    else
                        BuildInteractives(interX, MIN_INTERACTIVE_Y, Random.Range(1, 3), Random.Range(2, 5), Random.Range(1, 1));
                }

                if (leftBlocks > rightBlocks)
                {
                    newEnemies = (int)Mathf.Ceil(numberOfEnemies / (numberOfGaps + 1));//Mathf.Min(leftBlocks, (int)Mathf.Ceil(numberOfEnemies / (numberOfGaps + 1)));
                    enemiesX =  (int)Random.Range(lastFloorX, lastFloorX + leftBlocks - newEnemies);
                }
                else
                {
                    newEnemies = (int)Mathf.Ceil(numberOfEnemies / (numberOfGaps + 1));// Mathf.Min(rightBlocks, (int)Mathf.Ceil(numberOfEnemies / (numberOfGaps + 1)));

                    if (lastUsedBlocks == null)
                        enemiesX =  (int)Random.Range(lastFloorX, lastFloorX + leftBlocks - Mathf.Round(numberOfEnemies / numberOfGaps));
                    else
                        enemiesX = (int)Random.Range(lastUsedBlocks[1], lastUsedBlocks[1] + rightBlocks - newEnemies);
                }

                //Enemies
                for (int j = 0; j < newEnemies; j++ )
                {
                    if (Random.Range(0, 100) > 70)
                        SetEnemy(koopaPrefab, enemiesX + j, 1);
                    else
                        SetEnemy(goombaPrefab, enemiesX + j, 1);

                }

            }
            else
            {
                //Pipes
                if (numBlocks >= 2 * pipesPerSeg + (pipesPerSeg - 1) * MAX_PIPES_SEP && pipesPerSeg != 0)
                    lastUsedBlocks = BuildPipes(pipesPerSeg, lastFloorX - NUMBER_OF_INITIAL_BLOCKS, numBlocks);

                //Solids
                if (lastUsedBlocks != null && solidPerSeg != 0)
                {
                    leftBlocks = lastUsedBlocks[0] - (int)(lastFloorX - NUMBER_OF_INITIAL_BLOCKS);
                    rightBlocks = lastFloorX - NUMBER_OF_INITIAL_BLOCKS + numBlocks - lastUsedBlocks[1];

                    SetSolidBlocks(pipesPerSeg, lastFloorX - NUMBER_OF_INITIAL_BLOCKS, numBlocks, lastUsedBlocks, leftBlocks, rightBlocks, i == numberOfGaps, i == 0);
                }
                else
                    prevLeftSolid = false;

                //Interactives
                //Fit in the left side
                if (Random.Range(0, 100) >= 30)
                {
                    if (leftBlocks > rightBlocks)
                    {
                        interX = lastFloorX + Random.Range(0, 3);
                    }
                    else
                    {
                        if (lastUsedBlocks == null)
                            interX = lastFloorX + Random.Range(0, 3);
                        else
                            interX = lastUsedBlocks[1] + Random.Range(0, 3);
                    }

                    if (currHighestY >= MIN_INTERACTIVE_Y)
                    {
                        BuildInteractives(currHighestX + Random.Range(2, 4), MAX_INTERACTIVE_Y, Random.Range(1, 3), Random.Range(2, 5), Random.Range(0, 1));
                        BuildInteractives(currHighestX + Random.Range(5, 8), MIN_INTERACTIVE_Y, 0, 1, 0);
                    }
                    else
                        BuildInteractives(interX, MIN_INTERACTIVE_Y, Random.Range(1, 3), Random.Range(2, 5), Random.Range(0, 1));
                }

                if (leftBlocks > rightBlocks)
                {
                    newEnemies = (int)Mathf.Ceil(numberOfEnemies / (numberOfGaps + 1));// Mathf.Min(leftBlocks, (int)Mathf.Ceil(numberOfEnemies / (numberOfGaps + 1)));
                    enemiesX = (int)Random.Range(lastFloorX, lastFloorX + leftBlocks - newEnemies);
                    //Debug.Log("enemies " + leftBlocks);
                }
                else
                {
                    newEnemies = (int)Mathf.Ceil(numberOfEnemies / (numberOfGaps + 1));// Mathf.Min(rightBlocks, (int)Mathf.Ceil(numberOfEnemies / (numberOfGaps + 1)));

                    if (lastUsedBlocks == null)
                        enemiesX = (int)Random.Range(lastFloorX, lastFloorX + leftBlocks - Mathf.Round(numberOfEnemies / numberOfGaps));
                    else
                        enemiesX = (int)Random.Range(lastUsedBlocks[1], lastUsedBlocks[1] + rightBlocks - newEnemies);
                    //Debug.Log("enemies " + rightBlocks);
                }

                
                //Enemies
                for (int j = 0; j < newEnemies; j++)
                {
                    if (Random.Range(0, 100) > 70)
                        SetEnemy(koopaPrefab, enemiesX + j, 2);
                    else
                        SetEnemy(goombaPrefab, enemiesX + j, 2);

                }
            }

            lastFloorX += numBlocks + gapW;
        }
    }

    public void BuildInteractives(int iniX,int iniY, int bonus, int breakables,int mushrooms)
    { 
        int total, setBonus, setBreak, setMush, rand, posX;
        GameObject pref;

        total = bonus + breakables + mushrooms;
        setBonus = bonus;
        setBreak = breakables;
        setMush = mushrooms;
        posX = iniX;
        for (int i = 0; i < total; i++)
        {
            rand = Random.Range(0,100);

            if (setBonus > 0 && setBreak > 0 && setMush > 0)
            {
                //Bonus
                if (rand <= 100 / 4)
                {
                    pref = questionBlockPrefab;
                    setBonus--;
                }
                //Break
                else if (rand > 100 / 4 && rand <= 2 * (100 / 4))
                {
                    pref = breakBlockPrefab;
                    setBreak--;
                }
                //Mushroom
                else if (rand > 2 * (100 / 4) && rand <= 3 * (100 / 4))
                {
                    pref = mushroomQuestionBlockPrefab;
                    setMush--;
                }
                else
                {
                    pref = null;
                    i--;
                }
            }
            else if (setBonus <= 0)
            {
                if (setBreak <= 0)
                {
                    pref = mushroomQuestionBlockPrefab;
                    setMush--;
                }
                else if (setMush <= 0)
                {
                    pref = breakBlockPrefab;
                    setBreak--;
                }
                else
                {
                    //Mushroom
                    if (rand <= 100 / 2)
                    {
                        pref = breakBlockPrefab;
                        setBreak--;
                    }
                    //Breakable
                    else
                    {
                        pref = mushroomQuestionBlockPrefab;
                        setMush--;
                    }
                }
            }
            else if (setBreak <= 0)
            {
                if (setBonus <= 0)
                {
                    pref = mushroomQuestionBlockPrefab;
                    setMush--;
                }
                else if (setMush <= 0)
                {
                    pref = questionBlockPrefab;
                    setBonus--;
                }
                else
                {
                    //Bonus
                    if (rand <= 100 / 2)
                    {
                        pref = questionBlockPrefab;
                        setBonus--;
                    }
                    //Mushroom
                    else
                    {
                        pref = mushroomQuestionBlockPrefab;
                        setMush--;
                    }
                }
            }
            else if (setMush <= 0)
            {
                if (setBreak <= 0)
                {
                    pref = questionBlockPrefab;
                    setBonus--;
                }
                else if (setBonus <= 0)
                {
                    pref = breakBlockPrefab;
                    setBreak--;
                }
                else
                {
                    //Bonus
                    if (rand <= 100 / 2)
                    {
                        pref = questionBlockPrefab;
                        setBonus--;
                    }
                    //Breakable
                    else
                    {
                        pref = breakBlockPrefab;
                        setBreak--;
                    }
                }
            }
            else
                pref = null;

            if (pref != null)
                CreateInteractiveBlocks(pref, posX, 1, iniY, 1);

            posX++;
        }

    }

    protected override void UpdateCoinsValues()
    {
        coinsText.text = "x " + pickedCoins.ToString();
    }

    public void SetSolidBlocks(int numberOfSolids, int iniX, int numBlocks, int[] prevLastBlocks, int leftBlocks, int rightBlocks, bool lastFloor, bool firstFloor)
    {
        SolidBlockSide side;
        int width, height,posX;
        bool createSolid;

        posX = iniX;
        
        for (int i = 0; i < numberOfSolids; i++)
        {
            createSolid = false;
            
            side = SolidBlockSide.Left;
            if(lastFloor)
                height = MAX_SOLID_HEIGHT;
            else
                height = Random.Range(MIN_SOLID_HEIGHT, MAX_SOLID_HEIGHT);
            width = Random.Range(MIN_SOLID_WIDTH, MAX_SOLID_WIDTH);

            //Fit in the left side
            if (leftBlocks >= width)
            {
                //Fit in the right side
                if (rightBlocks >= width)
                {
                    //Fit in the left side
                    if (leftBlocks > rightBlocks)
                    {
                        if (!firstFloor)
                        {
                            if (prevLeftSolid)
                            {
                                posX = iniX;
                                createSolid = true;
                                side = SolidBlockSide.Right;
                                prevLeftSolid = false;
                            }
                        }
                    }
                    else
                    {
                        if (lastFloor)
                            posX = prevLastBlocks[1];
                        else
                            posX = iniX + numBlocks - width - 1;
                        createSolid = true;
                        side = SolidBlockSide.Left;
                        prevLeftSolid = true;
                    }
                }
                else
                {
                    if (!firstFloor)
                    {
                        if (prevLeftSolid)
                        {
                            posX = iniX;
                            createSolid = true;
                            side = SolidBlockSide.Right;
                            prevLeftSolid = false;
                        }
                        else
                        {
                        
                        }
                    }
                }
            }
            //Fit in the right side
            else if (rightBlocks >= width)
            {
                if (lastFloor)
                    posX = prevLastBlocks[1];
                else
                    posX = iniX + numBlocks - width - 1;
                createSolid = true;
                side = SolidBlockSide.Left;
                prevLeftSolid = true;
            }
            else
                Debug.Log("No cabe");

            if (createSolid)
            {
                BuildSolidBlock(posX, width, height, side);
                posX +=  width;

                //Current heighest point
                if (height > currHighestY)
                {
                    currHighestY = height;
                    currHighestX = posX;
                }
            }

        }
    }

    public void BuildSolidBlock(int initialX, int width, int height, SolidBlockSide side)
    {
        for (int i = 0; i <= width; i++)
        {
            switch (side)
            { 
                case SolidBlockSide.Left:
                    if(i <= height)
                        CreateDownBlocks(solidBlockPrefab, solidBlockBoundingPrefab, initialX + i, 1, i, i);
                    else
                        CreateDownBlocks(solidBlockPrefab, solidBlockBoundingPrefab, initialX + i, 1, height, height);
                    break;
                case SolidBlockSide.Right:
                    if (i <= height)
                        CreateDownBlocks(solidBlockPrefab, solidBlockBoundingPrefab, initialX + width - i, 1, i, i);
                    else
                        CreateDownBlocks(solidBlockPrefab, solidBlockBoundingPrefab, initialX + width - i, 1, height, height);
                    break;
            }
            
        }
    }

    public int[] BuildPipes(int numPipes,int iniX, int numBlocks)
    {
        int sepX, prevX, height, maxSep;
        int[] firstLastBlock;

        prevX = iniX;
        maxSep = numBlocks / MAX_PIPES_SEP;
        firstLastBlock = new int[2];
        firstLastBlock[0] = 0;
        firstLastBlock[1] = 0;
        for (int i = 0; i < numPipes; i++)
        {
            if(i == 0)
                sepX = Random.Range(0, Mathf.RoundToInt(numBlocks - 2) / numPipes);
            else
                sepX = Random.Range(MIN_PIPES_SEP, MAX_PIPES_SEP);

            height = Random.Range(MIN_PIPES_HEIGHT, MAX_PIPES_HEIGHT + 1);

            //Current heighest point
            if (height > currHighestY)
            {
                currHighestY = height;
                currHighestX = prevX + sepX;
            }

            //TODO: there is a mistake when we create the pipes ( the height of the pipe)
            CreatePipe(prevX + sepX, height, height);

            if(i == 0)
                firstLastBlock[0] = prevX + sepX;
            
            
            firstLastBlock[1] = prevX + sepX + 2;

            prevX += sepX;
        }

        return firstLastBlock;
    }

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

    protected override void GameOver()
    {
        base.GameOver();

        if (performanceData != null)
        {
            IncreaseDeaths();
            SaveTime();

            //SavePerformance("lose");
        }
        DisattachNeurosky();
        Application.LoadLevel("GCGameOverAuto");
    }
}
