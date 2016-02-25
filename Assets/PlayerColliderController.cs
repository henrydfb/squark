using UnityEngine;
using System.Collections;
using System;

public class PlayerColliderController : MonoBehaviour {

    GameController gameController;
    PlatformController currentPlatform;

	// Use this for initialization
	void Start () 
    {
        gameController = GameObject.Find(Names.GameController).GetComponent<GameController>();
        currentPlatform = null;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    protected virtual void OnTriggerEnter2D(Collider2D col)
    {
        PlayerController player;
        SimpleEnemyController enemy;
        PlatformController platform;
        double dist;

        switch (col.tag)
        { 
            case Names.Platform:
                platform = col.GetComponent<PlatformController>();
                player = gameObject.GetComponentInParent<PlayerController>();
                player.TouchPlatformDown();

                //Platforms
                /*if (currentPlatform != null)
                {
                    if (currentPlatform != platform)
                    {
                        //Current is at the left side
                        if (currentPlatform.transform.position.x < platform.transform.position.x)
                        {
                            dist = (currentPlatform.transform.position.x + (currentPlatform.GetComponent<BoxCollider2D>().bounds.size.x / 2)) - (platform.transform.position.x - (platform.GetComponent<BoxCollider2D>().bounds.size.x / 2));
                        }
                        else
                        {
                            dist = (currentPlatform.transform.position.x - (currentPlatform.GetComponent<BoxCollider2D>().bounds.size.x / 2)) - (platform.transform.position.x + (platform.GetComponent<BoxCollider2D>().bounds.size.x / 2));
                        }

                        dist = Math.Abs(Math.Round(dist,1));

                        //Update attention

                        //Low Jump platform
                        //What we are doing here is: calclulate the distance between gaps and check which type do they belong to
                        if (dist >= 0.8f && dist <= 1.0f)
                            gameController.AddAttentionValue(GameController.AttentionType.Low);
                        //Med Jump platform
                        else if (dist >= 1.4f && dist <= 1.6f)
                            gameController.AddAttentionValue(GameController.AttentionType.Med);
                        //High Jump platform
                        else
                            gameController.AddAttentionValue(GameController.AttentionType.Hig);

                        gameController.AddAttentionValue(GameController.AttentionType.Gap);
                    }
                }*/

                gameController.SaveGapData(currentPlatform, platform,"jump");
                currentPlatform = platform;

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
                    //Update attention
                    switch (enemy.jumpType)
                    { 
                        case Jump.HeightType.Long:
                            gameController.AddAttentionValue(GameController.AttentionType.Hig);
                            break;
                        case Jump.HeightType.Medium:
                            gameController.AddAttentionValue(GameController.AttentionType.Med);
                            break;
                        case Jump.HeightType.Short:
                            gameController.AddAttentionValue(GameController.AttentionType.Low);
                            break;
                    }
                    gameController.AddAttentionValue(GameController.AttentionType.Ene);

                    //Disable
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
