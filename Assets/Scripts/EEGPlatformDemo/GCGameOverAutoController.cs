using UnityEngine;
using System.Collections;

public class GCGameOverAutoController : GameOverController 
{
    public GCGameOverAutoController()
        :base(Names.AutoGCDemoScene)
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
