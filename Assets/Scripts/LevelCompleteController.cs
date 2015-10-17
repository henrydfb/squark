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
        //storedData = GameObject.FindWithTag("STOREDDATA").GetComponent<StoredDataCtrl>();
    }

    protected virtual void OnGUI()
    {
        //string scoreText, highscoreText;

        // Make a background box
        GUI.Box(new Rect((Screen.width - boxWidth) / 2, (Screen.height - boxHeight) / 2, boxWidth, boxHeight), "Level Complete!");

        //Retry
        if (GUI.Button(new Rect((Screen.width - buttonWidth) / 2, (Screen.height - buttonHeight) / 2, buttonWidth, buttonHeight), "Next"))
            Application.LoadLevel("GameContentsAuto");
    }
}
