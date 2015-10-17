using UnityEngine;
using System.Collections;

public class QuestionController : PlatformController 
{
    public enum State
    { 
        Normal,
        Hit
    }

    public Sprite hitSprite;

    public GameObject bonusPref;

    protected State state;

    void Start()
    {
        base.Start();

        state = State.Normal;
    }

    public State GetState()
    {
        return state;
    }

    public virtual void Hit()
    {
        GameObject bonus;
        if (state == State.Normal)
        {
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

            spriteRenderer.sprite = hitSprite;
            state = State.Hit;
            if (bonusPref != null)
            {
                bonus = (GameObject)Instantiate(bonusPref, new Vector3(transform.position.x, transform.position.y + 0.32f, 0), Quaternion.identity);
                bonus.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, 100));
                gameController.PickCoin();
                //Count for the performance data
                gameController.PickBonus();
            }
        }
    }
}
