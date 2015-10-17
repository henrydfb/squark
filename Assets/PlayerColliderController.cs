using UnityEngine;
using System.Collections;

public class PlayerColliderController : MonoBehaviour {

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    protected virtual void OnTriggerEnter2D(Collider2D col)
    {
        PlayerController player;
        SimpleEnemyController enemy;

        switch (col.tag)
        { 
            case Names.Platform:
                player = gameObject.GetComponentInParent<PlayerController>();
                player.TouchPlatformDown();
                break;
            case Names.Question:
                player = gameObject.GetComponentInParent<PlayerController>();
                player.TouchPlatformDown();
                break;
            case Names.Pipe:
                player = gameObject.GetComponentInParent<PlayerController>();
                player.TouchPlatformDown();
                break;
            case Names.Breakable:
                player = gameObject.GetComponentInParent<PlayerController>();
                player.TouchPlatformDown();
                break;
            case Names.EnemyBoundingHit:
                player = gameObject.GetComponentInParent<PlayerController>();
                if (!player.IsDead())
                {
                    player.KillEnemy();
                    enemy = ((GameObject)col.gameObject).GetComponentInParent<SimpleEnemyController>();
                    enemy.GetComponent<BoxCollider2D>().enabled = false;
                    //enemy.Die();
                }
                break;
        }
    }

    protected virtual void OnTriggerExit2D(Collider2D col)
    {
        PlayerController player;
        SimpleEnemyController enemy;

        switch (col.tag)
        {
            case Names.Platform:
                player = gameObject.GetComponentInParent<PlayerController>();
                player.UnTouchPlatformDown();
                break;
            case Names.Question:
                player = gameObject.GetComponentInParent<PlayerController>();
                player.UnTouchPlatformDown();
                break;
            case Names.Pipe:
                player = gameObject.GetComponentInParent<PlayerController>();
                player.UnTouchPlatformDown();
                break;
            case Names.Breakable:
                player = gameObject.GetComponentInParent<PlayerController>();
                player.UnTouchPlatformDown();
                break;
        }
    }
}
