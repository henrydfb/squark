using UnityEngine;
using System.Collections;

/// <summary>
/// 
/// </summary>
public class Geometry
{
    /// <summary>
    /// 
    /// </summary>
    GeometryInterpretation[] interpretations;

    public Geometry(GeometryInterpretation[] interpretations)
    {
        this.interpretations = interpretations;   
    }

    public GeometryInterpretation[] GetInterpretations()
    {
        return interpretations;
    }

    public string GetPrint()
    {
        string print;

        print = "(";
        foreach (GeometryInterpretation g in interpretations)
        {
            print += g.GetType() + ",";
        }
        print += ")";

        return print;
    }
}