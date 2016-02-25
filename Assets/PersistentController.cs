using UnityEngine;
using System.Collections;

public class PersistentController : MonoBehaviour 
{
    public float time;
    public string stringTime;
    public Geometry geometry;
    public Rhythm rhythm = null;
    public int[] deathMatrix;
    public int deaths;
    public float avgAttention;

    public float CalculateGlobalPerformance(float maxTime)
    {
        const float W1 = 0.4f;
        const float W2 = 0.3f;
        const float W3 = 0.3f;
        float deathsRate, timeRate;

        deathsRate = 100 / (deaths + 1);
        timeRate = (maxTime / time) * 100;

        return deathsRate * W1 + timeRate * W2 + avgAttention * W3;
    }
}
