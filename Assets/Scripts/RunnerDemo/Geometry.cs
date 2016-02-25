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
    float gapActRate;
    float eneActRate;
    float spkActRate;

    public Geometry(GeometryInterpretation[] interpretations, float gapActRate, float eneActRate, float spkActRate)
    {
        this.interpretations = interpretations;
        this.gapActRate = gapActRate;
        this.eneActRate = eneActRate;
        this.spkActRate = spkActRate;
    }

    public float GetGapActRate()
    {
        return gapActRate;
    }

    public float GetEneActRate()
    {
        return eneActRate;
    }

    public float GetSpkActRate()
    {
        return spkActRate;
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