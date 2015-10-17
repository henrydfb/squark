using UnityEngine;
using System.Collections;

public class MovingPlatformController : PlatformController 
{
    /// <summary>
    /// 
    /// </summary>
    public Vector2 startPoint;
    /// <summary>
    /// 
    /// </summary>
    public Vector2 endPoint;

    /// <summary>
    /// 
    /// </summary>
    public float speed;

    private Vector2 aimPoint;

    protected override void Start()
    {
        base.Start();

        startPoint = new Vector2(transform.position.x, transform.position.y);
        endPoint = new Vector2(transform.position.x + 2, transform.position.y);
        speed = 0.01f;

        aimPoint = endPoint;
    }

    protected override void Update()
    {
        base.Update();

        Vector2 pos,aim;

        pos = new Vector2(transform.position.x, transform.position.y);

        if (Vector2.Distance(pos, aimPoint) <= 0.1f)
        {
            if (aimPoint == endPoint)
                aimPoint = startPoint;
            else
                aimPoint = endPoint;
        }

        aim = Helper.Aim(pos, aimPoint);
        transform.position += new Vector3(aim.x,aim.y,0) * speed;
    }
}
