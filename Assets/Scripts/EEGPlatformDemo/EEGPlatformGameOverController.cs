using UnityEngine;
using System.Collections;

public class EEGPlatformGameOverController : MonoBehaviour 
{
    void Start()
    {
        PersistentController persistentData = GameObject.Find("PersistentObject").GetComponent<PersistentController>();

        GameObject.Find("TimeText").GetComponent<GUIText>().text = persistentData.time;
    }


    void Update()
    {
        if (Input.GetButtonUp(Names.StartInput))
        {
            Application.LoadLevel("Squark");
        }
    }
}
