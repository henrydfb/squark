using UnityEngine;
using System.Collections;

/// <summary>
/// 
/// </summary>
public class NoGap : Geometry
{
    /// <summary>
    /// 
    /// </summary>
    public const string TYPE = "NoGap";

    public NoGap(float beginTime, int endPos)
        :base(TYPE,beginTime)
    {
    }
}