using UnityEngine;

public class GameControllerGG : GameController 
{
    public GameObject platformPrefab;

    protected override void Start()
    {
        base.Start();

        /*GameObject platform;
        PlatformController platformCont;

        platform = (GameObject)Instantiate(platformPrefab, new Vector3(0,10), Quaternion.identity);
        platformCont = platform.GetComponent<PlatformController>();
        platformCont.Contruct(20);*/
    }
}
