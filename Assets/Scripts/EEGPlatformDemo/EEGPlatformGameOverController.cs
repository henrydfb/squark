using UnityEngine;
using System.Collections;

public class EEGPlatformGameOverController : MonoBehaviour 
{
    void Start()
    {
        GameObject perObj;
        PersistentController persistentData;
        perObj = GameObject.Find("PersistentObject");
        persistentData = perObj.GetComponent<PersistentController>();

        DontDestroyOnLoad(perObj);

        GameObject.Find("TimeText").GetComponent<GUIText>().text = persistentData.stringTime;
    }


    void Update()
    {
        if (Input.GetButtonUp(Names.StartInput))
        {
            Application.LoadLevel("Squark");
        }
    }
}
