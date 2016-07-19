using UnityEngine;
using System.Collections;
using System;

public class PlayerColliderControllerRGT : PlayerColliderController 
{
    GameControllerRGT gameController;
    PlatformController currentPlatform;

	// Use this for initialization
	void Start () 
    {
        gameController = GameObject.Find(Names.GameController).GetComponent<GameControllerRGT>();
        currentPlatform = null;
	}

    protected override void OnTriggerEnter2D(Collider2D col)
    {
        base.OnTriggerEnter2D(col);

        PlayerController player;
        SimpleEnemyController enemy;
        PlatformController platform;

        switch (col.tag)
        {
            case Names.Platform:
                platform = col.GetComponent<PlatformController>();
                player = gameObject.GetComponentInParent<PlayerController>();
                player.TouchPlatformDown();

                gameController.SaveGapData(currentPlatform, platform, "jump");
                currentPlatform = platform;

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
                            gameController.AddAttentionValue(GameControllerRGT.AttentionType.Hig);
                            break;
                        case Jump.HeightType.Medium:
                            gameController.AddAttentionValue(GameControllerRGT.AttentionType.Med);
                            break;
                        case Jump.HeightType.Short:
                            gameController.AddAttentionValue(GameControllerRGT.AttentionType.Low);
                            break;
                    }
                    gameController.AddAttentionValue(GameControllerRGT.AttentionType.Ene);

                    //Disable
                    enemy.GetComponent<BoxCollider2D>().enabled = false;
                    //enemy.Die();
                }
                break;
            default:
                break;
        }
    }

    protected override void OnTriggerExit2D(Collider2D col)
    {
        PlayerController player;

        switch (col.tag)
        {
            case Names.Platform:
                player = gameObject.GetComponentInParent<PlayerController>();
                player.UnTouchPlatformDown();
                break;
            default:
                break;
        }
    }
}
