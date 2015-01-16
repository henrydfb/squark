using UnityEngine;
using System.Collections;
using System.IO;

/// <summary>
/// 
/// </summary>
public class RhythmFactory : MonoBehaviour
{
    public Rhythm.Type rhythmType;

    public Rhythm.Density rhythmDensity;

    public float rhythmLength;

    public float minJump;

    public float maxJump;


    /// <summary>
    /// 
    /// </summary>
    public void Start()
    {
        Rhythm rhythm;

        for (int i = 0; i < 1; i++)
        {
            rhythm = new Rhythm(rhythmType, rhythmDensity, rhythmLength);
            rhythm.Build(minJump, maxJump);
            //print(rhythm.GetPrint());
            InterpretRhythm(rhythm);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void Update()
    {}

    /// <summary>
    /// 
    /// </summary>
    public void InterpretRhythm(Rhythm rhythm)
    {
        Action[] actions;
        Geometry[] geometries;
        Geometry geometry;
        int nGeometries = 6;

        actions = rhythm.GetActions();
        geometries = new Geometry[actions.Length];

        // searches the current directory and sub directory
        //int fCount = Directory.GetFiles(Path.GetDirectoryName("Geometries"), "*", SearchOption.AllDirectories).Length;
        // searches the current directory
        //int fCount2 = Directory.GetFiles(Path.GetDirectoryName("Geometries"), "*", SearchOption.TopDirectoryOnly).Length;

        //Debug.Log("WII: " + fCount);
        //Debug.Log("WII2: " + fCount2);
        foreach (Action a in actions)
        { 

        }
    }
}
