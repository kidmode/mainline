using UnityEngine;
using System.Collections;
using System.Collections.Generic;




public delegate void TweenFinishedHandler( UIElement p_element, Tweener.TargetVar p_targetVar );



public class Tweener
{
	public enum TargetVar
	{
		Position,
		Rotation,
		Scale,
		Alpha
	}

	public enum Style 
	{ 
		Standard, 
		QuadIn,
		QuadOut, 
		QuadInReverse, 
		QuadOutReverse, 
		QuadInOut
		//Bezier
	}

	private class Track
	{
		public TargetVar targetVar;

		public System.Object goal;
		public System.Object start;

		public Style style = Style.Standard;

		public float duration;

		public TweenFinishedHandler onFinishedHandler;

		public bool isFinished
		{
			get { return timer >= duration; }
			set 
			{ 
				timer = ( value ) 
				? timer = endTime : timer; 
			}
		}

		public bool loop;

		public float timer;
		public float endTime;
	}


	public Tweener( UIElement p_target )
	{
		DebugUtils.Assert( p_target != null );

		m_target = p_target;
		m_trackList = new List< Track >();
	}

	//Helpers for adding tweening tracks
	public void addPositionTrack( List< Vector3 > p_goal, float p_duration, 
	                     TweenFinishedHandler p_onFinishHandler = null, 
	                             Style p_style = Style.Standard, bool p_loop = false )
	{
		addTrack( null, (System.Object)p_goal, TargetVar.Position, p_duration, p_onFinishHandler, p_style, p_loop );
	}

	public void addRotationTrack( Quaternion p_start, Quaternion p_goal, float p_duration, 
	                             TweenFinishedHandler p_onFinishHandler = null, 
	                             Style p_style = Style.Standard, bool p_loop = false )
	{
		addTrack( (System.Object)p_start, (System.Object)p_goal, TargetVar.Rotation, p_duration, p_onFinishHandler, p_style, p_loop  );
	}

	public void addRotationTrack( float p_start, float p_goal, float p_duration, 
	                             TweenFinishedHandler p_onFinishHandler = null, 
	                             Style p_style = Style.Standard, bool p_loop = false )
	{

		Quaternion l_start = Quaternion.AngleAxis( p_start, Vector3.forward );
		Quaternion l_rotation = Quaternion.AngleAxis( p_goal, Vector3.forward );

		addTrack( (System.Object)l_start, (System.Object)l_rotation, TargetVar.Rotation, p_duration, p_onFinishHandler, p_style, p_loop  );
	}

	public void addScaleTrack( Vector3 p_start, Vector3 p_goal, float p_duration, 
	                     TweenFinishedHandler p_onFinishHandler = null, 
	                          Style p_style = Style.Standard, bool p_loop = false )
	{
		m_target.transform.localScale = p_start;
		addTrack( (System.Object)p_start, (System.Object)p_goal, TargetVar.Scale, p_duration, p_onFinishHandler, p_style, p_loop  );
	}

	public void addAlphaTrack( float p_start, float p_goal, float p_duration, 
	                          TweenFinishedHandler p_onFinishHandler = null, 
	                          Style p_style = Style.Standard, bool p_loop = false )
	{
		addTrack( (System.Object)p_start, (System.Object)p_goal, TargetVar.Alpha, p_duration, p_onFinishHandler, p_style, p_loop  );
	}



	public void addTrack( 	System.Object p_start, System.Object p_goal, 
	                        TargetVar p_targetVariable, float p_duration, 
	                     	TweenFinishedHandler p_onFinishHandler = null, 
	                     	Style p_style = Style.Standard, bool p_loop = false )
	{
		Track l_track = new Track();

		
		l_track.start = p_start;
		l_track.goal = p_goal;
		l_track.duration = p_duration;
		l_track.timer = 0;
		l_track.onFinishedHandler = p_onFinishHandler;
		l_track.targetVar = p_targetVariable;
		l_track.loop = p_loop;

		l_track.endTime = Time.unscaledTime + p_duration;

		DebugUtils.Assert( m_trackList != null );
		m_trackList.Add( l_track );

		switch( l_track.targetVar )
		{
		case TargetVar.Position:
			onPositionFinishedEvent += l_track.onFinishedHandler;
			break;
		case TargetVar.Rotation:
			onRotationFinishedEvent += l_track.onFinishedHandler;
			break;
		case TargetVar.Scale:
			onScaleFinishedEvent += l_track.onFinishedHandler;
			break;
		case TargetVar.Alpha:
			onAlphaFinishedEvent += l_track.onFinishedHandler;
			break;
		}
	}

	public void update( float p_delta )
	{
	//	_Debug.log ( m_trackList.Count );
		for( int i = m_trackList.Count - 1; i >= 0; --i )
		{
			Track l_track = m_trackList[ i ];

			l_track.timer += p_delta;
				
			if( l_track.isFinished )
			{
				_setTrackProgress( l_track, 1.0f );	

				if( l_track.loop )
				{
					l_track.timer = 0.0f;
					continue;
				}

				if( l_track.onFinishedHandler != null )
				{
					switch( l_track.targetVar )
					{
					case TargetVar.Position:
						onPositionFinishedEvent( m_target, l_track.targetVar );
						onPositionFinishedEvent -= l_track.onFinishedHandler;
						break;
					case TargetVar.Rotation:
						onRotationFinishedEvent( m_target, l_track.targetVar );
						onRotationFinishedEvent -= l_track.onFinishedHandler;
						break;
					case TargetVar.Scale:
						onScaleFinishedEvent( m_target, l_track.targetVar );
						onScaleFinishedEvent -= l_track.onFinishedHandler;
						break;
					case TargetVar.Alpha:
						onAlphaFinishedEvent( m_target, l_track.targetVar );
						onAlphaFinishedEvent -= l_track.onFinishedHandler;
						break;
					}
				}

				m_trackList.RemoveAt( i );
				continue;
			}
				
			float l_percent = _getPercentFromTrack( l_track );

			_setTrackProgress( l_track, l_percent );
		}
	}


	public void stop( bool p_deep )
	{
		foreach( Track t in m_trackList )
		{
			t.loop = false;
			t.isFinished = true;
		}

		if( p_deep )
		{
			int l_childCount = m_target.getChildCount();
			for( int i = 0; i < l_childCount; ++i )
			{
				UIElement l_child = m_target.getChildAt( i );
				l_child.tweener.stop( p_deep );
			}
		}
	}
	//------------------- Private Implementation ----------------------

	private UIElement m_target;
	private List<Track> m_trackList;
	
	private event TweenFinishedHandler onPositionFinishedEvent;
	private event TweenFinishedHandler onRotationFinishedEvent;
	private event TweenFinishedHandler onScaleFinishedEvent;
	private event TweenFinishedHandler onAlphaFinishedEvent;


	private void _setTrackProgress( Track p_track, float p_percent )
	{
		switch( p_track.targetVar )
		{
		case TargetVar.Position: 	_setPositionToPercentage( p_track, p_percent );		break;
		case TargetVar.Rotation: 	_setRotationToPercentage( p_track, p_percent ); 	break;
		case TargetVar.Scale:		_setScaleToPercentage( p_track, p_percent );		break;													
		case TargetVar.Alpha:		_setAlphaToPercentage( p_track, p_percent );		break;
		}
	}



	private float _getPercentFromTrack( Track p_track )
	{
		float l_percent =  p_track.timer / p_track.duration;
		
		switch( p_track.style )
		{
		case Tweener.Style.Standard:
			break;
		case Tweener.Style.QuadIn:
			l_percent = l_percent * l_percent;
			break;
		case Tweener.Style.QuadOut:
			l_percent = l_percent * ( l_percent - 2 ) * -1;
			break;
		case Tweener.Style.QuadInReverse:
			l_percent = ( 1 - l_percent ) * ( 1 - l_percent );
			break;
		case Tweener.Style.QuadOutReverse:
			l_percent = (1-l_percent) * ( (1-l_percent) - 2 ) * -1;
			break;
		case Tweener.Style.QuadInOut:
			if( l_percent < 0.5 )
				l_percent = l_percent * l_percent;
			else
				l_percent = l_percent * ( l_percent - 2 ) * -1;
			break;
		}
		
		if( l_percent > 1 )
			l_percent = 1;
		
		return l_percent;
	}
	
	
	private Vector3 _tweenPoint( float p_percentage, List<Vector3> p_pointList )
	{
		int l_numPoints = p_pointList.Count;
		float l_point = ((l_numPoints - 1) * p_percentage);
		
		//find the time between two points
		float l_percentage = l_point % 1;
		
		//Get index of 4 points
		int l_i1 = Mathf.FloorToInt( l_point );
		int l_i2 = Mathf.CeilToInt( l_point );
		int l_i0 = l_i1 - 1;
		int l_i3 = l_i2 + 1;
		
		//Make sure the indices are in range
		if (l_i0 < 0)
			l_i0 = l_i1;
		
		if (l_i2 >= l_numPoints)
			l_i2 = l_numPoints - 1;
		
		if (l_i3 >= l_numPoints)
			l_i3 = l_numPoints - 1;
		
		
		//cache powers
		float l_t = l_percentage;
		float l_t2 = l_t * l_t;
		float l_t3 = l_t2 * l_t;
		
		//Get the actual points
		Vector3 l_p0 = p_pointList[ l_i0 ];
		Vector3 l_p1 = p_pointList[ l_i1 ];
		Vector3 l_p2 = p_pointList[ l_i2 ];
		Vector3 l_p3 = p_pointList[ l_i3 ];
		
		Vector3 l_currentPoint = Vector3.zero;
		
		//tween using catmulrom
		l_currentPoint.x = 0.5f * ((2 * l_p1.x) 
		                           + (-l_p0.x + l_p2.x) * l_t 
		                           + (2 * l_p0.x - 5 * l_p1.x + 4 * l_p2.x - l_p3.x) * l_t2 
		                           + (-l_p0.x + 3 * l_p1.x- 3 * l_p2.x + l_p3.x) * l_t3);
		l_currentPoint.y = 0.5f * ((2 * l_p1.y) 
		                           + (-l_p0.y + l_p2.y) * l_t 
		                           + (2 * l_p0.y - 5 * l_p1.y + 4 * l_p2.y - l_p3.y) * l_t2 
		                           + (-l_p0.y + 3 * l_p1.y - 3 * l_p2.y + l_p3.y) * l_t3);	
		l_currentPoint.z = 0.5f * ((2 * l_p1.z) 
		                           + (-l_p0.z + l_p2.z) * l_t 
		                           + (2 * l_p0.z - 5 * l_p1.z + 4 * l_p2.z - l_p3.z) * l_t2 
		                           + (-l_p0.z + 3 * l_p1.z - 3 * l_p2.z + l_p3.z) * l_t3);	
		
		return l_currentPoint;
	}


//---------- Different sets for all the Target Variable types ----------

	private void _setPositionToPercentage( Track p_track, float p_percent )
	{
		List<Vector3> l_goal = p_track.goal as List<Vector3>;
		//DebugUtils.Assert( l_goal != null );

		Vector3 l_position = _tweenPoint( p_percent, l_goal );
		m_target.transform.localPosition = l_position;
	}

	private void _setRotationToPercentage( Track p_track, float p_percent )
	{
		Quaternion l_goal 	= (Quaternion)p_track.goal;
		Quaternion l_start	= (Quaternion)p_track.start;

		Quaternion l_rotation = Quaternion.Slerp( l_start, l_goal, p_percent );
		m_target.transform.localRotation = l_rotation;
	}

	private void _setScaleToPercentage( Track p_track, float p_percent )
	{
		Vector3 l_goal 	= (Vector3)p_track.goal;
		Vector3 l_start	= (Vector3)p_track.start;

		Vector3 l_scale = Vector3.Lerp( l_start, l_goal, p_percent );
		m_target.transform.localScale = l_scale;
	}

	private void _setAlphaToPercentage( Track p_track, float p_percent )
	{
		float l_goal 	= (float)p_track.goal;
		float l_start	= (float)p_track.start;

		float l_alpha 	= Mathf.Lerp( l_start, l_goal, p_percent );
		m_target.alpha = l_alpha;

		//Set m_target.active = true at here. In case m_target become true before addAlphaTrack is called.
		if (!m_target.active) 
		{
			m_target.active = true;
		}
	}
}
