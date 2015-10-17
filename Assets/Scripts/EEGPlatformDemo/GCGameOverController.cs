using UnityEngine;
using System.Collections;

public class GCGameOverController : GameOverController 
{
    public GCGameOverController()
        :base(Names.GCDemoScene)
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
