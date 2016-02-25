using UnityEngine;
using System.Collections;

/// <summary>
/// 
/// </summary>
public abstract class GeometryInterpretation
{
    /// <summary>
    /// 
    /// </summary>
    protected string type;

    /// <summary>
    /// 
    /// </summary>
    protected int typeNumber;

    /// <summary>
    /// 
    /// </summary>
    protected Action action;


    public GeometryInterpretation(string type, int typeNumber,Action action)
    {
        this.type = type;
        this.typeNumber = typeNumber;
        this.action = action;
    }

    public string GetType()
    {
        return type;
    }

    public virtual string GetPrintAction()
    {
        return type;
    }

    public void Print()
    {
        Debug.Log(type);
    }

    public Action GetAction()
    {
        return action;
    }
}