using UnityEngine;
using System.Collections;

/// <summary>
/// 
/// </summary>
public class KillEnemy : GeometryInterpretation
{
    /// <summary>
    /// 
    /// </summary>
    public const string TYPE = "KillEnemy";

    /// <summary>
    /// 
    /// </summary>
    public const int TYPE_NUMBER = 3;

    public KillEnemy(Action action)
        :base(TYPE,TYPE_NUMBER,action)
    {
    }
}