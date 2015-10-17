using UnityEngine;

public static class Helper
{
	public static Vector2 Aim(Vector2 initialPoint,Vector2 finalPoint)
	{
		Vector2 direction, aim ;
		float mod;
		
		direction = new Vector2(finalPoint.x - initialPoint.x,finalPoint.y - initialPoint.y);
		
		mod = Mathf.Sqrt(Mathf.Pow(direction.x, 2) + Mathf.Pow(direction.y, 2));
		
		if (mod == 0)
		{
			mod = Mathf.Sqrt(Mathf.Pow(finalPoint.x, 2) + Mathf.Pow(finalPoint.y, 2));
			finalPoint.x = finalPoint.x / mod;
			finalPoint.y = finalPoint.y / mod;
			aim = finalPoint;
		}
		else
		{
			direction.x = direction.x / mod;
			direction.y = direction.y / mod;
			aim = direction;
		}
		
		return aim;
	}

    public static Vector2 CalculateReflect(Vector2 vector,Vector2 normal)
	{
		float relNv;
		Vector2 reflect;
		
		relNv = vector.x * normal.x + vector.y * normal.y;
		reflect = new Vector2();
		reflect.x = (vector.x - (2 * relNv * normal.x));
		reflect.y = (vector.y - (2 * relNv * normal.y));
					
		
		return reflect;
	}
}
