using UnityEngine;
using System.Collections;

public class BezierPoint : System.Object
{
	public Vector2 main;
	public Vector2 control1;//Think of as left
	public Vector2 control2;//Right
	public BezierCurve curve1;//Left
	public BezierCurve curve2;//Right
	
	public BezierPoint (Vector2 m, Vector2 l, Vector2 r) 
	{
		main = m;
		control1 = l;
		control2 = r;
	}
}
