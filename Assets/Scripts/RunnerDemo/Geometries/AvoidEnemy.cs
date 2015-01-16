using UnityEngine;
using System.Collections;

/// <summary>
/// 
/// </summary>
public class AvoidEnemy : Geometry
{
    /// <summary>
    /// 
    /// </summary>
    public const string TYPE = "AvoidEnemy";

    public AvoidEnemy(float beginTime, int endPos)
        :base(TYPE,beginTime)
    {
    }
}