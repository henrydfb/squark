using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlatformController : MonoBehaviour 
{
    private bool isLast = false;

    public GameObject sprite;


    protected GameController gameController;
    protected List<GameObject> pieces;

	// Use this for initialization
	protected virtual void Start () 
    {
        gameController = GameObject.Find(Names.GameController).GetComponent<GameController>();
	}

    public bool GetLast()
    {
        return isLast;
    }

    public void SetLast(bool isLast)
    {
        this.isLast = isLast;
    }

	// Update is called once per frame
	protected virtual void Update () 
    {
        if (gameController.GameStarted())
        {
            //transform.position += new Vector3(-Mathf.Abs(gameController.WorldSpeed), 0);
            //Destroy
            /*if (transform.position.x + GetComponent<Renderer>().bounds.size.x / 2 <= Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.WorldToScreenPoint(Camera.main.transform.position).x - Screen.width / 2, 0)).x)
            {
                foreach(GameObject p in pieces)
                    Destroy(p);
                Destroy(gameObject);
            }*/
        }
	}

    public bool CompletelyEnteredCamera()
    {
        return transform.position.x + GetComponent<Renderer>().bounds.size.x / 2 <= Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.WorldToScreenPoint(Camera.main.transform.position).x + Screen.width / 2, 0)).x;
    }

    public void Contruct(int numberOfBlock)
    {
        GameObject newSpt;

        transform.localScale = new Vector3(numberOfBlock,1);
        pieces = new List<GameObject>();
        //Sprites
        for (int i = 0; i < numberOfBlock; i++)
        {
            newSpt = (GameObject)Instantiate(sprite, transform.position + new Vector3(i * 0.3f, 0,-i*0.0001f), Quaternion.identity);
            //if(i == 0)
              //  newSpt.transform.position =  newSpt.transform.position - new Vector3(GetComponent<BoxCollider2D>().bounds.size.x / 2,1);
            //else
                newSpt.transform.position = newSpt.transform.position - new Vector3(GetComponent<BoxCollider2D>().bounds.size.x / 2, 0);

            pieces.Add(newSpt);
            //newSpt.transform.SetParent(this.transform);
        }

        
    }
}
