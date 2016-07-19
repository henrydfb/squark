using UnityEngine;
using System.Collections;

public class CameraControllerGG : CameraController 
{
    protected override void Start()
    {
        player = GameObject.FindGameObjectWithTag(Names.Player).GetComponent<PlayerControllerGG>();
        gameController = GameObject.Find(Names.GameController).GetComponent<GameControllerGG>();

        base.Start();
    }
}
