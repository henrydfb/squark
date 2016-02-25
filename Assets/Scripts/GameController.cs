using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;

public class GameController : MonoBehaviour 
{
    public enum AttentionType
    {
        Low = 0,
        Med = 1,
        Hig = 2,
        Ene = 3,
        Gap = 4,
        Spk = 5
    }

    public enum DeathType
    {
        Low = 0,
        Med = 1,
        Hig = 2,
        Ene = 3,
        Gap = 4,
        Spk = 5
    }

    public const int NUMBER_OF_ATTENTION_POINT = 30;

    public bool IsGameNeurosky = true;

    protected bool isGameRunning = false;
    /// <summary>
    /// Left limit
    /// </summary>
    public float leftLimit;
    /// <summary>
    /// Right limit
    /// </summary>
    public float rightLimit;
    /// <summary>
    /// Down limit
    /// </summary>
    public float downLimit;
    /// <summary>
    /// UpLimit
    /// </summary>
    public float upLimit;

    public PersistentController persistentData;
    public RhythmPersistentController rhythmPersistentData;
    //Max time in seconds
    protected const float MAX_TIME = 60 * 10;
    protected const float MIN_TIME = 0;
    protected const int NUMBER_OF_BLOCKS = 200;
    //Current play time
    protected float time;
    protected GUIText timeText;
    protected GUIText levelText;
    protected GUIText coinsText;
    protected GUIText neuroskyStatsText;
    protected MessageController feedbackMessage;
    protected GameObject player;
    protected bool isGameOver;
    protected int numberOfCoins;
    protected int pickedCoins;

    protected ArrayList attValues;
    protected RhythmFactory rhythmFactory;


    //Neurosky values
    protected TGCConnectionController controller;
    protected int poorSignal1 = -1;
    protected int attention1;
    protected int meditation1;
    protected int blink;
    protected int indexSignalIcons = 1;
    protected float delta;
    protected bool gameStarted;
    protected int currAttSec;
    protected string currAct;
    protected string currLevelType;

    public GameObject performanceDataPref;
    protected PerformanceData performanceData;
    protected RhythmPersistentController rhythmPersistent;

    //Attention values
    protected List<float>[] attentionMatrix;
    //Death values
    protected int[] deathMatrix;

    private int[] attentionLevels;
    private int currentAttLvl;

    protected float globalValue;

    protected bool sentDeathData;

    void Awake()
    {
        GameObject perObj, rhythmObj;

        perObj = GameObject.FindGameObjectWithTag("Performance");
        if (perObj != null)
        {
            performanceData = perObj.GetComponent<PerformanceData>();
            if (performanceData != null)
                DontDestroyOnLoad(performanceData);
        }

        rhythmObj = GameObject.Find("PersistentRhythm");
        if (rhythmObj != null)
        {
            rhythmPersistent = rhythmObj.GetComponent<RhythmPersistentController>();
            DontDestroyOnLoad(rhythmObj);
        }
    }

    public int GetMeditation()
    {
        return meditation1;
    }

	// Use this for initialization
    protected virtual void Start() 
    {
        GameObject nkystsObj, perObj;

        //Persistent data
        perObj = GameObject.Find("PersistentObject");
        persistentData = perObj.GetComponent<PersistentController>();
        Object.DontDestroyOnLoad(perObj);

        timeText = GameObject.Find(Names.TimeText).GetComponent<GUIText>();
        levelText = GameObject.Find(Names.LevelText).GetComponent<GUIText>();
        coinsText = GameObject.Find(Names.CoinsText).GetComponent<GUIText>();
        feedbackMessage = GameObject.Find(Names.FeedbackMessage).GetComponent<MessageController>();
        
        nkystsObj = GameObject.Find(Names.NeuroskyStatsText);
        if(nkystsObj != null)
            neuroskyStatsText = nkystsObj.GetComponent<GUIText>();
        player = GameObject.Find(Names.Player);
        isGameOver = false;
        numberOfCoins = GameObject.FindGameObjectsWithTag(Names.Coin).Length;
        pickedCoins = 0;

        UpdateCoinsValues();

        //Neurosky
        if (IsGameNeurosky)
        {
            controller = GameObject.Find("NeuroSkyTGCController").GetComponent<TGCConnectionController>();
            controller.UpdatePoorSignalEvent += OnUpdatePoorSignal;
            controller.UpdateAttentionEvent += OnUpdateAttention;
            //controller.UpdateMeditationEvent += OnUpdateMeditation;
            //controller.UpdateBlinkEvent += OnUpdateBlink;
        }
        else
        {
            //Repeating
            InvokeRepeating("UpdateAttention", TGCConnectionController.NEUROSKY_INITIAL_TIME, 1);
            //InvokeRepeating("UpdateMeditation", TGCConnectionController.NEUROSKY_INITIAL_TIME, 1);
        }

        if (IsGameNeurosky)
        {
            if(controller.ConnectionWasSuccessful())
                feedbackMessage.Show(Names.ConnectingMessage);
        }

        attValues = new ArrayList();
        currAttSec = 0;
        currAct = "";

        //Att matrix
        attentionMatrix = new List<float>[6];

        attentionMatrix[(int)AttentionType.Ene] = new List<float>();
        attentionMatrix[(int)AttentionType.Gap] = new List<float>();
        attentionMatrix[(int)AttentionType.Hig] = new List<float>();
        attentionMatrix[(int)AttentionType.Low] = new List<float>();
        attentionMatrix[(int)AttentionType.Med] = new List<float>();
        attentionMatrix[(int)AttentionType.Spk] = new List<float>();

        //Death matrix
        if (persistentData.deathMatrix.Length == 0)
        {
            deathMatrix = new int[6];
            for (int i = 0; i < deathMatrix.Length; i++)
                deathMatrix[i] = 0; //We initialize this in -1 to identify when the value was initialized or not
        }
        else
            deathMatrix = persistentData.deathMatrix;

        currentAttLvl = 0;
        attentionLevels = new int[NUMBER_OF_ATTENTION_POINT];
        for (int i = 0; i < attentionLevels.Length; i++)
            attentionLevels[i] = -1; //We initialize this in -1 to identify when the value was initialized or not
        sentDeathData = false;
	}

    public void UpdateAttention()
    {
        OnUpdateAttention(Random.Range(0,100));
    }

    public void UpdateMeditation()
    {
        OnUpdateMeditation(Random.Range(0, 100));
    }

    public void SetCurrentAction(string act)
    {
        currAct = act;
    }

    public PerformanceData GetPerformanceData()
    {
        return performanceData;
    }

    public TGCConnectionController GetTGCConnectionController()
    {
        return controller;
    }

    public bool IsGameRunning()
    {
        return isGameRunning;
    }

    protected virtual void UpdateCoinsValues()
    {
        coinsText.text = pickedCoins.ToString("00") + "/" + numberOfCoins.ToString("");
    }

    protected void OnUpdatePoorSignal(int value)
    {
        poorSignal1 = value;
        if (value < 25)
        {
            indexSignalIcons = 0;
        }
        else if (value >= 25 && value < 51)
        {
            indexSignalIcons = 4;
        }
        else if (value >= 51 && value < 78)
        {
            indexSignalIcons = 3;
        }
        else if (value >= 78 && value < 107)
        {
            indexSignalIcons = 2;
        }
        else if (value >= 107)
        {
            indexSignalIcons = 1;
        }
    }

    protected void OnUpdateAttention(int value)
    {
        ArrayList elements;
        elements = new ArrayList(3);
        attention1 = value;

        elements.Add(currAttSec);
        elements.Add(value);
        elements.Add(currAct);
        attValues.Add(elements);

        currAttSec++;

        GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraController>().OnUpdateAttention(value);

        currAct = "";

        //Attention levels
        attentionLevels[currentAttLvl] = value;
        if (currentAttLvl + 1 < attentionLevels.Length)
            currentAttLvl++;
        else
            currentAttLvl = 0;
    }

    protected void OnUpdateMeditation(int value)
    {
        meditation1 = value;
        //Debug.Log("med " + value);

        Camera.main.GetComponent<CameraController>().OnUpdateAttention(value);
    }

    protected void OnUpdateBlink(int value)
    {
        blink = value;
        Debug.Log("blink " + value);
        //Make the player jump
        player.GetComponent<PlayerController>().Jump();
    }

    protected void OnUpdateDelta(float value)
    {
        delta = value;
    }
	
	// Update is called once per frame
    protected virtual void Update() 
    {
        PlatformController diePlatformL, diePlatformR;
        int min, sec;

        //Check if the game started
        if (IsGameNeurosky)
        {
            isGameRunning = poorSignal1 == 0 && controller.ConnectionWasSuccessful();
            if (isGameRunning)
                feedbackMessage.Hide();
        }
        else
            isGameRunning = true;

        min = (int)(time / 60);
        sec = (int)(time % 60);
        
        /*
        //Left Limit
        Debug.DrawLine(new Vector3(leftLimit, downLimit), new Vector3(leftLimit, upLimit), Color.red);
        //Right Limit
        Debug.DrawLine(new Vector3(rightLimit, downLimit), new Vector3(rightLimit, upLimit), Color.red);
        //Up Limit
        Debug.DrawLine(new Vector3(leftLimit, upLimit), new Vector3(rightLimit, upLimit), Color.red);
        //Down Limit
        Debug.DrawLine(new Vector3(leftLimit, downLimit), new Vector3(rightLimit, downLimit), Color.red);*/

        //If the game started
        if (isGameRunning)
        {
            //Check if it's game over
            if (isGameOver)
                GameOver();
            else
            {
                //Out of the screen
                isGameOver = (player.transform.position.y + player.GetComponent<Renderer>().bounds.size.y / 2 <= downLimit);// || player.GetComponent<PlayerController>().IsDead();

                if (isGameOver && !sentDeathData && !player.GetComponent<PlayerController>().IsDead())
                {
                    //Platforms
                    diePlatformL = FindClosestPlatform("left");
                    diePlatformR = FindClosestPlatform("right");
                    SaveGapData(diePlatformL, diePlatformR, "death");
                    player.GetComponent<PlayerController>().Die();
                    sentDeathData = true;
                }
            }
        }

        timeText.text = min.ToString("00") + ":" + sec.ToString("00");
        UpdateCoinsValues();
	}

    public void SetLevel(int level)
    {
        if(levelText == null)
            levelText = GameObject.Find(Names.LevelText).GetComponent<GUIText>();

        levelText.text = "Level " + level;
    }

    private PlatformController FindClosestPlatform(string side)
    {
        GameObject[] platformObjects;
        PlatformController platform;
        float distance;
        bool sideCond;

        platform = null;
        distance = Mathf.Infinity;

        platformObjects = GameObject.FindGameObjectsWithTag(Names.Platform);

        if (platformObjects.Length > 0)
        {
            foreach (GameObject s in platformObjects)
            {
                sideCond = false;
                switch (side)
                {
                    case "left":
                        sideCond = player.transform.position.x > s.transform.position.x;
                        break;
                    case "right":
                        sideCond = player.transform.position.x < s.transform.position.x;
                        break;
                }

                if ((Vector2.Distance(player.transform.position, s.transform.position) < distance) && sideCond)
                {
                    platform = s.GetComponent<PlatformController>();
                    distance = Vector2.Distance(player.transform.position, s.transform.position);
                }
            }
        }

        return platform;
    }

    public void SaveGapData(PlatformController platform1, PlatformController platform2,string dataType)
    {
        double dist;

        //Platforms
        if (platform1 != null & platform2 != null)
        {
            if (platform1 != platform2)
            {
                //Current is at the left side
                if (platform1.transform.position.x < platform2.transform.position.x)
                    dist = (platform1.transform.position.x + (platform1.GetComponent<BoxCollider2D>().bounds.size.x / 2)) - (platform2.transform.position.x - (platform2.GetComponent<BoxCollider2D>().bounds.size.x / 2));
                else
                    dist = (platform1.transform.position.x - (platform1.GetComponent<BoxCollider2D>().bounds.size.x / 2)) - (platform2.transform.position.x + (platform2.GetComponent<BoxCollider2D>().bounds.size.x / 2));

                dist = System.Math.Abs(System.Math.Round(dist, 1));

                //Update attention
                switch (dataType)
                { 
                    case "death":
                        break;
                    case "jump":
                        break;
                }

                //Low Jump platform
                //What we are doing here is: calclulate the distance between gaps and check which type do they belong to
                if (dist >= 0.8f && dist <= 1.0f)
                {
                    if(dataType == "jump")
                        AddAttentionValue(GameController.AttentionType.Low);
                    else if (dataType == "death")
                        AddDeath(GameController.DeathType.Low);
                }
                //Med Jump platform
                else if (dist >= 1.4f && dist <= 1.6f)
                {
                    if (dataType == "jump")
                        AddAttentionValue(GameController.AttentionType.Med);
                    else if (dataType == "death")
                        AddDeath(GameController.DeathType.Med);
                }
                //High Jump platform
                else
                {
                    if (dataType == "jump")
                        AddAttentionValue(GameController.AttentionType.Hig);
                    else if (dataType == "death")
                        AddDeath(GameController.DeathType.Hig);
                }

                if (dataType == "jump")
                    AddAttentionValue(GameController.AttentionType.Gap);
                else if (dataType == "death")
                    AddDeath(GameController.DeathType.Gap);
            }
        }
    }

    public int GetNumberOfDeaths()
    {
        return deathMatrix[(int)DeathType.Ene] + deathMatrix[(int)DeathType.Gap] + deathMatrix[(int)DeathType.Spk];
    }

    protected virtual void GameOver()
    {
        CleanScene();
    }

    public bool GameStarted()
    {
        return gameStarted;
    }

    protected void CleanScene()
    {
        if (IsGameNeurosky)
        {
            controller.UpdatePoorSignalEvent -= OnUpdatePoorSignal;
            controller.UpdateAttentionEvent -= OnUpdateAttention;
            controller.UpdateMeditationEvent -= OnUpdateMeditation;
            controller.UpdateBlinkEvent -= OnUpdateBlink;
            controller.UpdateDeltaEvent -= OnUpdateDelta;
        }
    }

    public void DisattachNeurosky()
    {
        if(IsGameNeurosky)
        {
            /*controller.UpdatePoorSignalEvent -= OnUpdatePoorSignal;
            controller.UpdateAttentionEvent -= OnUpdateAttention;*/

            Debug.Log("finish");
            controller.UpdatePoorSignalEvent -= OnUpdatePoorSignal;
            controller.UpdateAttentionEvent -= OnUpdateAttention;
            controller.UpdateMeditationEvent -= OnUpdateMeditation;
            controller.UpdateBlinkEvent -= OnUpdateBlink;
            controller.UpdateDeltaEvent -= OnUpdateDelta;
        }
    }

    public bool IsGameOver()
    {
        return isGameOver;
    }

    public void PickCoin()
    {
        pickedCoins++;
    }

    public bool WonGame()
    {
        return pickedCoins >= numberOfCoins;
    }

    protected virtual void OnGUI()
    {
        if (neuroskyStatsText != null)
        {
            neuroskyStatsText.text = "STATS:\n\n\n\n\n\n\n\n\n\n\n\n";
            neuroskyStatsText.text += "Poor Signal: " + poorSignal1 + "\n";
            neuroskyStatsText.text += "Attention: " + attention1 + "\n";
            /*neuroskyStatsText.text += "Meditation: " + meditation1 + "\n";
            neuroskyStatsText.text += "Blink: " + blink + "\n";
            neuroskyStatsText.text += "Delta: " + delta + "\n";*/
        }
    }

    public float GetAttentionValuesAvg()
    {
        float attAverage;
        int attentionSum, attCount;

        attentionSum = 0;
        attCount = 0;
        foreach (int att in attentionLevels)
        {
            if (att >= 0)
            {
                attentionSum += att;
                attCount++;
            }
        }

        attAverage = attentionSum / attCount;

        return attAverage;
    }

    public void AddAttentionValue(AttentionType index)
    {
        float avg;

        avg = GetAttentionValuesAvg();
        attentionMatrix[(int)index].Add(avg);
    }

    public void AddDeath(DeathType type)
    {
        deathMatrix[(int)type]++;
    }

    public void OnDestroy()
    {
        //if(IsGameNeurosky)
          //  controller.Disconnect();
    }

    public void BeatEnemy()
    {
        if (performanceData != null)
            performanceData.killedEnemies++;
    }

    public void PickBonus()
    {
        if (performanceData != null)
            performanceData.pickedBonus++;
    }

    public void PickMushroom()
    {
        if (performanceData != null)
            performanceData.pickedMushrooms++;
    }

    public void BreakBlock()
    {
        if (performanceData != null)
            performanceData.brokenBlocks++;
    }

    public void SaveTime()
    {
        if (performanceData != null)
            performanceData.completedTime = Mathf.RoundToInt(time);
    }

    public void IncreaseDeaths()
    {
        if (performanceData != null)
            performanceData.deadTimes++;
    }

    public void SavePerformance(int level,float gameTime,float avgAttention,Rhythm rhythm,Geometry geometry,List<float>[] attentionMatrix,int[] deathMatrix)
    {
        StreamWriter file;

        string path, date, perFileName;

        date = System.DateTime.Today.ToString("dd-MM-yyyy") + " " + System.DateTime.Now.ToString("HH:mm:ss");

        perFileName = Globals.METHOD + "-level-" + level + "-" + System.DateTime.Today.ToString("dd-MM-yyyy") + ".txt";

        //Performance File
        path = Globals.ROOT_FOLDER + "\\" + Globals.SUBJECT_NAME + "\\" + perFileName;

        if (File.Exists(path))
        {
            Debug.Log(perFileName + " already exists.");
            return;
        }

        file = File.CreateText(path);

        file.WriteLine("*****EXPERIMENT RESULTS******");
        file.WriteLine("Subject: " + Globals.SUBJECT_NAME);
        file.WriteLine("");
        file.WriteLine("Date: " + date);
        file.WriteLine("Level: " + level);
        file.WriteLine("");
        file.WriteLine("***Rhythm-Group***");
        file.WriteLine("");
        file.WriteLine("Performance: " + rhythm.GetGlobalPerformance() / 100);
        file.WriteLine("Difficulty: " + rhythm.GetDifficulty());
        file.WriteLine("Time: " + rhythm.GetTime());
        file.WriteLine("Actions: " + rhythm.GetActions().Length);
        file.WriteLine("Low rate: " + rhythm.GetLowActionRate());
        file.WriteLine("Med rate: " + rhythm.GetMedActionRate());
        file.WriteLine("Hig rate: " + rhythm.GetHigActionRate());
        file.WriteLine("Ene rate: " + geometry.GetEneActRate());
        file.WriteLine("Gap rate: " + geometry.GetGapActRate());
        file.WriteLine("Spk rate: " + geometry.GetSpkActRate());
        file.WriteLine("");
        file.WriteLine("");
        file.WriteLine("***Performance***");
        file.WriteLine("");
        file.WriteLine("Gametime: " + gameTime);
        if (deathMatrix != null)
        {
            if (deathMatrix.Length >= 6)
            {
                file.WriteLine("Total deaths: " + (deathMatrix[(int)DeathType.Ene] + deathMatrix[(int)DeathType.Gap] + deathMatrix[(int)DeathType.Spk]));
                file.WriteLine("Gap deaths: " + deathMatrix[(int)DeathType.Gap]);
                file.WriteLine("Ene deaths: " + deathMatrix[(int)DeathType.Ene]);
                file.WriteLine("Spk deaths: " + deathMatrix[(int)DeathType.Spk]);
                file.WriteLine("Low deaths: " + deathMatrix[(int)DeathType.Low]);
                file.WriteLine("Med deaths: " + deathMatrix[(int)DeathType.Med]);
                file.WriteLine("Hig deaths: " + deathMatrix[(int)DeathType.Hig]);
            }
        }
        file.WriteLine("");
        file.WriteLine("***Attention***");
        file.WriteLine("");
        file.WriteLine("Average attention: " + avgAttention);
        file.WriteLine("Attention Values:");
        if (attentionMatrix != null)
        {
            if (attentionMatrix.Length >= 6)
            {
                file.WriteLine("Attention Ene");
                foreach (float val in attentionMatrix[(int)AttentionType.Ene])
                    file.WriteLine(val);

                file.WriteLine("Attention Gap");
                foreach (float val in attentionMatrix[(int)AttentionType.Gap])
                    file.WriteLine(val);

                file.WriteLine("Attention Spk");
                foreach (float val in attentionMatrix[(int)AttentionType.Spk])
                    file.WriteLine(val);

                file.WriteLine("Attention Low");
                foreach (float val in attentionMatrix[(int)AttentionType.Low])
                    file.WriteLine(val);

                file.WriteLine("Attention Med");
                foreach (float val in attentionMatrix[(int)AttentionType.Med])
                    file.WriteLine(val);

                file.WriteLine("Attention Hig");
                foreach (float val in attentionMatrix[(int)AttentionType.Hig])
                    file.WriteLine(val);
            }
        }

        file.Close();
    }

    public void SaveWinAux()
    {
        StreamWriter perFile;

        string path;
        string perFileName = Globals.AUX_FILE_NAME;

        path = Globals.ROOT_FOLDER + "\\" + Globals.SUBJECT_NAME + "\\" + perFileName;

        if (File.Exists(path))
        {
            Debug.Log(perFileName + " already exists.");
            return;
        }

        perFile = File.CreateText(path);
        perFile.WriteLine(performanceData.killedEnemies);
        perFile.WriteLine(performanceData.pickedMushrooms);
        perFile.WriteLine(performanceData.pickedBonus);
        perFile.WriteLine(performanceData.deadTimes);
        perFile.WriteLine(performanceData.brokenBlocks);
        perFile.WriteLine(performanceData.completedTime);

        perFile.Close();
    }

    public void SavePersistentData()
    {
        persistentData.deathMatrix = deathMatrix;
        persistentData.geometry = rhythmFactory.GetMainGeometry();
        persistentData.rhythm = rhythmFactory.GetMainRhythm();
        persistentData.time = time;
        persistentData.stringTime = timeText.text;
        persistentData.deaths = GetNumberOfDeaths();
        persistentData.avgAttention = GetAttentionValuesAvg();
    }

    public void SaveRhythmPersistentData()
    {
        rhythmPersistent.rhythm = rhythmFactory.GetMainRhythm();
        rhythmPersistent.geometry = rhythmFactory.GetMainGeometry();
        rhythmPersistent.attentionMatrix = attentionMatrix;
        rhythmPersistent.deathMatrix = deathMatrix;
        rhythmPersistent.globalPerformance = CalculateGlobalPerformance();
        rhythmPersistent.gameTime = time;
        rhythmPersistent.avgAttention = GetAttentionValuesAvg();
        rhythmPersistent.level = rhythmPersistent.level + 1;
    }

    public float CalculateGlobalPerformance()
    {
        const float W1 = 0.4f;
        const float W2 = 0.3f;
        const float W3 = 0.3f;
        float deathsRate, timeRate;

        deathsRate = 100 / (GetNumberOfDeaths() + 1);
        timeRate = (rhythmFactory.GetMainRhythm().GetTime() / time) * 100;

        Debug.Log("performance " + rhythmFactory.GetMainRhythm().GetTime() + " time " + time);
        return deathsRate * W1 + timeRate * W2 + GetAttentionValuesAvg() * W3;
    }
}
