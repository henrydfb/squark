using UnityEngine;
using System.Collections;

public class CoinController : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.collider.tag == Names.Player)
        {
            collider2D.enabled = false;
            Debug.Log("Choco!");
            Destroy(gameObject);
            
        }
    }
}
