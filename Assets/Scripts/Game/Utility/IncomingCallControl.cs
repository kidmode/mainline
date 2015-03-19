using UnityEngine;
using System.Collections;

public static class IncomingCallControl
{

	#if UNITY_ANDROID && !UNITY_EDITOR
	private static AndroidJavaObject m_incomingCallControl = new AndroidJavaObject( "com.zoodles.kidmode.blockincomingcall.IncomingCallControl" );
	private static bool m_enable = false;
	#endif

	private static void GetInstance()
	{
		#if UNITY_ANDROID && !UNITY_EDITOR
		if( null == m_incomingCallControl )
		{
			m_incomingCallControl = new AndroidJavaObject( "com.zoodles.kidmode.blockincomingcall.IncomingCallControl" );
		}
		#endif
	}

	public static void StartBlock()
	{
		#if UNITY_ANDROID && !UNITY_EDITOR
		GetInstance ();
		m_incomingCallControl.Call("Register");
		m_enable = true;
		#endif
	}

	public static void EndBlock()
	{
		#if UNITY_ANDROID && !UNITY_EDITOR
		if( m_enable == false )
			return;
		GetInstance ();
		m_incomingCallControl.Call("Unregister");
		#endif
	}
}
