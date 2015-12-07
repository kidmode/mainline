using UnityEngine;

public static class FabricUtils {

	public static void Log (string format, params object[] list) {
		Debug.Log ("[Fabric] " + string.Format (format, list));
	}

	public static void Warn (string format, params object[] list) {
		Debug.LogWarning ("[Fabric] " + string.Format (format, list));
	}

	public static void Error (string format, params object[] list) {
		throw new System.Exception ("[Fabric] " + string.Format (format, list));
	}
}
