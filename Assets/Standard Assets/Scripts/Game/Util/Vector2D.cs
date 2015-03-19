using UnityEngine;

public class Vector2D : object 
{
	public Vector2D(float p_x, float p_y)
	{
		x = p_x;
		y = p_y;
	}

	public Vector2D(float p_x)
	{
		x = p_x;
		y = 0f;
	}
	
	public Vector2D()
	{
		x = 0f;
		y = 0f;
	}
			
	public float length
	{
		get
		{
			return Mathf.Sqrt((x * x) + (y * y));
		}
	}
	
	public float lengthSquared
	{
		get
		{
			return ((x * x) + (y * y));
		}
	}
	
	public Vector2D add(Vector2D p_a)
	{
		return new Vector2D((x + p_a.x), (y + p_a.y));
	}
	
	public static float angleBetween(Vector2D p_a, Vector2D p_b)
	{
		Vector2D l_a = p_a.clone();
		Vector2D l_b = p_b.clone();
		
		l_a.normalize();
		l_b.normalize();
		
		return Mathf.Acos( (float) l_a.dotProduct(l_b));
	}
	
	public Vector2D clone()
	{
		return new Vector2D(x, y);
	}
	
	
	public void decrementBy(Vector2D p_a)
	{
		x -= p_a.x;
		y -= p_a.y;
	}
	
	public static float distance(Vector2D p_pt1, Vector2D p_pt2)
	{
		float l_dx = (p_pt2.x - p_pt1.x);
		float l_dy = (p_pt2.y - p_pt1.y);
		
		return Mathf.Sqrt((l_dx * l_dx) + (l_dy * l_dy));
	}
	
	public static float distanceSquared(Vector2D p_pt1, Vector2D p_pt2)
	{
		float l_dx = (p_pt2.x - p_pt1.x);
		float l_dy = (p_pt2.y - p_pt1.y);
		
		return ((l_dx * l_dx) + (l_dy * l_dy));
	}
	
	public double dotProduct(Vector2D p_a)
	{
		return ((x * p_a.x) + (y * p_a.y));
	}
	
	public double crossProduct(Vector2D p_a)
	{
		return ((x * p_a.y) - (y * p_a.x));
	}
	
	public bool equals(Vector2D p_toCompare)
	{
		return ((x == p_toCompare.x) && (y == p_toCompare.y));
	}
	
	public void incrementBy(Vector2D p_a)
	{
		x += p_a.x;
		y += p_a.y;
	}
	
	public bool nearEquals(Vector2D p_toCompare, float p_tolerance)
	{
		float l_diffX = x - p_toCompare.x;
		float l_diffY = y - p_toCompare.y;
	
		return (Mathf.Abs(l_diffX) < p_tolerance
				&& Mathf.Abs(l_diffY) < p_tolerance);
	}
	
	public void negate()
	{
		x *= -1;
		y *= -1;
	}
	
	public float normalize()
	{
		float l_length;
		
		l_length = this.length;
		
		if (0 != l_length)
		{
			x /= l_length;
			y /= l_length;
		}
		
		return l_length;
	}
	
	public void scaleBy(float p_s)
	{
		x *= p_s;
		y *= p_s;
	}
	
	public Vector2D subtract(Vector2D p_a)
	{
		return new Vector2D((x - p_a.x), (y - p_a.y));
	}
	
	public string toString()
	{
		return "Vector2D(" + x + ", " + y + ")";
	}
	
	public void zero()
	{
		x = 0;
		y = 0;
	}
	
	public Vector2D perpendicular()
	{
		return new Vector2D(-y, x);
	}
	
	public void orthogonalize()
	{
		float l_swap;
			
		l_swap = y;
			
		y = x;
		x = -l_swap;			
	}
	public void reflect(Vector2D p_norm)
	{
		float l_scalar;
		
		l_scalar = (float) (2.0 * this.dotProduct(p_norm));
		
		x += (l_scalar * -p_norm.x);
		y += (l_scalar * -p_norm.y);		
	}	
	
	public void rotate(float p_angle)
	{
		float l_x0 = x;
		float l_y0 = y;
	
		float l_sin = Mathf.Sin(p_angle);
		float l_cos = Mathf.Cos(p_angle);
		
		x = ((l_x0 * l_cos) - (l_y0 * l_sin));
		y = ((l_x0 * l_sin) + (l_y0 * l_cos));			
	}
	
	public void truncate(float p_maxLength)
	{
		if (length > p_maxLength)
		{
			normalize();
			scaleBy(p_maxLength);
		}
	}
	
	public void copy(Vector2D p_a)
	{
		x = p_a.x;
		y = p_a.y;
	}
	
	// returns 1 if p_v is clockwise of this vector or -1 if
	// it is anticlockwise (y axis points down a x axis to the right)
	public int sign(Vector2D p_v)
	{
		return (((y * p_v.x) > (x * p_v.y)) ? -1 : 1);
	}			
	
	public bool isZero()
	{
		return (((x * x) + (y * y)) < float.MinValue);
	}
	
	public static Vector2D X_AXIS = null;
	public static Vector2D Y_AXIS = null;
	
	public static Vector2D ORIGIN = null;
	
	public float x;
	public float y;
	
	public static void initializeGlobal()
	{
		X_AXIS = new Vector2D(1.0f, 0.0f);
		Y_AXIS = new Vector2D(0.0f, 1.0f);
		ORIGIN = new Vector2D(0.0f, 0.0f);			
	}
	
	public static void releaseGlobal()
	{
		X_AXIS = null;
		Y_AXIS = null;
		ORIGIN = null;			
	}
	
	public float distanceTo(Vector2D p_vector)
	{
		float l_differenceX = (p_vector.x - x);
		float l_differenceY = (p_vector.y - y);
		
		return Mathf.Sqrt(l_differenceX * l_differenceX + l_differenceY * l_differenceY);
	}
	
	public float distanceSquaredTo(Vector2D p_vector)
	{
		float l_differenceX = p_vector.x - x;
		l_differenceX = Mathf.Abs(l_differenceX);
		
		float l_differenceY = p_vector.y - y;
		l_differenceY = Mathf.Abs(l_differenceY);
		
		return l_differenceX * l_differenceX + l_differenceY * l_differenceY;
	}
}
