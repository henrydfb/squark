using UnityEngine;
using System.Collections;

public class BlinkWinGameController : WinGameController 
{
    public BlinkWinGameController()
        : base(Names.BlinkDemoScene)
    { }

    protected override void Start()
    {
        base.Start();
    }
}
