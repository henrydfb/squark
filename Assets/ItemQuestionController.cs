using UnityEngine;
using System.Collections;

public class ItemQuestionController : QuestionController 
{
    public GameObject mushroomPrefab;

    public override void Hit()
    {
        GameObject mush;
        if (state == State.Normal)
        {
            mush = (GameObject)Instantiate(mushroomPrefab, new Vector3(transform.position.x, transform.position.y + 0.32f, 0), Quaternion.identity);
            mush.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, 100));
        }

        base.Hit();
    }

    protected override void Update()
    {
        base.Update();

        //Destroy
        if ((transform.position.y + GetComponent<Renderer>().bounds.size.y / 2 <= gameController.downLimit))
        {
            //Count picked mushroom
            gameController.PickMushroom();
            Destroy(gameObject);
        }
    }
}
