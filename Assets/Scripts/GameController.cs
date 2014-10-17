using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

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
    private const float MAX_TIME = 60;
    private const float NUMBER_OF_BLOCKS = 200;
    //Current play time
    private float time;
    private GUIText timeText;
    private GameObject plat;
    private GameObject player;
    private bool isGameOver;

	// Use this for initialization
	void Start () {
        time = MAX_TIME;
        timeText = GameObject.Find(Names.TimeText).GetComponent<GUIText>();
        plat = GameObject.Find("Block");
        player = GameObject.Find(Names.Player);
        isGameOver = false;

        //plat.transform.localScale = new Vector3((NUMBER_OF_BLOCKS * player.renderer.bounds.size.x)/plat.renderer.bounds.size.x, 1, 0);
        //plat.transform.position += new Vector3(plat.renderer.bounds.size.x / 2, 0, 0);

	}
	
	// Update is called once per frame
	void Update () 
    {
        int min, sec;

        min = (int)(time / 60);
        sec = (int)(time % 60);

        //Left Limit
        Debug.DrawLine(new Vector3(leftLimit, downLimit), new Vector3(leftLimit, upLimit), Color.red);
        //Right Limit
        Debug.DrawLine(new Vector3(rightLimit, downLimit), new Vector3(rightLimit, upLimit), Color.red);
        //Up Limit
        Debug.DrawLine(new Vector3(leftLimit, upLimit), new Vector3(rightLimit, upLimit), Color.red);
        //Down Limit
        Debug.DrawLine(new Vector3(leftLimit, downLimit), new Vector3(rightLimit, downLimit), Color.red);

        if (!isGameOver)
        {
            //Debug.Log(Camera.main.ScreenToWorldPoint(new Vector3(0,Screen.height / 2,0)));
            //Out of the screen
            isGameOver = player.transform.position.y + player.renderer.bounds.size.y / 2 <= downLimit;
            if (isGameOver)
                player.GetComponent<PlayerController>().Die();

            if (time > 0)
                time -= Time.deltaTime;
            else
            {
                Debug.Log("Time is over!");
                isGameOver = true;
            }
        }

        timeText.text = min + ":" + sec;

        //plat.transform.position = new Vector3(player.transform.position.x, plat.transform.position.y, plat.transform.position.z);
	}

    public bool IsGameOver()
    {
        return isGameOver;
    }
}
