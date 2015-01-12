using UnityEngine;
using System.Collections;

public class EEGPlatformGameOverController : GameOverController 
{
    public EEGPlatformGameOverController()
        :base(Names.EEGPlatformDemoScene)
    { 
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void OnGUI()
    {
        base.OnGUI();
    }
}
