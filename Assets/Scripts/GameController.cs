using UnityEngine;
using System.Collections;

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
    protected const float NUMBER_OF_BLOCKS = 200;
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

    //Neurosky values
    protected TGCConnectionController controller;
    protected int poorSignal1 = -1;
    protected int attention1;
    protected int meditation1;
    protected int blink;
    protected int indexSignalIcons = 1;
    protected float delta;

	// Use this for initialization
    protected virtual void Start() 
    {
        timeText = GameObject.Find(Names.TimeText).GetComponent<GUIText>();
        coinsText = GameObject.Find(Names.CoinsText).GetComponent<GUIText>();
        feedbackMessage = GameObject.Find(Names.FeedbackMessage).GetComponent<MessageController>();
        neuroskyStatsText = GameObject.Find(Names.NeuroskyStatsText).GetComponent<GUIText>();
        player = GameObject.Find(Names.Player);
        isGameOver = false;
        numberOfCoins = GameObject.FindGameObjectsWithTag(Names.Coin).Length;
        pickedCoins = 0;

        UpdateCoinsValues();

        //Neurosky
        controller = GameObject.Find("NeuroSkyTGCController").GetComponent<TGCConnectionController>();
        controller.UpdatePoorSignalEvent += OnUpdatePoorSignal;
        controller.UpdateAttentionEvent += OnUpdateAttention;
        controller.UpdateMeditationEvent += OnUpdateMeditation;
        controller.UpdateBlinkEvent += OnUpdateBlink;
        controller.UpdateDeltaEvent += OnUpdateDelta;

        if (IsGameNeurosky && controller.ConnectionWasSuccessful())
        {
            feedbackMessage.Show(Names.ConnectingMessage);
        }
	}

    public TGCConnectionController GetTGCConnectionController()
    {
        return controller;
    }

    public bool IsGameRunning()
    {
        return isGameRunning;
    }

    private void UpdateCoinsValues()
    {
        coinsText.text = pickedCoins.ToString("00") + "/" + numberOfCoins.ToString();
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
        attention1 = value;
    }

    protected void OnUpdateMeditation(int value)
    {
        meditation1 = value;
    }

    protected virtual void OnUpdateBlink(int value)
    {
        blink = value;
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
                isGameOver = (player.transform.position.y + player.renderer.bounds.size.y / 2 <= downLimit) || player.GetComponent<PlayerController>().IsDead();

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

    protected void CleanScene()
    {
        controller.UpdatePoorSignalEvent -= OnUpdatePoorSignal;
        controller.UpdateAttentionEvent -= OnUpdateAttention;
        controller.UpdateMeditationEvent -= OnUpdateMeditation;
        controller.UpdateBlinkEvent -= OnUpdateBlink;
        controller.UpdateDeltaEvent -= OnUpdateDelta;
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
        neuroskyStatsText.text = "STATS:\n\n\n\n\n\n\n\n\n\n\n\n";
        neuroskyStatsText.text += "Poor Signal: " + poorSignal1 + "\n";
        neuroskyStatsText.text += "Attention: " + attention1 + "\n";
        /*neuroskyStatsText.text += "Meditation: " + meditation1 + "\n";
        neuroskyStatsText.text += "Blink: " + blink + "\n";
        neuroskyStatsText.text += "Delta: " + delta + "\n";*/
    }

    public void OnDestroy()
    {
        //if(IsGameNeurosky)
          //  controller.Disconnect();
    }

}
