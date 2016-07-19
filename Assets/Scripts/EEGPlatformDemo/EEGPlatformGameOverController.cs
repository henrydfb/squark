using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EEGPlatformGameOverController : MonoBehaviour 
{
    PersistentController persistentData;
    RhythmPersistentController rhythmPersistent;
    void Start()
    {
        GameObject perObj, rhythmObj;
        
        perObj = GameObject.Find("PersistentObject");
        rhythmObj = GameObject.Find("PersistentRhythm");
        persistentData = perObj.GetComponent<PersistentController>();
        rhythmPersistent = rhythmObj.GetComponent<RhythmPersistentController>();
        if(perObj != null)
            DontDestroyOnLoad(perObj);

        if (rhythmObj != null)
            DontDestroyOnLoad(rhythmObj);

        GameObject.Find("TimeText").GetComponent<Text>().text = persistentData.stringTime;
    }


    void Update()
    {
        if (Input.GetButtonUp(Names.JumpInput))
            SceneManager.LoadScene("Squark");
    }

    public void Restart()
    {
        persistentData.rhythm = null;
        persistentData.geometry = null;
        if (rhythmPersistent.level == 1)
        {
            rhythmPersistent.Restart();
        }

        SceneManager.LoadScene("Squark");
    }
}
