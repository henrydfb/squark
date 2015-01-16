using UnityEngine;
using System.Collections;

/// <summary>
/// 
/// </summary>
public class FlatGap : Geometry
{
    /// <summary>
    /// 
    /// </summary>
    public const string TYPE = "FlatGap";

    public FlatGap(float beginTime, float endTime)
        :base(TYPE,beginTime,endTime)
    {
    }
}