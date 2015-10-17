using UnityEngine;
using System.Collections;

public class PlayerUpCollider : MonoBehaviour {

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    protected virtual void OnTriggerEnter2D(Collider2D col)
    {
        GCPlayerController player;

        switch (col.tag)
        {
            case Names.Breakable:
                player = gameObject.GetComponentInParent<GCPlayerController>();
                player.StopJump();
                GameObject.Destroy(col.gameObject);
                break;
            case Names.Question:
                break;
        }
    }
}
