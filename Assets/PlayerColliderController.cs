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
            case Names.EnemyBoundingHit:
                player = gameObject.GetComponentInParent<PlayerController>();
                player.KillEnemy();
                enemy = ((GameObject)col.gameObject).GetComponentInParent<SimpleEnemyController>();
                enemy.Die();
                break;
        }
    }
}
