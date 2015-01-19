using UnityEngine;
using System.Collections;

/// <summary>
/// 
/// </summary>
public class NoGap : GeometryInterpretation
{
    /// <summary>
    /// 
    /// </summary>
    public const string TYPE = "NoGap";

    /// <summary>
    /// 
    /// </summary>
    public const int TYPE_NUMBER = 4;

    public NoGap()
        :base(TYPE,TYPE_NUMBER)
    {
    }
}