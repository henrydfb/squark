using UnityEngine;
using System.Collections;

public class RunnerGameController : GameController {

    public float WorldSpeed;
    public GameObject platformPrefab;
    public GameObject enemyPrefab;
    public GameObject coinPrefab;
    public GameObject playerShadowPrefab;

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
        time = MIN_TIME;

        base.Start();

        gameStarted = false;
        startTimer = 0;

        if(GameObject.Find(Names.FirstPlatform) != null)
            lastPlatform = GameObject.Find(Names.FirstPlatform).GetComponent<PlatformController>();
        cameraController = Camera.main.GetComponent<CameraController>();
        previousAttentionAverage = 0;
        currentSec = 0;
        rhythmFactory = GameObject.Find(Names.RhythmFactory).GetComponent<RhythmFactory>();
    }

    bool jidoStarted = false;

    protected override void Update()
    {
        base.Update();

        if (isGameRunning)
        {
            downLimit = rhythmFactory.GetLowestPosY();

            //Check if it's game over
            if (!isGameOver)
            {
                time += Time.deltaTime;
            }
        }
    }

    protected override void GameOver()
    {
        base.GameOver();

        SavePersistentData();
        Application.LoadLevel(Names.RunnerGameOverScene);
    }
}
