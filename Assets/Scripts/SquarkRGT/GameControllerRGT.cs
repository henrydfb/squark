using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameControllerRGT : GameController
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

    public RhythmPersistentController rhythmPersistentData;
    public PersistentController persistentData;
    
    protected RhythmFactory rhythmFactory;
    protected RhythmPersistentController rhythmPersistent;
    //Attention values
    protected List<float>[] attentionMatrix;
    //Death values
    protected int[] deathMatrix;
    //private int[] attentionLevels;
    //private int currentAttLvl;
    //protected float globalValue;
    //protected bool sentDeathData;
    private float startTimer;
    private PlatformController lastPlatform;
    private CameraController cameraController;
    private float previousAttentionAverage;
    private int currentSec;
    private float iniX;
    private int prevTime;
    
    void Awake()
    {
        GameObject rhythmObj;

        rhythmObj = GameObject.Find("PersistentRhythm");
        if (rhythmObj != null)
        {
            rhythmPersistent = rhythmObj.GetComponent<RhythmPersistentController>();
            DontDestroyOnLoad(rhythmObj);
        }
    }

	// Use this for initialization
    protected override void  Start()
    {
        base.Start();

        GameObject perObj;

        //Persistent data
        perObj = GameObject.Find("PersistentObject");
        persistentData = perObj.GetComponent<PersistentController>();
        Object.DontDestroyOnLoad(perObj);
        
        player = GameObject.Find(Names.Player);
        isGameOver = false;

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


        time = MIN_TIME;

        gameStarted = true;

        startTimer = 0;

        if(GameObject.Find(Names.FirstPlatform) != null)
            lastPlatform = GameObject.Find(Names.FirstPlatform).GetComponent<PlatformController>();
        cameraController = Camera.main.GetComponent<CameraController>();
        previousAttentionAverage = 0;
        currentSec = 0;
        rhythmFactory = GameObject.Find(Names.RhythmFactory).GetComponent<RhythmFactory>();
	}

    public void Reset()
    {
        CleanScene();

        SceneManager.LoadScene("Squark");
    }

    public void Restart()
    {
        persistentData.rhythm = null;
        persistentData.geometry = null;
        rhythmPersistent.Restart();

        CleanScene();

        SceneManager.LoadScene("Squark");
    }

    public override void ReachGoal()
    {
        base.ReachGoal();

        SavePersistentData();
        SaveRhythmPersistentData();
        CleanScene();
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

	// Update is called once per frame
    /*protected virtual void Update() 
    {
        PlatformController diePlatformL, diePlatformR;

        //Check if the game started
        if (IsGameNeurosky)
        {
            isGameRunning = poorSignal1 == 0 && controller.ConnectionWasSuccessful();
            if (isGameRunning)
                feedbackMessage.Hide();
        }
        else
            isGameRunning = true;

        //If the game started
        if (isGameRunning)
        {
            downLimit = rhythmFactory.GetLowestPosY();

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

                time += Time.deltaTime;
            }
        }
	}*/

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
                        AddAttentionValue(AttentionType.Low);
                    else if (dataType == "death")
                        AddDeath(DeathType.Low);
                }
                //Med Jump platform
                else if (dist >= 1.4f && dist <= 1.6f)
                {
                    if (dataType == "jump")
                        AddAttentionValue(AttentionType.Med);
                    else if (dataType == "death")
                        AddDeath(DeathType.Med);
                }
                //High Jump platform
                else
                {
                    if (dataType == "jump")
                        AddAttentionValue(AttentionType.Hig);
                    else if (dataType == "death")
                        AddDeath(DeathType.Hig);
                }

                if (dataType == "jump")
                    AddAttentionValue(AttentionType.Gap);
                else if (dataType == "death")
                    AddDeath(DeathType.Gap);
            }
        }
    }

    public int GetNumberOfDeaths()
    {
        return deathMatrix[(int)DeathType.Ene] + deathMatrix[(int)DeathType.Gap] + deathMatrix[(int)DeathType.Spk];
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

        #if UNITY_STANDALONE

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
        #endif
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

        //Save data to show in the level complete graph
        rhythmPersistent.SaveValues(GetPerfomanceValue(), GetAttentionValuesAvg(), rhythmPersistent.rhythm.GetDifficulty(), CalculateGlobalPerformance());
    }

    public float GetPerfomanceValue()
    {
        float deathsRate, timeRate;

        deathsRate = 100 / (GetNumberOfDeaths() + 1);
        timeRate = (rhythmFactory.GetMainRhythm().GetTime() / time) * 100;

        return deathsRate * 0.5f + timeRate * 0.5f;
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

    protected override void GameOver()
    {
        base.GameOver();

        SavePersistentData();

        SceneManager.LoadScene(Names.RunnerGameOverScene);
    }
}
