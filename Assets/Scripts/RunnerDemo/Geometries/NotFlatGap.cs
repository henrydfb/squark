using UnityEngine;
using System.Collections;

/// <summary>
/// 
/// </summary>
public class NotFlatGap : Geometry
{
    /// <summary>
    /// 
    /// </summary>
    public const string TYPE = "NotFlatGap";

    public NotFlatGap(float beginTime, int endPos)
        :base(TYPE,beginTime)
    {
    }
}