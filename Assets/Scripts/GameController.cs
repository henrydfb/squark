using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour 
{
    public const int NUMBER_OF_ATTENTION_POINT = 30;

    public bool IsGameNeurosky = true;

    protected bool isGameRunning = false;

    public float WorldSpeed;
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
    public const float MAX_TIME = 60 * 10;
    public const float MIN_TIME = 0;
    public const int NUMBER_OF_BLOCKS = 200;

    //Current play time
    protected float time;
    protected Text timeText;
    protected Text levelText;
    protected Text neuroskyStatsText;
    //protected MessageController feedbackMessage;
    Text feedbackMessage;
    protected GameObject player;
    protected bool isGameOver;
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
    protected string currLevelType;

    protected int[] attentionLevels;
    protected int currentAttLvl;

    protected float globalValue;

    protected bool sentDeathData;

    protected bool hasStarted;

    protected string currAct;

    public int GetMeditation()
    {
        return meditation1;
    }

	// Use this for initialization
    protected virtual void Start() 
    {
        GameObject nkystsObj;

        timeText = GameObject.Find(Names.TimeText).GetComponent<Text>();
        levelText = GameObject.Find(Names.LevelText).GetComponent<Text>();
        //feedbackMessage = GameObject.Find(Names.FeedbackMessage).GetComponent<MessageController>();
        feedbackMessage = GameObject.Find(Names.FeedbackMessage).GetComponent<Text>();
        
        nkystsObj = GameObject.Find(Names.NeuroskyStatsText);
        if(nkystsObj != null)
            neuroskyStatsText = nkystsObj.GetComponent<Text>();
        player = GameObject.Find(Names.Player);
        isGameOver = false;
        hasStarted = false;

        //Neurosky
        if (IsGameNeurosky)
        {
            controller = GameObject.Find("NeuroSkyTGCController").GetComponent<TGCConnectionController>();
            controller.UpdatePoorSignalEvent += OnUpdatePoorSignal;
            controller.UpdateAttentionEvent += OnUpdateAttention;
            controller.UpdateMeditationEvent += OnUpdateMeditation;
            //controller.UpdateBlinkEvent += OnUpdateBlink;

            feedbackMessage.text = "Connecting EEG Device";
        }
        else
        {
            //Repeating
            InvokeRepeating("UpdateAttention", TGCConnectionController.NEUROSKY_INITIAL_TIME, 1);
            //InvokeRepeating("UpdateMeditation", TGCConnectionController.NEUROSKY_INITIAL_TIME, 1);
        }

        attValues = new ArrayList();
        currAttSec = 0;
        currAct = "";

        currentAttLvl = 0;
        attentionLevels = new int[NUMBER_OF_ATTENTION_POINT];
        for (int i = 0; i < attentionLevels.Length; i++)
            attentionLevels[i] = -1; //We initialize this in -1 to identify when the value was initialized or not
        sentDeathData = false;
	}

    public virtual void ReachGoal()
    { 
    }

    public void SetCurrentAction(string act)
    {
        currAct = act;
    }

    public void UpdateAttention()
    {
        OnUpdateAttention(Random.Range(0,100));
    }

    public void UpdateMeditation()
    {
        OnUpdateMeditation(Random.Range(0, 100));
    }

    public TGCConnectionController GetTGCConnectionController()
    {
        return controller;
    }

    public bool IsGameRunning()
    {
        return isGameRunning;
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

        Debug.Log("att: " + value);

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
        Debug.Log("med " + value);

        //Camera.main.GetComponent<CameraController>().OnUpdateAttention(value);
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
            {
                if(hasStarted)
                    feedbackMessage.text = "";
                else
                    feedbackMessage.text = "Ready to play!";
            }
            else
                feedbackMessage.text = "Connecting EEG Device";
        }
        else
        {
            isGameRunning = true;
        }

        min = (int)(time / 60);
        sec = (int)(time % 60);

        //If the game started
        if (isGameRunning && hasStarted)
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
                    player.GetComponent<PlayerController>().Die();
                    sentDeathData = true;
                }

                time += Time.deltaTime;
            }
        }

        timeText.text = min.ToString("00") + ":" + sec.ToString("00");
	}

    public bool GameHasStarted()
    {
        return hasStarted;
    }

    public void MovePlayer()
    {
        hasStarted = true;
        feedbackMessage.text = "";
    }

    public void SetLevel(int level)
    {
        if(levelText == null)
            levelText = GameObject.Find(Names.LevelText).GetComponent<Text>();

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
}
