using UnityEngine;
using System.Collections;

/// <summary>
/// 
/// </summary>
public class FlatGap : GeometryInterpretation
{
    /// <summary>
    /// 
    /// </summary>
    public const string TYPE = "FlatGap";

    /// <summary>
    /// 
    /// </summary>
    public const int TYPE_NUMBER = 2;

    public FlatGap(Action action)
        :base(TYPE,TYPE_NUMBER,action)
    {
    }
}