using UnityEngine;
using System.Collections;

/// <summary>
/// 
/// </summary>
public class NotFlatGap : GeometryInterpretation
{
    /// <summary>
    /// 
    /// </summary>
    public const string TYPE = "NotFlatGap";

    /// <summary>
    /// 
    /// </summary>
    public const int TYPE_NUMBER = 5;

    public NotFlatGap()
        :base(TYPE,TYPE_NUMBER)
    {
    }
}