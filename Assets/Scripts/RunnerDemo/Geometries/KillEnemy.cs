using UnityEngine;
using System.Collections;

/// <summary>
/// 
/// </summary>
public class KillEnemy : Geometry
{
    /// <summary>
    /// 
    /// </summary>
    public const string TYPE = "KillEnemy";

    public KillEnemy(float beginTime, int endPos)
        :base(TYPE,beginTime)
    {
    }
}