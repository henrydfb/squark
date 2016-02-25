using UnityEngine;
using System.Collections;

public class LevelCompleteController : MonoBehaviour 
{
    //Box width
    private const float boxWidth = 500;
    //Box height
    private const float boxHeight = 300;
    //Button width
    private const float buttonWidth = 100;
    //Button height
    private const float buttonHeight = 40;
    //private StoredDataCtrl storedData;
    private string onRetryName;

    public LevelCompleteController(string onRetryName)
    {
        this.onRetryName = onRetryName;
    }

    protected virtual void Start()
    {
        GameObject perObj, rhythmObj;

        //Persistent data
        perObj = GameObject.Find("PersistentObject");
        rhythmObj = GameObject.Find("PersistentRhythm");

        if (perObj != null)
            GameObject.Destroy(perObj);

        if (rhythmObj != null)
            DontDestroyOnLoad(rhythmObj);
    }

    protected virtual void Update()
    {
        HandleInput();
    }

    protected virtual void HandleInput()
    {
        if (Input.GetButtonUp(Names.JumpInput))
            Application.LoadLevel("Squark");
    }

    protected virtual void OnGUI()
    {
        //string scoreText, highscoreText;

        // Make a background box
        GUI.Box(new Rect((Screen.width - boxWidth) / 2, (Screen.height - boxHeight) / 2, boxWidth, boxHeight), "Level Complete!");

        //Retry
        if (GUI.Button(new Rect((Screen.width - buttonWidth) / 2, (Screen.height - buttonHeight) / 2, buttonWidth, buttonHeight), "Next"))
            Application.LoadLevel("Squark");
            //Application.LoadLevel("GameContentsAuto");
    }
}
