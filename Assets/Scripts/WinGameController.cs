using UnityEngine;
using System.Collections;

public class WinGameController : MonoBehaviour {

    //Box width
    private const float boxWidth = 500;
    //Box height
    private const float boxHeight = 300;
    //Button width
    private const float buttonWidth = 100;
    //Button height
    private const float buttonHeight = 40;
    //private StoredDataCtrl storedData;

    protected virtual void Start()
    {

        //storedData = GameObject.FindWithTag("STOREDDATA").GetComponent<StoredDataCtrl>();
    }

    protected virtual void OnGUI()
    {
        //string scoreText, highscoreText;

        // Make a background box
        GUI.Box(new Rect((Screen.width - boxWidth) / 2, (Screen.height - boxHeight) / 2, boxWidth, boxHeight), "Level Cleared!");

        //Retry
        if (GUI.Button(new Rect((Screen.width - buttonWidth) / 2, (Screen.height - buttonHeight) / 2, buttonWidth, buttonHeight), "Retry?"))
            Application.LoadLevel(Names.BlinkDemoScene);

        //Quit
        if (GUI.Button(new Rect((Screen.width - buttonWidth) / 2, ((Screen.height - buttonHeight) / 2) + buttonHeight + buttonHeight / 2, buttonWidth, buttonHeight), "Quit"))
            Application.Quit();

        //High score
        //GUI.TextField(new Rect((Screen.width - buttonWidth) / 2, ((Screen.height - buttonHeight) / 2) + buttonHeight - 3 * buttonHeight, buttonWidth, buttonHeight), highscoreText);

        //Score
        //GUI.TextField(new Rect((Screen.width - buttonWidth) / 2, ((Screen.height - buttonHeight) / 2) + buttonHeight - 2 * buttonHeight, buttonWidth, buttonHeight), scoreText);
    }
}
