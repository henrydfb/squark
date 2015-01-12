using UnityEngine;
using System.Collections;

/// <summary>
/// 
/// </summary>
public class Action
{
    public enum Type
    { 
        Move,    
        Jump
    }

    /// <summary>
    /// 
    /// </summary>
    private Type type;

    /// <summary>
    /// 
    /// </summary>
    private float beginTime;

    /// <summary>
    /// 
    /// </summary>
    private float endTime;

    public Action(Type type, float beginTime, float endTime)
    {
        this.type = type;
        this.beginTime = beginTime;
        this.endTime = endTime;
    }

    public Action(Type type, float beginTime)
    {
        this.type = type;
        this.beginTime = beginTime;
    }

    public float GetBeginTime()
    {
        return beginTime;
    }

    public Type GetType()
    {
        return type;
    }

    public void SetEndTime(float value)
    {
        endTime = value;
    }

    public void Print()
    {
        Debug.Log(type + ":" + "(" + beginTime + "," + endTime + ")");
    }
}