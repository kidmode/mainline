package com.zoodles.kidmode;

import android.content.SharedPreferences;
import android.content.SharedPreferences.Editor;
import android.os.AsyncTask;
import android.preference.PreferenceManager;

/**
 * Keys for values stored in application preferences.
 * 
 */
public class Preferences
{
	// ////////////////////////////////////////////////////////////////////
	// Members
	// ////////////////////////////////////////////////////////////////////

	protected App mApp;

	// ////////////////////////////////////////////////////////////////////
	// App preferences
	// ////////////////////////////////////////////////////////////////////

	protected boolean mIncomingCallsEnabled;
	protected int mExitAction;
	protected int mExitPinCode;

	public Preferences(
			App app)
	{
		mApp = app;
		reload();
	}

	/**
	 * Load/Reload the values from disk, this happens during construction and
	 * when com.zoodles.kidmode.parent.SettingsActivity#onStop() is called
	 */
	public void reload()
	{
		// Public preferences
		SharedPreferences sharedPrefs = getSharedPrefs();
		mIncomingCallsEnabled = sharedPrefs.getBoolean(PREFS_KEY_ALLOW_INCOMING_CALLS, true);
		mExitPinCode = sharedPrefs.getInt(PREFS_KEY_EXIT_PIN_CODE, PIN_CODE_DEFAULT);
		mExitAction = sharedPrefs.getInt(PREFS_KEY_EXIT_ACTION, DEFAULT_EXIT_ACTION);
	}

	/**
	 * Clears all persistent state. Used when logging out. Should be invoked
	 * from a background thread.
	 */
	public void clearAll()
	{
		execEditor(new SharedPrefEditor()
		{
			@Override
			public void editValue(Editor editor)
			{
				editor.remove(PREFS_KEY_EXIT_PIN_CODE);
				editor.remove(PREFS_KEY_EXIT_ACTION);
				editor.remove(PREFS_KEY_EMERGENCY_EXIT);
			}
		}, false);
		reload();
	}

	// ////////////////////////////////////////////////////////////////////
	// Preferences
	// ////////////////////////////////////////////////////////////////////

	/**
	 * Returns preference value to allow incoming calls. TRUE == allow incoming
	 * calls during Home Button Lock.
	 * 
	 * @return
	 */
	public boolean incomingCallsEnabled()
	{
		return mIncomingCallsEnabled;
	}

	/**
	 * @param allow
	 */
	public void enableIncomingCalls(boolean allow)
	{
		mIncomingCallsEnabled = allow;
	}

	/**
	 * Returns preference for exit action (PREFS_EXIT_ACTION_DRAW_Z,
	 * PREFS_EXIT_ACTION_BIRTH_YEAR)
	 * 
	 * @return
	 */
	public int exitAction()
	{
		return mExitAction;
	}

	/**
	 * Sets preferred exit action into preferences
	 * 
	 * @param action
	 */
	public void setExitAction(int action)
	{
		setExitAction(action, true);
	}

	/**
	 * Sets preferred exit action into preferences
	 * 
	 * @param action
	 */
	public void setExitAction(int action, boolean async)
	{
		mExitAction = action;
		editSharedValue(Preferences.PREFS_KEY_EXIT_ACTION, mExitAction, async);
	}

	public int exitPinCode()
	{
		return mExitPinCode;
	}

	/**
	 * Set PIN code to exit from Kid experience into Parent Dashboard or exit
	 * app entirely
	 * 
	 * @param code
	 */
	public void setExitPinCode(int code)
	{
		mExitPinCode = code;
		editSharedValue(Preferences.PREFS_KEY_EXIT_PIN_CODE, mExitPinCode, true);
	}

	// ////////////////////////////////////////////////////////////////////
	// #hasValue() methods
	// ////////////////////////////////////////////////////////////////////

	public boolean hasSharedValue(String key)
	{
		return getSharedPrefs().contains(key);
	}

	// ////////////////////////////////////////////////////////////////////
	// #getValue() methods
	// ////////////////////////////////////////////////////////////////////

	public boolean getSharedValue(String key, boolean defValue)
	{
		return getSharedPrefs().getBoolean(key, defValue);
	}

	public long getSharedValue(String key, long defValue)
	{
		return getSharedPrefs().getLong(key, defValue);
	}

	public int getSharedValue(String key, int defValue)
	{
		return getSharedPrefs().getInt(key, defValue);
	}

	public String getSharedValue(String key, String defValue)
	{
		return getSharedPrefs().getString(key, defValue);
	}

	// ////////////////////////////////////////////////////////////////////
	// #editValue() methods
	// ////////////////////////////////////////////////////////////////////

	public void editSharedValue(final String key, final int value, boolean async)
	{

		execEditor(new SharedPrefEditor()
		{
			@Override
			public void editValue(Editor editor)
			{
				editor.putInt(key, value);
			}
		}, async);
	}

	public void editSharedValue(final String key, final long value, boolean async)
	{

		execEditor(new SharedPrefEditor()
		{
			@Override
			public void editValue(Editor editor)
			{
				editor.putLong(key, value);
			}
		}, async);
	}

	public void editSharedValue(final String key, final boolean value, boolean async)
	{

		execEditor(new SharedPrefEditor()
		{
			@Override
			public void editValue(Editor editor)
			{
				editor.putBoolean(key, value);
			}
		}, async);

	}

	public void editSharedValue(final String key, final String value, boolean async)
	{

		execEditor(new SharedPrefEditor()
		{
			@Override
			public void editValue(Editor editor)
			{
				editor.putString(key, value);
			}
		}, async);

	}

	// ////////////////////////////////////////////////////////////////////
	// Helper Methods
	// ////////////////////////////////////////////////////////////////////

	/**
	 * Retrieve application preferences object for storing internal "shared"
	 * prefs. These are app configuration items which are persisted in
	 * preferences, are are displayed to the user via
	 * com.zoodles.kidmode.parent.SettingsActivity.
	 * 
	 * @return
	 */
	protected SharedPreferences getSharedPrefs()
	{
		return PreferenceManager.getDefaultSharedPreferences(mApp);
	}

	/**
	 * Edits a preferences item in the foreground or background by invoking the
	 * #edit method on the PrefEditor passed in
	 * 
	 * @param editor
	 * @param async
	 */
	private void execEditor(PrefEditor editor, boolean async)
	{
		if (async)
		{
			new EditTask().execute(editor);
		}
		else
		{
			editor.edit();
		}
	}

	// ////////////////////////////////////////////////////////////////////
	// Inner classes
	// ////////////////////////////////////////////////////////////////////

	/**
	 * Background task for editing preferences
	 */
	private static class EditTask extends AsyncTask<PrefEditor, Void, Void>
	{

		@Override
		protected Void doInBackground(PrefEditor... params)
		{
			setThreadName();
			params[0].edit();
			return null;
		}

		/**
		 * Set thread name of this AsyncTask so that crash reports are easier to
		 * understand.
		 * http://stackoverflow.com/questions/7684182/name-the-thread
		 * -of-an-asynctask
		 * 
		 * @return
		 */
		private void setThreadName()
		{
			Thread.currentThread().setName(this.getClass().getName());
		}
	}

	/**
	 * Interface used by the background task to edit preferences
	 */
	private static interface PrefEditor
	{

		public void edit();

		public void editValue(Editor editor);

	}

	/**
	 * Implementation for editing the shared preferences database.
	 */
	private static abstract class SharedPrefEditor implements PrefEditor
	{

		public void edit()
		{
			SharedPreferences prefs = PreferenceManager.getDefaultSharedPreferences(App.instance());
			Editor editor = prefs.edit();
			editValue(editor);
			editor.commit();
		}

		public abstract void editValue(Editor editor);
	}

	// ////////////////////////////////////////////////////////////////////
	// Constants
	// ////////////////////////////////////////////////////////////////////

	// Device options
	public static final String PREFS_KEY_EMERGENCY_EXIT = "PREFS_KEY_EMERGENCY_EXIT";
	public static final String PREFS_KEY_ALLOW_INCOMING_CALLS = "PREFS_KEY_ALLOW_INCOMING_CALLS";

	// ////////////////////////////////////////////////////////////////////
	// Exit Screen
	// ////////////////////////////////////////////////////////////////////

	public static final String PREFS_KEY_EXIT_ACTION = "PREFS_KEY_EXIT_ACTION";
	public static final String PREFS_KEY_EXIT_PIN_CODE = "PREFS_KEY_EXIT_PIN_CODE";

	public static final int PREFS_EXIT_ACTION_DRAW_Z = 1;
	public static final int PREFS_EXIT_ACTION_BIRTH_YEAR = 2;
	public static final int PREFS_EXIT_ACTION_PIN_CODE		= 3;

	public static final int PIN_CODE_DEFAULT = -1;
	public static final int DEFAULT_EXIT_ACTION = PREFS_EXIT_ACTION_BIRTH_YEAR;
}
