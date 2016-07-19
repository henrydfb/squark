using UnityEngine;
using System.Collections;

public class PlayerHorizontalCollider : MonoBehaviour 
{
    enum CollisionDir
    { 
        Left,
        Right
    }

    private GameObject collidingObj = null;

    private CollisionDir collDir;

    public void OnTriggerEnter2D(Collider2D col)
    {
        PlayerController player;

        player = gameObject.GetComponentInParent<PlayerController>();

        switch (tag)
        { 
            case Names.PlayerLeftCollider:
                //if (player.transform.position.x > col.transform.position.x + col.bounds.size.x / 2)
                //{
                    player.SetCollidingLeft(true,col.gameObject);
                    collidingObj = col.gameObject;
                    collDir = CollisionDir.Left;
                //}
                break;
            case Names.PlayerRightCollider:
                //if (player.transform.position.x < col.transform.position.x - col.bounds.size.x / 2)
                //{
                    player.SetCollidingRight(true, col.gameObject);
                    collidingObj = col.gameObject;
                    collDir = CollisionDir.Right;
                //}
                break;
            default:
                break;
        }
    }

    public void OnTriggerExit2D(Collider2D col)
    {
        PlayerController player;

        player = gameObject.GetComponentInParent<PlayerController>();

        if (collidingObj == col.gameObject)
        {
            switch (collDir)
            { 
                case CollisionDir.Left:
                    player.SetCollidingLeft(false, col.gameObject);
                    break;
                case CollisionDir.Right:
                    player.SetCollidingRight(false, col.gameObject);
                    break;
            }

            collidingObj = null;
        }
    }
}
