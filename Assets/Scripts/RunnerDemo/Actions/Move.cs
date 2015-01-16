using UnityEngine;
using System.Collections;

/// <summary>
/// 
/// </summary>
public class Move : Action
{
    /// <summary>
    /// 
    /// </summary>
    public const string TYPE = "Move";

    /// <summary>
    /// The position of the end time on the Rythm array
    /// </summary>
    private int endPosition;

    public Move(float beginTime, int endPos)
        :base(TYPE,beginTime)
    {
        this.endPosition = endPos;
    }

    public int GetEndTimePosition()
    {
        return endPosition;
    }
}