using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelCompleteCamera : MonoBehaviour 
{
    public Material mat;
    public Vector3 axisPos;
    public float size;
    private List<float>[] values;
    private float[] currValues;

	// Use this for initialization
	void Start () {
        GameObject per, diff, glob, att, prevPer, prevDiff, prevGlob, prevAtt;

        if (GameObject.Find("PersistentRhythm") != null)
        {
            values = GameObject.Find("PersistentRhythm").GetComponent<RhythmPersistentController>().values;

            prevPer = GameObject.Find("per-bar-prev");
            prevAtt = GameObject.Find("att-bar-prev");
            prevDiff = GameObject.Find("diff-bar-prev");
            prevGlob = GameObject.Find("glob-bar-prev");

            per = GameObject.Find("per-bar");
            att = GameObject.Find("att-bar");
            diff = GameObject.Find("diff-bar");
            glob = GameObject.Find("glob-bar");

            for (int i = 0; i < values.Length; i++)
            {
                for (int j = 0; j < values[i].Count; j++)
                    values[i][j] = values[i][j] > 1 ? values[i][j] / 100 : values[i][j];
            }


            prevPer.transform.localScale = new Vector3(prevPer.transform.localScale.x, values[0][values[0].Count - 2], prevPer.transform.localScale.z);
            prevAtt.transform.localScale = new Vector3(prevAtt.transform.localScale.x, values[1][values[1].Count - 2], prevAtt.transform.localScale.z);
            prevDiff.transform.localScale = new Vector3(prevDiff.transform.localScale.x, values[2][values[2].Count - 2], prevDiff.transform.localScale.z);
            prevGlob.transform.localScale = new Vector3(prevGlob.transform.localScale.x, values[3][values[3].Count - 2], prevGlob.transform.localScale.z);

            per.transform.localScale = new Vector3(per.transform.localScale.x, values[0][values[0].Count - 1], per.transform.localScale.z);
            att.transform.localScale = new Vector3(att.transform.localScale.x, values[1][values[1].Count - 1], att.transform.localScale.z);
            diff.transform.localScale = new Vector3(diff.transform.localScale.x, values[2][values[2].Count - 1], diff.transform.localScale.z);
            glob.transform.localScale = new Vector3(glob.transform.localScale.x, values[3][values[3].Count - 1], glob.transform.localScale.z);
        }
	}

   /* void OnPostRender()
    {
        if (!mat)
        {
            Debug.LogError("Please Assign a material on the inspector");
            return;
        }

        if (values[0].Count > 1 && values[1].Count > 1 && values[2].Count > 1 && values[3].Count > 1)
        {
            GL.PushMatrix();
            mat.SetPass(0);
            GL.LoadOrtho();
            GL.Begin(GL.LINES);

            GL.Color(new Color(167f / 255f, 153f / 255f, 82f / 255f));

            //Hor axis
            GL.Vertex(axisPos + new Vector3(0, 0, 0) * size);
            GL.Vertex(axisPos + new Vector3(1, 0, 0) * size);

            //Ver axis
            GL.Vertex(axisPos + new Vector3(0, 0, 0) * size);
            GL.Vertex(axisPos + new Vector3(0, 1, 0) * size);

            int j = 0;
            foreach (List<float> v in values)
            {
                switch (j)
                {
                    case 0:
                        GL.Color(new Color(143f / 255f, 157f / 255f, 219f / 255f));
                        break;
                    case 1:
                        GL.Color(new Color(139f / 255f, 207f / 255f, 156f / 255f));
                        break;
                    case 2:
                        GL.Color(new Color(207f / 255f, 139f / 255f, 139f / 255f));
                        break;
                    case 3:
                        GL.Color(new Color(224f / 255f, 212f / 255f, 99f / 255f));
                        break;
                }

                for (int i = 1; i < v.Count; i++)
                {
                    Debug.Log(v[i]);
                    GL.Vertex(axisPos + new Vector3((float)(i - 1) / (v.Count - 1), v[i - 1], 0) * size);
                    GL.Vertex(axisPos + new Vector3((float)i / (v.Count - 1), v[i], 0) * size);
                }

                j++;
            }

            GL.End();
            GL.PopMatrix();
        }
    }*/
}
