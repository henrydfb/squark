using UnityEngine;
using System.Collections;

/// <summary>
/// 
/// </summary>
public class AvoidEnemy : GeometryInterpretation
{
    /// <summary>
    /// 
    /// </summary>
    public const string TYPE = "AvoidEnemy";

    /// <summary>
    /// 
    /// </summary>
    public const int TYPE_NUMBER = 0;

    public AvoidEnemy()
        :base(TYPE,TYPE_NUMBER)
    {
    }
}