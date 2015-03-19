// Simple plugin wrapper for native calls.
using UnityEngine;
using System.Collections;

// We need this one for importing our IOS functions
using System.Runtime.InteropServices;
 
public class GCSPlugin : MonoBehaviour
{
	// Use this #if so that if you run this code on a different platform, you won't get errors.
#if UNITY_IPHONE
	[DllImport ("__Internal")]
	private static extern string  _getLocaleCountryCode();	
	
	[DllImport ("__Internal")]
    private static extern bool _isMusicPlaying ();
#endif

	public static string GetLocaleCountryCode()
	{
	    string l_countryCode = "GCS";
	    // We check for UNITY_IPHONE again so we don't try this if it isn't iOS platform.
#if UNITY_IPHONE
	    // Now we check that it's actually an iOS device/simulator, not the Unity Player. You only get plugins on the actual device or iOS Simulator.
	    if (Application.platform == RuntimePlatform.IPhonePlayer)
	    {
	        l_countryCode = _getLocaleCountryCode();
	    }
#endif
	    // TODO:  You could test for Android, PC, Mac, Web, etc and do something with a plugin for them here.
	 
	    return l_countryCode;		
	}
	
    public static bool IsMusicPlaying ()
    {
		bool l_isMusicPlaying  = false;
		
#if UNITY_IPHONE
        if ( Application.platform == RuntimePlatform.IPhonePlayer )
        {
            l_isMusicPlaying = _isMusicPlaying();
        }
#endif
        
        return l_isMusicPlaying;
    }
}