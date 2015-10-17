using UnityEngine;
using System.Collections;
using System.IO;

public class GameController : MonoBehaviour {

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
    //Max time in seconds
    protected const float MAX_TIME = 60 * 10;
    protected const float MIN_TIME = 0;
    protected const int NUMBER_OF_BLOCKS = 200;
    //Current play time
    protected float time;
    protected GUIText timeText;
    protected GUIText coinsText;
    protected GUIText neuroskyStatsText;
    protected MessageController feedbackMessage;
    protected GameObject player;
    protected bool isGameOver;
    protected int numberOfCoins;
    protected int pickedCoins;

    protected ArrayList attValues;

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

    void Awake()
    {
        GameObject perObj;

        perObj = GameObject.FindGameObjectWithTag("Performance");
        if (perObj != null)
        {
            performanceData = perObj.GetComponent<PerformanceData>();
            if (performanceData != null)
                DontDestroyOnLoad(performanceData);
        }
    }

	// Use this for initialization
    protected virtual void Start() 
    {
        GameObject nkystsObj;
        timeText = GameObject.Find(Names.TimeText).GetComponent<GUIText>();
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
            Debug.Log("neurosky si senor");
            //controller.UpdateMeditationEvent += OnUpdateMeditation;
            //controller.UpdateBlinkEvent += OnUpdateBlink;
            //controller.UpdateDeltaEvent += OnUpdateDelta;
        }
        else
        {
            //Repeating
            InvokeRepeating("Test", TGCConnectionController.NEUROSKY_INITIAL_TIME, 1);
        }

        if (IsGameNeurosky)
        {
            if(controller.ConnectionWasSuccessful())
                feedbackMessage.Show(Names.ConnectingMessage);
        }

        attValues = new ArrayList();
        currAttSec = 0;
        currAct = "";
	}

    public void Test()
    {
        OnUpdateAttention(Random.Range(0,100));
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

        currAct = "";

        Camera.main.GetComponent<CameraController>().OnUpdateAttention(value);
        //Debug.Log(currAttSec + " " +  value);
    }

    protected void OnUpdateMeditation(int value)
    {
        meditation1 = value;
    }

    protected virtual void OnUpdateBlink(int value)
    {
        blink = value;
        //Make the player jump
        //player.GetComponent<PlayerController>().Jump();
    }

    protected void OnUpdateDelta(float value)
    {
        delta = value;
    }
	
	// Update is called once per frame
    protected virtual void Update() 
    {
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

                if (isGameOver)
                    player.GetComponent<PlayerController>().Die();
            }
        }

        timeText.text = min.ToString("00") + ":" + sec.ToString("00");
        UpdateCoinsValues();
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
        controller.UpdatePoorSignalEvent -= OnUpdatePoorSignal;
        controller.UpdateAttentionEvent -= OnUpdateAttention;
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

    public void SavePerformance(string saveType)
    {
        StreamWriter perFile, eegFile;

        string path;
        string perFileName = "per-" + currLevelType + "-" + performanceData.deadTimes + "-" + saveType + ".txt";
        string eegFileName = "eeg-" + currLevelType + "-" + performanceData.deadTimes + "-" + saveType + ".csv";

        //Performance File
        path = Globals.ROOT_FOLDER + "\\" + Globals.SUBJECT_NAME + "\\" + perFileName;

        if (File.Exists(path))
        {
            Debug.Log(perFileName + " already exists.");
            return;
        }

        perFile = File.CreateText(path);
        perFile.WriteLine("*****PERFORMANCE******");
        perFile.WriteLine("Subject: " + Globals.SUBJECT_NAME);
        perFile.WriteLine("");
        perFile.WriteLine("Enemies: " + performanceData.killedEnemies);
        perFile.WriteLine("Mushrooms: " + performanceData.pickedMushrooms);
        perFile.WriteLine("Bonus: " + performanceData.pickedBonus);
        perFile.WriteLine("Dead: " + performanceData.deadTimes);
        perFile.WriteLine("Broken: " + performanceData.brokenBlocks);
        perFile.WriteLine("Time: " + performanceData.completedTime);

        perFile.Close();

        //EEG File
        path = Globals.ROOT_FOLDER + "\\" + Globals.SUBJECT_NAME + "\\" + eegFileName;

        if (File.Exists(path))
        {
            Debug.Log(eegFileName + " already exists.");
            return;
        }

        eegFile = File.CreateText(path);
        eegFile.WriteLine("sec,eeg,action");
        foreach(ArrayList a in attValues)
        {
            eegFile.WriteLine("{0},{1},{2}", a[0], a[1], a[2]);
        }
        
        eegFile.Close();

        //Save aux
        if (saveType == "win" && currLevelType == "mario")
            SaveWinAux();
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
}
