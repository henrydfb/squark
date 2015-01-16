using UnityEngine;
using System.Collections;

/// <summary>
/// 
/// </summary>
public class Jump : Action
{
    /// <summary>
    /// 
    /// </summary>
    public const string TYPE = "Jump";

    public enum HeightType
    {
        Short,
        Medium,
        Long
    }

    private HeightType heightType;

    public Jump(float beginTime, float endTime, HeightType heightType)
        :base(TYPE,beginTime,endTime)
    {
        this.heightType = heightType;
    }

    public override string GetPrintAction()
    {
        return "(" + type + ":" + beginTime.ToString("00.00") + "," + heightType + ")";
    }
}