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


    public GeometryInterpretation(string type, int typeNumber)
    {
        this.type = type;
        this.typeNumber = typeNumber;
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
}