using UnityEngine;
using System.Collections;

/// <summary>
/// 
/// </summary>
public abstract class Action
{
    /// <summary>
    /// 
    /// </summary>
    protected string type;

    /// <summary>
    /// 
    /// </summary>
    protected float beginTime;

    /// <summary>
    /// 
    /// </summary>
    protected float endTime;

    public Action(string type, float beginTime, float endTime)
    {
        this.type = type;
        this.beginTime = beginTime;
        this.endTime = endTime;
    }

    public Action(string type, float beginTime)
    {
        this.type = type;
        this.beginTime = beginTime;
    }

    public float GetBeginTime()
    {
        return beginTime;
    }

    public float GetEndTime()
    {
        return endTime;
    }

    public string GetType()
    {
        return type;
    }

    public void SetEndTime(float value)
    {
        endTime = value;
    }

    public virtual string GetPrintAction()
    {
        return "(" + type + ":" + beginTime.ToString("00.00") + "," + endTime.ToString("00.00") + ")";
    }

    public void Print()
    {
        Debug.Log(type + ":" + "(" + beginTime + "," + endTime + ")");
    }
}