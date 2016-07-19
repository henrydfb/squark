using System.Collections.Generic;
using UnityEngine;

public class RhythmPersistentController : MonoBehaviour 
{
    public Rhythm rhythm = null;
    public Geometry geometry = null;
    public List<float>[] attentionMatrix = null;
    public int[] deathMatrix = null;
    public float globalPerformance = 0;
    public float gameTime = 0;
    public float avgAttention = 0;
    public int level = 0;
    public List<float>[] values;

    public void Reset()
    {
        globalPerformance = 0;
        gameTime = 0;
        avgAttention = 0;
    }

    public void Restart()
    { 
        rhythm = null;
        geometry = null;
        attentionMatrix = null;
        deathMatrix = null;
        globalPerformance = 0;
        gameTime = 0;
        avgAttention = 0;
        level = 1;
        values = null;
    }

    public void SaveValues(float per, float att, float diff, float glob)
    {
        //Initializing
        if (values == null)
        {
            values = new List<float>[4];
            for (int i = 0; i < 4; i++)
            {
                values[i] = new List<float>();
                if(i == 2)
                    values[i].Add(0);
                else
                    values[i].Add(0.5f);
            }
        }

        for (int i = 0; i < 4; i++)
        {
            switch (i)
            { 
                case 0:
                    values[i].Add(per);
                    break;
                case 1:
                    values[i].Add(att);
                    break;
                case 2:
                    values[i].Add(diff);
                    break;
                case 3:
                    values[i].Add(glob);
                    break;
            }
        }
    }
}
