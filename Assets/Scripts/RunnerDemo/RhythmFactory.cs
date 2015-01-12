using UnityEngine;
using System.Collections;

/// <summary>
/// 
/// </summary>
public class RhythmFactory : MonoBehaviour
{
    /// <summary>
    /// 
    /// </summary>
    public void Start()
    {
        Rhythm rhythm;

        rhythm = new Rhythm(Rhythm.Type.Regular, Rhythm.Density.High, 20);
        rhythm.Build(0.3f, 0.7f,0.5f,4);
        rhythm.Print();
    }

    /// <summary>
    /// 
    /// </summary>
    public void Update()
    {}

    /// <summary>
    /// 
    /// </summary>
    public void CreateRhythm()
    { }
}
