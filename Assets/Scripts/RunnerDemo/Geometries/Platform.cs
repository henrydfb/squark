using UnityEngine;
using System.Collections;

/// <summary>
/// 
/// </summary>
public class Platform : GeometryInterpretation
{
    /// <summary>
    /// 
    /// </summary>
    public const string TYPE = "Platform";

    /// <summary>
    /// 
    /// </summary>
    public const int TYPE_NUMBER = 6;

    public Platform(Action action)
        :base(TYPE,TYPE_NUMBER,action)
    {
    }
}