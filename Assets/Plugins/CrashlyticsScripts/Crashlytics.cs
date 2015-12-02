using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;
using System.Text.RegularExpressions;


public class Crashlytics : MonoBehaviour
{
#if UNITY_IOS && !UNITY_EDITOR
	[DllImport("__Internal")]
	private static extern void crash();
	
	[DllImport("__Internal")]
	private static extern void recordCustomExceptionName (string name, string reason, string stackTrace);
	
	[DllImport("__Internal")]
	private static extern void setDebugMode (int debugMode);
	
	[DllImport("__Internal")]
	private static extern bool isInitialized();
#endif

	private static Crashlytics instance;

	void Awake ()
	{
		if (instance == null) {
			RegisterExceptionHandlers();

			instance = this;
			DontDestroyOnLoad(this);
		} else if (instance != this) {
			// Must not be the first time Crashlytics is created so
			// we can destroy this instance's gameObject
			Destroy(this.gameObject);
		}
	}
	
	public static void RegisterExceptionHandlers ()
	{
		// We can only send logged exceptions if the SDK has been initialized
		if (IsSDKInitialized ()) {
			Log ("Registering exception handlers");

			AppDomain.CurrentDomain.UnhandledException += HandleException;	
			
#if UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5 || UNITY_4_6
			Application.RegisterLogCallback(HandleLog);
#else
			Application.logMessageReceived += HandleLog;
#endif
		} else {
			Log ("Did not register exception handlers: Crashlytics SDK was not initialized");
		}
	}

	static bool IsSDKInitialized ()
	{
#if UNITY_IOS && !UNITY_EDITOR
		return isInitialized ();
#elif UNITY_ANDROID && !UNITY_EDITOR
		var crashlyticsClass = new AndroidJavaClass("com.crashlytics.android.Crashlytics");
		AndroidJavaObject crashlyticsInstance = null;
		try {
			crashlyticsInstance = crashlyticsClass.CallStatic<AndroidJavaObject>("getInstance");
		}
		catch {
			crashlyticsInstance = null;
		}
		return crashlyticsInstance != null;
#else
		return false;
#endif
	}

	public static void SetDebugMode (bool debugMode)
	{
#if UNITY_IOS && !UNITY_EDITOR
		setDebugMode(debugMode ? 1 : 0);
#endif
	}

	/// <summary>
	/// Force a crash.
	/// This method only works on iOS.
	/// For Android, use ThrowNonFatal ().
	/// </summary>
	public static void Crash ()
	{
#if UNITY_IOS && !UNITY_EDITOR
		crash ();
#else
		Log ("Method Crash () is unimplemented on this platform");
#endif
	}

	/// <summary>
	/// Throws an exception.
	/// </summary>
	public static void ThrowNonFatal ()
	{
#if !UNITY_EDITOR
		string s = null;
		string l = s.ToLower ();
		Log (l);
#else
		Log ("Method ThrowNonFatal () is not invokable from the context of the editor");
#endif
	}

#if UNITY_IOS && !UNITY_EDITOR
	[Obsolete("StartWithApiKeyiOS is deprecated. Crashlytics is now initialized natively at launch.")]
	static void StartWithApiKeyiOS (string apiKey) {}
#elif UNITY_ANDROID && !UNITY_EDITOR
	[Obsolete("StartAndroid is deprecated. Crashlytics is now initialized natively at launch.")]
	static void StartAndroid () {}
#endif

	static void HandleException(object sender, UnhandledExceptionEventArgs eArgs)
	{
		Exception e = (Exception)eArgs.ExceptionObject;
		HandleLog (e.Message.ToString (), e.StackTrace.ToString (), LogType.Exception);
	}

	static void HandleLog(string message, string stackTraceString, LogType type)
	{
		if (type == LogType.Exception) {
#if UNITY_IOS && !UNITY_EDITOR
			HandleExceptioniOS (message, stackTraceString);
#elif UNITY_ANDROID && !UNITY_EDITOR
			HandleExceptionAndroid (message, stackTraceString);
#endif
		}
	}

#if UNITY_IOS && !UNITY_EDITOR
	static void HandleExceptioniOS (string message, string stackTraceString)
	{
		string[] messageParts = getMessageParts(message);
		recordCustomExceptionName(messageParts[0], messageParts[1], stackTraceString);
	}

	private static string[] getMessageParts (string message)
	{
		// Split into two parts so we only split on the first delimiter
		char[] delim = { ':' };
		string[] messageParts = message.Split(delim, 2, StringSplitOptions.None);

		foreach (string part in messageParts) {
			part.Trim ();
		}

		if (messageParts.Length == 2) {
			return messageParts;
		} else {
			return new string[] {"Exception", message};
		}
	}

#elif UNITY_ANDROID && !UNITY_EDITOR
	static void HandleExceptionAndroid (string message, string stackTraceString)
	{
		Log ("Recording exception: " + message);

		// new Exception(message)
		var exceptionClass = AndroidJNI.FindClass("java/lang/Exception");
		var exceptionConstructor = AndroidJNI.GetMethodID (exceptionClass, "<init>", "(Ljava/lang/String;)V");		
		var exceptionArgs = new jvalue[1];
		exceptionArgs[0].l = AndroidJNI.NewStringUTF(message);
		var exceptionObj = AndroidJNI.NewObject (exceptionClass, exceptionConstructor, exceptionArgs);

		// stackTrace = [StackTraceElement, ...]
		var stackTraceElClass = AndroidJNI.FindClass ("java/lang/StackTraceElement");
		var stackTraceElConstructor = AndroidJNI.GetMethodID (stackTraceElClass, "<init>", "(Ljava/lang/String;Ljava/lang/String;Ljava/lang/String;I)V");
		var parsedStackTrace = ParseStackTraceString (stackTraceString);
		var stackTraceArray = AndroidJNI.NewObjectArray(parsedStackTrace.Length, stackTraceElClass, IntPtr.Zero);
		for (var i = 0; i < parsedStackTrace.Length; i++) {
			var frame = parsedStackTrace[i];

			// new StackTraceElement()
			var stackTraceArgs = new jvalue[4];
			stackTraceArgs[0].l = AndroidJNI.NewStringUTF(frame["class"]);
			stackTraceArgs[1].l = AndroidJNI.NewStringUTF(frame["method"]);
			stackTraceArgs[2].l = AndroidJNI.NewStringUTF(frame["file"]);
			stackTraceArgs[3].i = Int32.Parse(frame["line"]);
			var stackTraceEl = AndroidJNI.NewObject (stackTraceElClass, stackTraceElConstructor, stackTraceArgs);
			AndroidJNI.SetObjectArrayElement(stackTraceArray, i, stackTraceEl);
		}

		// exception.setStackTrace(stackTraceArray)
		var setStackTraceMethod = AndroidJNI.GetMethodID (exceptionClass, "setStackTrace", "([Ljava/lang/StackTraceElement;)V");
		var setStackTraceArgs = new jvalue[1];
		setStackTraceArgs[0].l = stackTraceArray;
		AndroidJNI.CallVoidMethod (exceptionObj, setStackTraceMethod, setStackTraceArgs);

		// Crashlytics.logException(exception)
		var crashClass = AndroidJNI.FindClass ("com/crashlytics/android/Crashlytics");
		var logExceptionMethod = AndroidJNI.GetStaticMethodID (crashClass, "logException", "(Ljava/lang/Throwable;)V");
		var logExceptionArgs = new jvalue[1];
		logExceptionArgs[0].l = exceptionObj;
		AndroidJNI.CallStaticVoidMethod (crashClass, logExceptionMethod, logExceptionArgs);
	}
#endif

	static Dictionary<string, string>[] ParseStackTraceString (string stackTraceString)
	{
		string[] splitStackTrace = stackTraceString.Split(Environment.NewLine.ToCharArray());
		var result = new List< Dictionary<string, string> >();

		foreach (var frameString in splitStackTrace) {
			var regex = @"(.+)\.(.+) \((.*)\)";
			var matches = Regex.Matches(frameString, regex);

			if (matches.Count != 1) {
				continue;
			}

			var match = matches[0];
			var dict = new Dictionary<string, string>();
			dict.Add("class", match.Groups[1].Value);
			dict.Add("method", match.Groups[2].Value + " (" + match.Groups[3].Value + ")");
			dict.Add("file", match.Groups[1].Value);
			dict.Add("line", "0");

			result.Add (dict);
		}

		return result.ToArray();
	}
	
	static void Log (string s)
	{
#if UNITY_ANDROID && !UNITY_EDITOR
		var logClass = new AndroidJavaClass("android.util.Log");
		logClass.CallStatic<int> ("d", "Crashlytics", s);
#else
		Debug.Log ("[Crashlytics] " + s);
#endif
	}
}
