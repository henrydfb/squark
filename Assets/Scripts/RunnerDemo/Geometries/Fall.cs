using UnityEngine;
using System.Collections;

/// <summary>
/// 
/// </summary>
public class Fall : Geometry
{
    /// <summary>
    /// 
    /// </summary>
    public const string TYPE = "Fall";

    public Fall(float beginTime, int endPos)
        :base(TYPE,beginTime)
    {
    }
}