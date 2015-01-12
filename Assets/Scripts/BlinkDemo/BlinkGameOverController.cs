using UnityEngine;
using System.Collections;

public class BlinkGameOverController : GameOverController 
{
    public BlinkGameOverController()
        :base(Names.BlinkDemoScene)
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

    protected override void Retry()
    {
        base.Retry();

        Application.LoadLevel(Names.BlinkDemoScene);
    }
}
