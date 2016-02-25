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
}
