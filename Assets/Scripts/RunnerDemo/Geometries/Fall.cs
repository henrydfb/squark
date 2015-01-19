using UnityEngine;
using System.Collections;

/// <summary>
/// 
/// </summary>
public class Fall : GeometryInterpretation
{
    /// <summary>
    /// 
    /// </summary>
    public const string TYPE = "Fall";

    /// <summary>
    /// 
    /// </summary>
    public const int TYPE_NUMBER = 1;

    public Fall()
        :base(TYPE,TYPE_NUMBER)
    {
    }
}