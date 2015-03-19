using UnityEngine;
using System.Collections;

public class BezierCurve : System.Object
{
	public Vector2[] points;
	public float aproxLength;
	public Rect rect;

	public Vector2 Get (float t) {
		int t2 = (int)Mathf.Round (t*(points.Length-1));
		return points[t2];
	}
	
	public void Init (Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3) 
	{
		Vector2 topleft = new Vector2(Mathf.Infinity,Mathf.Infinity);
		Vector2 bottomright = new Vector2(Mathf.NegativeInfinity,Mathf.NegativeInfinity);
		
		topleft.x = Mathf.Min (topleft.x,p0.x);
		topleft.x = Mathf.Min (topleft.x,p1.x);
		topleft.x = Mathf.Min (topleft.x,p2.x);
		topleft.x = Mathf.Min (topleft.x,p3.x);
		
		topleft.y = Mathf.Min (topleft.y,p0.y);
		topleft.y = Mathf.Min (topleft.y,p1.y);
		topleft.y = Mathf.Min (topleft.y,p2.y);
		topleft.y = Mathf.Min (topleft.y,p3.y);
		
		bottomright.x = Mathf.Max (bottomright.x,p0.x);
		bottomright.x = Mathf.Max (bottomright.x,p1.x);
		bottomright.x = Mathf.Max (bottomright.x,p2.x);
		bottomright.x = Mathf.Max (bottomright.x,p3.x);
		
		bottomright.y = Mathf.Max (bottomright.y,p0.y);
		bottomright.y = Mathf.Max (bottomright.y,p1.y);
		bottomright.y = Mathf.Max (bottomright.y,p2.y);
		bottomright.y = Mathf.Max (bottomright.y,p3.y);
		
		rect = new Rect (topleft.x,topleft.y,bottomright.x-topleft.x,bottomright.y-topleft.y);
		
		
		ArrayList ps = new ArrayList ();
		
		Vector2 point1  = Mathfx.CubicBezier (0,p0,p1,p2,p3);
		Vector2 point2  = Mathfx.CubicBezier (0.05f,p0,p1,p2,p3);
		Vector2 point3  = Mathfx.CubicBezier (0.1f,p0,p1,p2,p3);
		Vector2 point4  = Mathfx.CubicBezier (0.15f,p0,p1,p2,p3);
		
		Vector2 point5  = Mathfx.CubicBezier (0.5f,p0,p1,p2,p3);
		Vector2 point6  = Mathfx.CubicBezier (0.55f,p0,p1,p2,p3);
		Vector2 point7  = Mathfx.CubicBezier (0.6f,p0,p1,p2,p3);
		
		aproxLength = Vector2.Distance (point1,point2) + Vector2.Distance (point2,point3) + Vector2.Distance (point3,point4)  + Vector2.Distance (point5,point6)  + Vector2.Distance (point6,point7);
		
		_Debug.log (Vector2.Distance (point1,point2) + "     " + Vector2.Distance (point3,point4) + "   " + Vector2.Distance (point6,point7));
		aproxLength*= 4;
		
		float a2 = 0.5f/aproxLength;//Double the amount of points since the aproximation is quite bad
		for (float i = 0 ;i<1;i+=a2) {
			ps.Add (Mathfx.CubicBezier (i,p0,p1,p2,p3));
		}
		
		points = ps.ToArray(typeof(Vector2)) as Vector2[];
	}
	
	public BezierCurve (Vector2 main, Vector2 control1, Vector2 control2, Vector2 end) 
	{
		Init (main,control1,control2,end);
	}
}