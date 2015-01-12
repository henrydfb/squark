using UnityEngine;
using System.Collections;

public class EEGPlatformGameController : GameController 
{
    public GameObject platformPrefab;

    private PlatformController lastPlatform;

    protected override void Start()
    {
        time = MAX_TIME;

        base.Start();
    }

    protected override void Update()
    {
        base.Update();

        if (isGameRunning)
        {
            //Check if it's game over
            if (!isGameOver)
            {
                if (time > 0)
                    time -= Time.deltaTime;
                else
                    isGameOver = true;
            }
        }
    }

    protected override void GameOver()
    {
        base.GameOver();

        Application.LoadLevel(Names.EEGPlatformGameOverScene);
    }

    public void CreateGameElements()
    {
        PlatformController newPlatform;
        GameObject newPlatObj;

        for (int i = 0; i < 5; i++)
        {
            newPlatObj = (GameObject)Instantiate(platformPrefab, new Vector3(i,0.5f, 0), Quaternion.identity);
            newPlatform = newPlatObj.GetComponent<PlatformController>();
        }
    }

    public void CreateNewPlatforms()
    {
        PlatformController newPlatform;
        GameObject newPlatObj;
        float newY, newX, yDiff, xSep;
        int newWidth, newHeight, attentionLevel, meditationLevel;

        newX = lastPlatform.transform.position.x + lastPlatform.renderer.bounds.size.x / 2;
        if (Random.Range(0, 100) >= 50)
            attentionLevel = attention1;// Random.Range(0, 100);
        else
            attentionLevel = Random.Range(0, 100);
        //Low
        if (attentionLevel >= 0 && attentionLevel < 100 / 3)
        {
            if (lastPlatform.transform.position.y - 1 > downLimit)
                yDiff = -1;
            else
            {
                if (attentionLevel > (100 / 3) / 2)
                    yDiff = 0;
                else
                    yDiff = 1;
            }
        }

        //Medium
        else if (attentionLevel >= 100 / 3 && attentionLevel < 2 * (100 / 3))
            yDiff = 0;
        //High
        else
        {
            if (lastPlatform.transform.position.y + 1 < upLimit - 1)
                yDiff = 1;
            else
            {
                if (attentionLevel > ((2 * (100 / 3)) - 100 / 2) / 2)
                    yDiff = 0;
                else
                    yDiff = -1;
            }
        }

        //Width
        if (Random.Range(0, 100) >= 50)
            newWidth = 3 + attention1 * 20 / 100;
        else
            newWidth = Random.Range(3, 20);

        //Width
        if (Random.Range(0, 100) >= 50)
            xSep = 0.5f + meditation1 * 2 / 100;
        else
            xSep = Random.Range(0.5f, 2f);


        newY = lastPlatform.transform.position.y + yDiff;

        newPlatObj = (GameObject)Instantiate(platformPrefab, new Vector3(newX, newY, 0), Quaternion.identity);
        newPlatform = newPlatObj.GetComponent<PlatformController>();
        newPlatform.Contruct(newWidth);
        newPlatObj.transform.position += new Vector3((newPlatObj.renderer.bounds.size.x / 2) + xSep, 0, 0);
        lastPlatform = newPlatform;
    }
}
