package com.onevcat.uniwebview;

import com.androidnative.AndroidNativeBridge;
import com.gamecloudstudios.ZoodlesDispatcher;
import com.unity3d.player.UnityPlayer;
import com.unity3d.player.UnityPlayerNativeActivity;
import com.zoodles.kidmode.App;
import com.zoodles.kidmode.IntentConstants;
import com.zoodles.kidmode.Preferences;
import com.zoodles.kidmode.activity.LauncherActivity;
import com.zoodles.kidmode.activity.kid.exit.ExitAppActivity;
import com.zoodles.kidmode.activity.parent.ParentDashboardLauncherActivity;
import com.zoodles.kidmode.features.DeviceInfo;
import com.zoodles.kidmode.model.content.InstalledApp;
import com.zoodles.kidmode.model.content.NativeApp;
import com.zoodles.kidmode.service.NativeAppService;
import com.zoodles.kidmode.util.ChildLock;
import com.zoodles.kidmode.util.ScreenLock;

import android.annotation.SuppressLint;
import android.app.Activity;
import android.app.ActivityManager;
import android.app.ActivityManager.RunningTaskInfo;
import android.app.AlarmManager;
import android.app.PendingIntent;
import android.content.ComponentName;
import android.content.Context;
import android.content.Intent;
import android.content.pm.ApplicationInfo;
import android.content.pm.PackageInfo;
import android.content.pm.PackageManager;
import android.content.pm.ResolveInfo;
import android.content.res.Configuration;
import android.net.Uri;
import android.os.Bundle;
import android.os.PowerManager;
import android.os.Process;
import android.os.SystemClock;
import android.util.Log;
import android.view.KeyEvent;
import android.view.MotionEvent;
import android.webkit.CookieSyncManager;
import android.webkit.ValueCallback;
import android.webkit.CookieManager;
import android.widget.Toast;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.Collections;
import java.util.HashMap;
import java.util.List;

import android.os.Handler;
import android.provider.Settings;
import android.content.pm.ActivityInfo;

@SuppressLint("NewApi")
public class AndroidPlugin extends AndroidNativeBridge
{
	public final static int FILECHOOSER_RESULTCODE = 1;
	protected static ValueCallback<Uri> _uploadMessages;
	protected static final String LOG_TAG = "UniWebView";

	protected static final String POWER_SAVING_WHITELIST = "com.htc.powersavinglauncher";
	private static final String HTC_LAUNCHER_WHITELIST = "com.htc.launcher";

	public static Activity getActivity()
	{
		return UnityPlayer.currentActivity;
	}

	public static void setUploadMessage(ValueCallback<Uri> message)
	{
		_uploadMessages = message;
	}

	@Override
	protected void onCreate(Bundle savedInstanceState)
	{
		super.onCreate(savedInstanceState);
		CookieSyncManager.createInstance(AndroidPlugin.getActivity());

		App l_app = App.instance();
		ChildLock l_childLock = l_app.childLock();
		l_childLock.setupHomeReplacement(l_app);

		m_whiteList = new ArrayList<String>();
		m_whiteList.add(POWER_SAVING_WHITELIST);
		m_whiteList.add(HTC_LAUNCHER_WHITELIST);

		Intent l_intent = this.getIntent();
		checkForDashboardAction(l_intent);
	}

	@Override
	public void onNewIntent(Intent p_intent)
	{
		super.onNewIntent(p_intent);
		checkForDashboardAction(p_intent);
	}

	private void checkForDashboardAction(Intent p_intent)
	{
		if (null != p_intent)
		{
			String l_action = p_intent.getAction();
			if (ParentDashboardLauncherActivity.DASHBOARD_ACTION.equals(l_action) || ExitAppActivity.KIDMODE_EXIT_ACTION.equals(l_action))
			{
				sendDashboardMessage(l_action);
			}
		}
	}

	private void sendDashboardMessage(String p_action)
	{
		Log.v("Zoodles", "Sending goto dashboard message to game logic");
		UnityPlayer.UnitySendMessage("GameLogic", "gotoParentDashboard", p_action);
	}

	// It is a black magic in onPause and onResume
	// to make the unity view not disappear when return from background
	// Something about inactive activity which might be not reload when resume
	// from bg.
	@Override
	protected void onPause()
	{
		super.onPause();
		ShowAllWebViewDialogs(false);

		CookieSyncManager manager = CookieSyncManager.getInstance();
		if (manager != null)
		{
			manager.stopSync();
		}

		if (m_tryRelock)
		{
			updateLockTaskOnResume();
		}

		m_isInForeground = false;
		checkForPreemption();
	}

	@Override
	protected void onStop()
	{
		super.onStop();
		m_exitHint = true;
		checkForPreemption();
	}

	@Override
	protected void onResume()
	{
		super.onResume();

		ShowAllWebViewDialogs(false);
		CookieSyncManager manager = CookieSyncManager.getInstance();
		if (manager != null)
		{
			manager.startSync();
		}

		App l_app = App.instance();
		ChildLock l_childLock = l_app.childLock();
		if (App.instance().childLock().inChildLock())
		{
			// l_app.stopLockScreenService();
			// l_app.stopWatcherLockScreenService();
			// l_app.startWatcherLockScreenService();

			l_childLock.startWatchForPreemption();
			l_childLock.allowedAppStopped();
			m_isLockPaused = false;
		}

		if (m_requestIntentSent)
		{
			m_requestIntentSent = false;
			m_requestFinished = true;
		}

		m_isInForeground = true;
		if (m_kidModeActive && m_isLockPaused)
		{
			m_tryRelock = true;
		}

		// ActivityManager activityManager = (ActivityManager)
		// getApplicationContext().getSystemService(ACTIVITY_SERVICE);
		// if (activityManager.isInLockTaskMode() == false)
		// {
		// this.startLockTask();
		// }

		restartHandler.postDelayed(restartRunnable, 200);

		AlarmManager alm = (AlarmManager) this.getSystemService(Context.ALARM_SERVICE);
		alm.cancel(PendingIntent.getActivity(this, 0, new Intent(this, this.getClass()), 0));
	}

	public boolean isInForeground()
	{
		return m_isInForeground;
	}

	public void _disableHomeButton()
	{
		runSafelyOnUiThread(new Runnable()
		{
			@Override
			public void run()
			{
				// Turn off home button after request has been satisfied.
				App l_app = App.instance();
				ChildLock l_childLock = l_app.childLock();
				l_childLock.removeHomeReplacement(App.instance());
			}
		});
	}

	public Handler restartHandler = new Handler();
	public Runnable restartRunnable = new Runnable()
	{
		@Override
		public void run()
		{
			ShowAllWebViewDialogs(true);
		}
	};

	@Override
	public void onConfigurationChanged(Configuration newConfig)
	{
		super.onConfigurationChanged(newConfig);
		Log.d(AndroidPlugin.LOG_TAG, "Rotation: " + newConfig.orientation);
		for (UniWebViewDialog dialog : UniWebViewManager.Instance().allDialogs())
		{
			dialog.updateContentSize();
			dialog.HideSystemUI();
		}
	}

	@Override
	protected void onActivityResult(int requestCode, int resultCode, Intent intent)
	{
		super.onActivityResult(requestCode, resultCode, intent);
		if (requestCode == FILECHOOSER_RESULTCODE)
		{
			if (null != _uploadMessages)
			{
				Uri result = intent == null || resultCode != Activity.RESULT_OK ? null : intent.getData();
				_uploadMessages.onReceiveValue(result);
				_uploadMessages = null;
			}
		}
	}

	public static void _UniWebViewInit(final String name, final int top, final int left, final int bottom, final int right)
	{
		runSafelyOnUiThread(new Runnable()
		{
			@Override
			public void run()
			{
				Log.d(LOG_TAG, "_UniWebViewInit");
				UniWebViewDialog.DialogListener listener = new UniWebViewDialog.DialogListener()
				{
					public void onPageFinished(UniWebViewDialog dialog, String url)
					{
						Log.d(LOG_TAG, "page load finished: " + url);
						UnityPlayer.UnitySendMessage(name, "LoadComplete", "");
					}

					public void onPageStarted(UniWebViewDialog dialog, String url)
					{
						Log.d(LOG_TAG, "page load started: " + url);
						UnityPlayer.UnitySendMessage(name, "LoadBegin", url);
					}

					public void onReceivedError(UniWebViewDialog dialog, int errorCode, String description, String failingUrl)
					{
						Log.d(LOG_TAG, "page load error: " + failingUrl + " Error: " + description);
						UnityPlayer.UnitySendMessage(name, "LoadComplete", description);
					}

					public boolean shouldOverrideUrlLoading(UniWebViewDialog dialog, String url)
					{
						boolean shouldOverride = false;
						if (url.startsWith("mailto:"))
						{
							Intent intent = new Intent(Intent.ACTION_SENDTO, Uri.parse(url));
							getActivity().startActivity(intent);
							shouldOverride = true;
						}
						else if (url.startsWith("tel:"))
						{
							Intent intent = new Intent(Intent.ACTION_DIAL, Uri.parse(url));
							getActivity().startActivity(intent);
						}
						else
						{
							boolean canResponseScheme = false;
							for (String scheme : dialog.schemes)
							{
								if (url.startsWith(scheme + "://"))
								{
									canResponseScheme = true;
									break;
								}
							}
							if (canResponseScheme)
							{
								UnityPlayer.UnitySendMessage(name, "ReceivedMessage", url);
								shouldOverride = true;
							}
						}
						return shouldOverride;
					}

					public void onDialogShouldCloseByBackButton(UniWebViewDialog dialog)
					{
						Log.d(LOG_TAG, "dialog should be closed");
						UnityPlayer.UnitySendMessage(name, "WebViewDone", "");
					}

					public void onDialogKeyDown(UniWebViewDialog dialog, int keyCode)
					{
						UnityPlayer.UnitySendMessage(name, "WebViewKeyDown", Integer.toString(keyCode));
					}

					public void onDialogClose(UniWebViewDialog dialog)
					{
						UniWebViewManager.Instance().removeUniWebView(name);
					}

					public void onJavaScriptFinished(UniWebViewDialog dialog, String result)
					{
						UnityPlayer.UnitySendMessage(name, "EvalJavaScriptFinished", result);
					}
				};
				UniWebViewDialog dialog = new UniWebViewDialog(getActivity(), listener);
				dialog.changeSize(top, left, bottom, right);
				UniWebViewManager.Instance().setUniWebView(name, dialog);
			}
		});
	}

	public static void _UniWebViewLoad(final String name, final String url)
	{
		runSafelyOnUiThread(new Runnable()
		{
			@Override
			public void run()
			{
				Log.d(LOG_TAG, "_UniWebViewLoad");
				UniWebViewDialog dialog = UniWebViewManager.Instance().getUniWebViewDialog(name);
				if (dialog != null)
				{
					dialog.load(url);
				}
			}
		});
	}

	public static void _UniWebViewReload(final String name)
	{
		runSafelyOnUiThread(new Runnable()
		{
			@Override
			public void run()
			{
				Log.d(LOG_TAG, "_UniWebViewReload");
				UniWebViewDialog dialog = UniWebViewManager.Instance().getUniWebViewDialog(name);
				if (dialog != null)
				{
					dialog.reload();
				}
			}
		});
	}

	public static void _UniWebViewStop(final String name)
	{
		runSafelyOnUiThread(new Runnable()
		{
			@Override
			public void run()
			{
				Log.d(LOG_TAG, "_UniWebViewStop");
				UniWebViewDialog dialog = UniWebViewManager.Instance().getUniWebViewDialog(name);
				if (dialog != null)
				{
					dialog.stop();
				}
			}
		});
	}

	public static void _UniWebViewChangeSize(final String name, final int top, final int left, final int bottom, final int right)
	{
		runSafelyOnUiThread(new Runnable()
		{
			@Override
			public void run()
			{
				Log.d(LOG_TAG, "_UniWebViewChangeSize");
				UniWebViewDialog dialog = UniWebViewManager.Instance().getUniWebViewDialog(name);
				if (dialog != null)
				{
					dialog.changeSize(top, left, bottom, right);
				}
			}
		});
	}

	public static void _UniWebViewShow(final String name)
	{
		runSafelyOnUiThread(new Runnable()
		{
			@Override
			public void run()
			{
				Log.d(LOG_TAG, "_UniWebViewShow");
				UniWebViewDialog dialog = UniWebViewManager.Instance().getUniWebViewDialog(name);
				if (dialog != null)
				{
					dialog.setShow(true);
				}
			}
		});
	}

	public static void _UniWebViewDismiss(final String name)
	{
		runSafelyOnUiThread(new Runnable()
		{
			@Override
			public void run()
			{
				Log.d(LOG_TAG, "_UniWebViewHide");
				UniWebViewDialog dialog = UniWebViewManager.Instance().getUniWebViewDialog(name);
				if (dialog != null)
				{
					dialog.setShow(false);
				}
			}
		});
	}

	public static void _UniWebViewEvaluatingJavaScript(final String name, final String js)
	{
		runSafelyOnUiThread(new Runnable()
		{
			@Override
			public void run()
			{
				Log.d(LOG_TAG, "_UniWebViewEvaluatingJavaScript");
				UniWebViewDialog dialog = UniWebViewManager.Instance().getUniWebViewDialog(name);
				if (dialog != null)
				{
					dialog.loadJS(js);
				}
			}
		});
	}

	public static void _UniWebViewAddJavaScript(final String name, final String js)
	{
		runSafelyOnUiThread(new Runnable()
		{
			@Override
			public void run()
			{
				Log.d(LOG_TAG, "_UniWebViewAddJavaScript");
				UniWebViewDialog dialog = UniWebViewManager.Instance().getUniWebViewDialog(name);
				if (dialog != null)
				{
					dialog.addJs(js);
				}
			}
		});
	}

	public static void _UniWebViewCleanCache(final String name)
	{
		runSafelyOnUiThread(new Runnable()
		{
			@Override
			public void run()
			{
				Log.d(LOG_TAG, "_UniWebViewCleanCache");
				UniWebViewDialog dialog = UniWebViewManager.Instance().getUniWebViewDialog(name);
				if (dialog != null)
				{
					dialog.cleanCache();
				}
			}
		});
	}

	public static void _UniWebViewCleanCookie(final String name, final String key)
	{
		runSafelyOnUiThread(new Runnable()
		{
			@Override
			public void run()
			{
				Log.d(LOG_TAG, "_UniWebViewCleanCookie");

				CookieManager cm = CookieManager.getInstance();
				if (key == null || key.length() == 0)
				{
					Log.d(LOG_TAG, "Cleaning all cookies");
					cm.removeAllCookie();
				}
				else
				{
					Log.d(LOG_TAG, "Setting an empty cookie for: " + key);
					cm.setCookie(key, "");
				}

				CookieSyncManager manager = CookieSyncManager.getInstance();
				if (manager != null)
				{
					manager.sync();
				}
			}
		});
	}

	public static void _UniWebViewDestroy(final String name)
	{
		runSafelyOnUiThread(new Runnable()
		{
			@Override
			public void run()
			{
				Log.d(LOG_TAG, "_UniWebViewDestroy");
				UniWebViewDialog dialog = UniWebViewManager.Instance().getUniWebViewDialog(name);
				if (dialog != null)
				{
					dialog.destroy();
				}
			}
		});
	}

	public static void _UniWebViewTransparentBackground(final String name, final boolean transparent)
	{
		runSafelyOnUiThread(new Runnable()
		{
			@Override
			public void run()
			{
				Log.d(LOG_TAG, "_UniWebViewTransparentBackground");
				UniWebViewDialog dialog = UniWebViewManager.Instance().getUniWebViewDialog(name);
				if (dialog != null)
				{
					dialog.setTransparent(transparent);
				}
			}
		});
	}

	public static void _UniWebViewSetSpinnerShowWhenLoading(final String name, final boolean show)
	{
		runSafelyOnUiThread(new Runnable()
		{
			@Override
			public void run()
			{
				Log.d(LOG_TAG, "_UniWebViewSetSpinnerShowWhenLoading: " + show);
				UniWebViewDialog dialog = UniWebViewManager.Instance().getUniWebViewDialog(name);
				if (dialog != null)
				{
					dialog.setSpinnerShowWhenLoading(show);
				}
			}
		});
	}

	public static void _UniWebViewSetSpinnerText(final String name, final String text)
	{
		runSafelyOnUiThread(new Runnable()
		{
			@Override
			public void run()
			{
				Log.d(LOG_TAG, "_UniWebViewSetSpinnerText: " + text);
				UniWebViewDialog dialog = UniWebViewManager.Instance().getUniWebViewDialog(name);
				if (dialog != null)
				{
					dialog.setSpinnerText(text);
				}
			}
		});
	}

	public static void _UniWebViewGoBack(final String name)
	{
		runSafelyOnUiThread(new Runnable()
		{
			@Override
			public void run()
			{
				Log.d(LOG_TAG, "_UniWebViewGoBack");
				UniWebViewDialog dialog = UniWebViewManager.Instance().getUniWebViewDialog(name);
				if (dialog != null)
				{
					dialog.goBack();
				}
			}
		});
	}

	public static void _UniWebViewGoForward(final String name)
	{
		runSafelyOnUiThread(new Runnable()
		{
			@Override
			public void run()
			{
				Log.d(LOG_TAG, "_UniWebViewGoForward");
				UniWebViewDialog dialog = UniWebViewManager.Instance().getUniWebViewDialog(name);
				if (dialog != null)
				{
					dialog.goForward();
				}
			}
		});
	}

	public static void _UniWebViewLoadHTMLString(final String name, final String htmlString, final String baseURL)
	{
		runSafelyOnUiThread(new Runnable()
		{
			@Override
			public void run()
			{
				Log.d(LOG_TAG, "_UniWebViewLoadHTMLString");
				UniWebViewDialog dialog = UniWebViewManager.Instance().getUniWebViewDialog(name);
				if (dialog != null)
				{
					dialog.loadHTMLString(htmlString, baseURL);
				}
			}
		});
	}

	public static String _UniWebViewGetCurrentUrl(final String name)
	{
		UniWebViewDialog dialog = UniWebViewManager.Instance().getUniWebViewDialog(name);
		if (dialog != null)
		{
			return dialog.getUrl();
		}
		return "";
	}

	public static void _UniWebViewSetBackButtonEnable(final String name, final boolean enable)
	{
		runSafelyOnUiThread(new Runnable()
		{
			@Override
			public void run()
			{
				Log.d(LOG_TAG, "_UniWebViewSetBackButtonEnable:" + enable);
				UniWebViewDialog dialog = UniWebViewManager.Instance().getUniWebViewDialog(name);
				if (dialog != null)
				{
					dialog.setBackButtonEnable(enable);
				}
			}
		});
	}

	public static void _UniWebViewSetBounces(final String name, final boolean enable)
	{
		runSafelyOnUiThread(new Runnable()
		{
			@Override
			public void run()
			{
				Log.d(LOG_TAG, "_UniWebViewSetBounces:" + enable);
				UniWebViewDialog dialog = UniWebViewManager.Instance().getUniWebViewDialog(name);
				if (dialog != null)
				{
					dialog.setBounces(enable);
				}
			}
		});
	}

	public static void _UniWebViewSetUseZoodleInterfacees(final String name, final boolean enable)
	{
		runSafelyOnUiThread(new Runnable()
		{
			@Override
			public void run()
			{
				Log.d(LOG_TAG, "_UniWebViewSetBounces:" + enable);
				UniWebViewDialog dialog = UniWebViewManager.Instance().getUniWebViewDialog(name);
				if (dialog != null)
				{
					dialog.setUseZoodlesInterface(enable);
				}
			}
		});
	}

	public static void _UniWebViewOverwriteAudioInterface(final String name)
	{
		runSafelyOnUiThread(new Runnable()
		{
			@Override
			public void run()
			{
				UniWebViewDialog dialog = UniWebViewManager.Instance().getUniWebViewDialog(name);
				if (dialog != null)
				{
					dialog.overwriteAudioInterface();
				}
			}
		});
	}

	public static void _UniWebViewSetZoomEnable(final String name, final boolean enable)
	{
		runSafelyOnUiThread(new Runnable()
		{
			@Override
			public void run()
			{
				Log.d(LOG_TAG, "_UniWebViewSetZoomEnable:" + enable);
				UniWebViewDialog dialog = UniWebViewManager.Instance().getUniWebViewDialog(name);
				if (dialog != null)
				{
					dialog.setZoomEnable(enable);
				}
			}
		});
	}

	public static void _UniWebViewAddUrlScheme(final String name, final String scheme)
	{
		runSafelyOnUiThread(new Runnable()
		{
			@Override
			public void run()
			{
				Log.d(LOG_TAG, "_UniWebViewAddUrlScheme:" + scheme);
				UniWebViewDialog dialog = UniWebViewManager.Instance().getUniWebViewDialog(name);
				if (dialog != null)
				{
					dialog.addUrlScheme(scheme);
				}
			}
		});
	}

	public static void _UniWebViewRemoveUrlScheme(final String name, final String scheme)
	{
		runSafelyOnUiThread(new Runnable()
		{
			@Override
			public void run()
			{
				Log.d(LOG_TAG, "_UniWebViewAddUrlScheme:" + scheme);
				UniWebViewDialog dialog = UniWebViewManager.Instance().getUniWebViewDialog(name);
				if (dialog != null)
				{
					dialog.removeUrlScheme(scheme);
				}
			}
		});
	}

	public static void _UniWebViewUseWideViewPort(final String name, final boolean use)
	{
		runSafelyOnUiThread(new Runnable()
		{
			@Override
			public void run()
			{
				Log.d(LOG_TAG, "_UniWebViewUseWideViewPort:" + use);
				UniWebViewDialog dialog = UniWebViewManager.Instance().getUniWebViewDialog(name);
				if (dialog != null)
				{
					dialog.useWideViewPort(use);
				}
			}
		});
	}

	protected static void runSafelyOnUiThread(final Runnable r)
	{
		getActivity().runOnUiThread(new Runnable()
		{
			public void run()
			{
				try
				{
					r.run();
				}
				catch (Exception e)
				{
					Log.d(LOG_TAG, "UniWebView should run on UI thread: " + e.getMessage());
				}
			}
		});
	}

	protected void ShowAllWebViewDialogs(boolean show)
	{
		ArrayList<UniWebViewDialog> webViewDialogs = UniWebViewManager.Instance().getShowingWebViewDialogs();
		for (UniWebViewDialog webViewDialog : webViewDialogs)
		{
			if (show)
			{
				Log.d(LOG_TAG, webViewDialog + "goForeGround");
				webViewDialog.goForeGround();
				webViewDialog.HideSystemUI();
			}
			else
			{
				Log.d(LOG_TAG, webViewDialog + "goBackGround");
				webViewDialog.goBackGround();
				webViewDialog.HideSystemUI();
			}
		}
	}

	//
	// ==================== Kids Mode functionality ======================
	//

	/*
	 * Closes the system tray after the user tries to open it.
	 */

	@Override
	public void onWindowFocusChanged(boolean hasFocus)
	{
		super.onWindowFocusChanged(hasFocus);

		if (App.instance().childLock().inChildLock())
		{
			if (null == m_screenLock)
			{
				m_screenLock = new ScreenLock(this);
			}

			m_screenLock.closeStatusBar(this);
		}

		// Closes all system windows, including the power menu and status bar on
		// window focus.

		// if (!hasFocus && App.instance().childLock().inChildLock())
		// {
		// Intent closeDialog = new Intent(Intent.ACTION_CLOSE_SYSTEM_DIALOGS);
		// sendBroadcast(closeDialog);
		// }
	}

	/*
	 * Unity function to enable and disable the use of kids mode.
	 */

	private boolean m_kidModeActive = false;
	private boolean m_isLockPaused = false;
	private boolean m_tryRelock = false;

	@SuppressLint("NewApi")
	public void _setKidsModeActive(boolean p_isActive)
	{
		final boolean l_isActive = p_isActive;
		runSafelyOnUiThread(new Runnable()
		{
			@Override
			public void run()
			{
				Log.v("Zoodles", "kidsmodeActive:" + l_isActive);
				App l_app = App.instance();
				ChildLock l_childLock = l_app.childLock();

				if (l_isActive)
				{
					// startKidExperience();
					l_childLock.startWatchForPreemption();
					l_childLock.setupHomeReplacement(App.instance());
				}
				else
				{
					l_childLock.stopWatchForPreemption();
					l_childLock.removeHomeReplacement(App.instance());
					l_childLock.setInChildLock(false);
				}
			}
		});

		/*
		 * if (p_isActive) { App app = App.getInstance(this); DeviceInfo di =
		 * app.deviceInfo(); if (di.getSDKVersion() >= 21) { if (m_kidModeActive
		 * == false) { startLockTask(); m_kidModeActive = true; } } } else { App
		 * app = App.getInstance(this); DeviceInfo di = app.deviceInfo(); if
		 * (di.getSDKVersion() >= 21) { if (m_kidModeActive) { stopLockTask();
		 * m_kidModeActive = false; } } }
		 */
	}

	@SuppressLint("NewApi")
	public void lockTask()
	{
		startLockTask();
		m_kidModeActive = true;
	}

	public void updateLockTaskOnResume()
	{
		try
		{
			// Log.v("Zoodles", "try updatelocktaskonresume");
			if (m_kidModeActive && m_isLockPaused)
			{
				m_tryRelock = true;
				Log.v("Zoodles", "try updatelocktaskonresume");
				startLockTask();
				m_isLockPaused = false;
				m_tryRelock = false;
			}
		}
		catch (Exception e)
		{
			Log.v("Zoodles", "Exception thrown by startLockTask");

			ActivityManager activityManager = (ActivityManager) getApplicationContext().getSystemService(ACTIVITY_SERVICE);
			List<RunningTaskInfo> l_tasks = activityManager.getRunningTasks(10);

			for (int i = 0; i < l_tasks.size(); ++i)
			{
				RunningTaskInfo l_task = l_tasks.get(i);
				Log.v("ZoodlesToptask", " " + l_task.topActivity);
			}
		}
	}

	@SuppressLint("NewApi")
	public void unlockTask()
	{
		m_kidModeActive = false;
		stopLockTask();
	}

	public boolean _hasHomeButton()
	{
		App l_app = App.instance();
		ChildLock l_childLock = l_app.childLock();
		return l_childLock.isDefaultHomeLauncher(this);
	}

	public boolean _hasAvailableHomeButton()
	{
		App l_app = App.instance();
		ChildLock l_childLock = l_app.childLock();
		boolean l_hasAvailableHomeButton = !l_childLock.hasDifferentDefault(this);
		return l_hasAvailableHomeButton;
	}

	// Returns whether the application is finished with a home request.
	public boolean _hasRequestFinished()
	{
		return m_requestFinished;
	}

	// Use this when the home application is unset.
	// This will send a home intent to prompt the user to pick a home.
	public void _requestHomeButton()
	{
		m_requestFinished = false;
		m_requestIntentSent = false;

		runSafelyOnUiThread(new Runnable()
		{
			@Override
			public void run()
			{
				App l_app = App.instance();
				ChildLock l_childLock = l_app.childLock();
				Activity l_activity = UnityPlayer.currentActivity;
				l_childLock.setupHomeReplacement(l_app);
				l_childLock.setInChildLock(false);
				App.instance().childLock().sendHomeIntent(l_activity, 0);
				m_requestIntentSent = true;
			}
		});
	}
	
	public boolean _incomingCallsEnabled()
	{
		App l_app = App.instance();
		Preferences l_preferences = l_app.preferences();
		boolean l_incomingCallsEnabled = l_preferences.incomingCallsEnabled();
		return l_incomingCallsEnabled;
	}
	
	public int _exitAction()
	{
		App l_app = App.instance();
		Preferences l_preferences = l_app.preferences();
		int l_exitAction = l_preferences.exitAction();
		return l_exitAction;
	}

	/*
	 * Call this function if the home button is set to another application. This
	 * function calls into the settings screen where the home button can be
	 * cleared
	 */
	public void _clearHomeButton()
	{
		m_requestFinished = false;
		m_requestIntentSent = false;

		runSafelyOnUiThread(new Runnable()
		{
			@Override
			public void run()
			{
				App app = App.instance();
				DeviceInfo di = app.deviceInfo();
				ChildLock cl = app.childLock();

				String packageName = null;

				// Determine who is the owner of the default home button
				// assignment
				List<ResolveInfo> responders = cl.getAllHomeButtonApps(app);
				packageName = responders.get(0).activityInfo.packageName;

				// Build an Intent to launch the system settings screen.
				Intent i = null;

				if (di.getSDKVersion() > 8)
				{
					// Gingerbread or higher
					// https://groups.google.com/group/android-developers/browse_thread/thread/ce50a8716cbec32?pli=1
					// i = new Intent(
					// Settings.ACTION_APPLICATION_DETAILS_SETTINGS,
					// Uri.parse("package:" + packageName));

					// Kidmode 2.0 -- sometimes more than home launcher is
					// available and sometimes the first responder isn't the
					// default, so resolve different
					Intent intent = new Intent(Intent.ACTION_MAIN);
					intent.addCategory(Intent.CATEGORY_HOME);
					ResolveInfo resolveInfo = getPackageManager().resolveActivity(intent, PackageManager.MATCH_DEFAULT_ONLY);
					String currentHomePackage = resolveInfo.activityInfo.packageName;

					i = new Intent(Settings.ACTION_APPLICATION_DETAILS_SETTINGS, Uri.parse("package:" + currentHomePackage));
				}
				else
				{
					// Froyo and lower
					// http://comments.gmane.org/gmane.comp.handhelds.android.devel/101147
					i = new Intent(Intent.ACTION_VIEW);
					i.setClassName("com.android.settings", "com.android.settings.InstalledAppDetails");
					i.putExtra("pkg", packageName); // Froyo
					i.putExtra("com.android.settings.ApplicationPkgName", packageName); // 2.1
				}

				cl.setInChildLock(false);

				// Launch the intent. Stay on the activity stack.
				startActivity(i);

				m_requestIntentSent = true;
			}
		});
	}

	/*
	 * function to externalize toasts to Unity for localization.
	 */
	public void _makeToast(String p_toastText)
	{
		final String l_toastText = p_toastText;
		runSafelyOnUiThread(new Runnable()
		{
			@Override
			public void run()
			{
				Toast t = Toast.makeText(App.instance(), l_toastText, Toast.LENGTH_LONG);
				t.show();
			}
		});
	}

	public String _getHomeName()
	{
		App app = App.instance();
		DeviceInfo di = app.deviceInfo();
		ChildLock cl = app.childLock();

		List<ResolveInfo> responders = cl.getAllHomeButtonApps(app);
		String packageName = responders.get(0).activityInfo.packageName;

		return packageName;
	}

	private void callIntent(Intent p_intent)
	{
		this.startActivity(p_intent);
	}

	private boolean isCallable(Intent intent)
	{
		List<ResolveInfo> list = getPackageManager().queryIntentActivities(intent, PackageManager.MATCH_DEFAULT_ONLY);
		return list.size() > 0;
	}

	public boolean isPackageExisted(String targetPackage)
	{
		List<ApplicationInfo> packages;
		PackageManager pm;
		pm = getPackageManager();
		packages = pm.getInstalledApplications(0);
		for (ApplicationInfo packageInfo : packages)
		{
			// Log.v("Zoodles", "package name:" + packageInfo.packageName);
			if (packageInfo.packageName.equals(targetPackage))
			{
				Log.v("Zoodles", "targetPackage found:" + targetPackage);
				return true;
			}
		}
		Log.v("Zoodles", "targetPackage not found:" + targetPackage);
		return false;
	}

	/*
	 * Methods to remove key press
	 */

	/**
	 * Suppress the back button, with a hint message to child/parent. Also trap
	 * the volume keys to enforce max volume.
	 */
	@Override
	public boolean onKeyDown(int keyCode, KeyEvent event)
	{
		if (keyCode == KeyEvent.KEYCODE_BACK)
		{
			return true;
		}
		if (isHardKey(keyCode))
		{
			// For all other hard keys.
			event.startTracking();
			return true;
		}

		return super.onKeyDown(keyCode, event);
	}

	/**
	 * Returns TRUE if the keypress is one of the hardware buttons on device
	 * (Search, Home, Back, Menu, and others).
	 * 
	 * @param keyCode
	 * @return
	 */
	protected boolean isHardKey(int keyCode)
	{
		return (keyCode == KeyEvent.KEYCODE_SEARCH || keyCode == KeyEvent.KEYCODE_HOME || keyCode == KeyEvent.KEYCODE_BACK || keyCode == KeyEvent.KEYCODE_CALL || // The
																																									// HTC
																																									// Hero
																																									// has
																																									// these
																																									// two
																																									// buttons
																																									// -
				keyCode == KeyEvent.KEYCODE_ENDCALL || // but Android OS does
														// not permit blocking
														// END CALL button
				keyCode == KeyEvent.KEYCODE_CAMERA || keyCode == KeyEvent.KEYCODE_ENVELOPE || // is
																								// this
																								// a
																								// dedicated
																								// email
																								// button?
		(keyCode == KeyEvent.KEYCODE_MENU));
	}

	@Override
	public boolean onKeyUp(int keyCode, KeyEvent event)
	{

		if (isHardKey(keyCode) && event.isTracking() && !event.isCanceled())
		{
			return true;
		}

		return super.onKeyUp(keyCode, event);
	}

	/**
	 * Suppress long click on any of the buttons
	 */
	public boolean onKeyLongPress(int keyCode, KeyEvent event)
	{
		if (isHardKey(keyCode))
		{
			// a long press of the hard key.
			// do our work, returning true to consume it. by
			// returning true, the framework knows an action has
			// been performed on the long press, so will set the
			// canceled flag for the following up event.
			Log.d("ZOODLES_LOG", "On key long press for keycode, event: " + keyCode + " " + event.toString());
			return true;
		}

		return super.onKeyLongPress(keyCode, event);
	}

	/**
	 * Suppress the search button
	 */
	@Override
	public boolean onSearchRequested()
	{
		return false;
	}

	@Override
	protected void onUserLeaveHint()
	{
		super.onUserLeaveHint();
		Log.d("ZOODLES_LOG", "onUserLeaveHint");
		m_exitHint = true;
	}

	protected boolean startKidExperience()
	{
		App app = App.instance();
		DeviceInfo di = app.deviceInfo();
		ChildLock childLock = App.instance().childLock();

		// If not support CL, ignore the CL flow, and advance to Kid's
		// experience
		if (!di.supportsChildLock())
		{
			// launchKidExperience();
			return true;
		}

		boolean notDefaultHomeLauncher = !childLock.isDefaultHomeLauncher(app);

		// Child lock will be erased when user upgrade before JB, so ask user to
		// reset it.
		if (notDefaultHomeLauncher)
		{
			Log.d("Zoodles", "Doesn't have CL, try to turn it on");
			resetChildLock();
			return false;
		}

		// Attempt to lock the home button
		childLock.setupHomeReplacement(app);
		if (childLock.isDefault(app))
		{
			// launchKidExperience();
		}
		else
		{
			resetChildLock();
			return false;
		}

		return true;
	}

	private void resetChildLock()
	{
		// int flag = 0;
		//
		// // Could not lock the home button for unknown reason. Ask the user to
		// reset it.
		// int childLockPrompt = ChildLockFragment.PROMPT_DEFAULT;
		//
		// // If we owned Home button last time we were running, pass in custom
		// title + message
		// if ( App.instance().sessionHandler().wasChildLockLastSession() ) {
		// childLockPrompt = ChildLockFragment.PROMPT_UPGRADE;
		// flag |= ChildLockFragment.FLAG_IGNORE_DONE_PAGE;
		// ZLog.d(TAG, "Flag to ignore done page "+ flag );
		// }
		//
		// IntroChildLockActivity.launchForResult( this, childLockPrompt, flag
		// );
	}

	protected void checkForPreemption()
	{
		App app = App.instance();
		ChildLock cl = app.childLock();

		if (isScreenOn() && !isScreenLocked() && cl.isPreempted())
		{
			if ( /* prefs.childLockEnabled() && */cl.inChildLock())
			{
				// Break infinite relaunch loops. If we are at top of activity
				// stack, we
				// pre-empted ourself. So don't relaunch.
				//
				// This check becomes necessary under ICS. Even though we listen
				// to screen
				// on/screen off broadcast notifications to temporarily pause
				// the activity
				// relauncher, there are race conditions that can be invoked by
				// very quickly
				// turning the screen off and back on. Under ICS, the user
				// actions happen
				// before all the lifecycle events can be processed by the app,
				// and the
				// relauncher is fooled into a state where it still wants to
				// relaunch.
				// This code is a last-line defense against that situation.

				ActivityManager activityManager = (ActivityManager) getApplicationContext().getSystemService(ACTIVITY_SERVICE);
				List<RunningTaskInfo> runningTasks = activityManager.getRunningTasks(1);
				if (runningTasks != null && runningTasks.size() > 0)
				{
					ActivityManager.RunningTaskInfo topTask = runningTasks.get(0);
					String topPackageName = topTask.topActivity.getPackageName();
					Log.v("Zoodles", "running top package:" + topPackageName);
					if (topPackageName != null)
					{
						if (isKidmodeTopTask() && m_exitHint == false)
						{
							Log.d("Zoodles", "busting an infinite relaunch loop.");
							return;
						}

						int l_whiteListCount = m_whiteList.size();
						for (int i = 0; i < l_whiteListCount; ++i)
						{
							String l_whiteListPackage = m_whiteList.get(i);
							if (topPackageName.startsWith(l_whiteListPackage))
							{
								Log.d("Zoodles", "Whitelisted package detected preempted this app.");
								return;
							}
						}
					}
				}

				// Yes, we legitimately have been pre-empted. So assert
				// ourselves
				// back to the top of the system task stack.
				Log.v("Zoodles", "relaunch");
				relaunchActivity();
			}
			else
			{
				// End any game play session that might be in progress.
				Log.d("Zoodles", "checkForPreemption ending game session");

				// End any game session and log it.
				// The child can get right back in from the multitasking/history
				// menu.
				// onResume in TabBarBaseActivity will start a session to
				// replace this one.
				// if ( app.sessionHandler().hasPlaySession() ) {
				// app.dataBroker().endPlaySession( this, null );
				// }
				// UploadService.launch(App.instance());
			}
		}

		m_exitHint = false;
	}

	public boolean isKidmodeTopTask()
	{
		ActivityManager activityManager = (ActivityManager) getApplicationContext().getSystemService(ACTIVITY_SERVICE);
		List<ActivityManager.RunningAppProcessInfo> processes = activityManager.getRunningAppProcesses();

		for (ActivityManager.RunningAppProcessInfo process : processes)
		{
			if (process.processName.contains("com.zoodles.kidmode"))
			{
				// Log.v("Zoodles", "Process importance:" + process.importance +
				// " " + process.IMPORTANCE_FOREGROUND + " " +
				// process.IMPORTANCE_VISIBLE + " " +
				// process.IMPORTANCE_BACKGROUND);
				if (process.importance == process.IMPORTANCE_FOREGROUND)
				{
					return true;
				}
			}
		}
		return false;
	}

	public void relaunchActivity()
	{
		AlarmManager alm = (AlarmManager) this.getSystemService(Context.ALARM_SERVICE);
		alm.cancel(PendingIntent.getActivity(this, 0, new Intent(this, this.getClass()), 0));

		// Set exact forces more accurate timing, but is only available in
		// kitkat and above, so use set when setExact is unavailable.
		App app = App.instance();
		DeviceInfo di = app.deviceInfo();
		int l_sdkVersion = di.getSDKVersion();
		if (l_sdkVersion >= 19)
		{
			alm.setExact(AlarmManager.ELAPSED_REALTIME, 100, PendingIntent.getActivity(this, 0, new Intent(this, this.getClass()), 0));
		}
		else
		{
			alm.set(AlarmManager.ELAPSED_REALTIME, 100, PendingIntent.getActivity(this, 0, new Intent(this, this.getClass()), 0));
		}
	}

	@SuppressWarnings("deprecation")
	public boolean isScreenOn()
	{
		PowerManager pwr = (PowerManager) getSystemService(Context.POWER_SERVICE);
		return pwr.isScreenOn();
	}

	protected boolean isScreenLocked()
	{
		return App.instance().screenReceiver().screenLocked();
	}

	/**
	 * Return TRUE is the app is installed on the device.
	 * 
	 * @param pkg
	 * @return
	 */
	public boolean isThisAppInstalledOnTheDevice(String pkg)
	{
		PackageManager mPackageManager = getPackageManager();
		try
		{
			mPackageManager.getPackageInfo(pkg, 0);
		}
		catch (PackageManager.NameNotFoundException e)
		{
			return false;
		}
		return true;
	}

	/**
	 * Get the list of installed apps and optionally exclude Kid Mode and/or the
	 * list of app. For the list of other apps, see {@link #EXCLUSIVE_APPS}. <br>
	 * 
	 * <b>The returned list is sorted alphabetically.</b>
	 * 
	 * @param excludeKidMode
	 * @param excludeOthers
	 *            see {@link #EXCLUSIVE_APPS}
	 * @return
	 */
	@SuppressWarnings("unchecked")
	public void getInstalledApps()
	{
		PackageManager mPackageManager = getPackageManager();

		try
		{
			// Get All installed Apps
			Intent mainIntent = new Intent(Intent.ACTION_MAIN, null);
			mainIntent.addCategory(Intent.CATEGORY_LAUNCHER);
			List<ResolveInfo> appList = mPackageManager.queryIntentActivities(mainIntent, 0);
			for (ResolveInfo info : appList)
			{
				Log.v("Zoodles", "PNM:" + info.activityInfo.processName + " PKGNM:" + info.activityInfo.packageName + " activity:" + getActivity(info.activityInfo.packageName));

			}
		}
		catch (Exception e)
		{
			// In case of large list of apps installed on device, it might cause
			// PackageManager to died due to
			// android.os.TransactionTooLargeException.
			// Ignore this, and return an empty list.
		}
	}

	public String getActivity(String p_package)
	{
		PackageManager mPackageManager = getPackageManager();

		Intent mainIntent = new Intent();
		mainIntent.setAction(Intent.ACTION_MAIN);
		mainIntent.addCategory(Intent.CATEGORY_LAUNCHER);
		mainIntent.setPackage(p_package);

		List<ResolveInfo> activityInfo = mPackageManager.queryIntentActivities(mainIntent, 0);

		return activityInfo.get(0).activityInfo.name;
	}

	public void startApp(String p_packageName, String p_activityName)
	{
		if (m_kidModeActive)
		{
			// stopLockTask();
			m_isLockPaused = true;
		}

		Intent playgroundIntent = new Intent().setClass(this, this.getClass());
		playgroundIntent.putExtra(IntentConstants.EXTRA_NATIVE_APP, true);
		playgroundIntent.addFlags(Intent.FLAG_ACTIVITY_CLEAR_TOP);
		playgroundIntent.addFlags(Intent.FLAG_ACTIVITY_EXCLUDE_FROM_RECENTS);
		PendingIntent pi = PendingIntent.getActivity(this.getApplicationContext(), 0, playgroundIntent, 0);
		NativeAppService.startApp(this, 0, p_packageName, p_activityName, pi, null);
	}

	public void startApp(String p_packageName)
	{
		PackageManager l_packageManager = this.getPackageManager();
		Intent l_launchIntent = l_packageManager.getLaunchIntentForPackage(p_packageName);
		String l_activityName = l_launchIntent.getComponent().getClassName();
		startApp(p_packageName, l_activityName);
	}

	public void startTempleRun()
	{
		startTempleRunHelper("com.imangi.templerun2", "com.prime31.UnityPlayerProxyActivity");
	}

	public void startTempleRunHelper(String p_packageName, String p_activityName)
	{
		if (m_kidModeActive)
		{
			// stopLockTask();
			m_isLockPaused = true;
		}

		Intent playgroundIntent = new Intent().setClass(this, this.getClass());
		playgroundIntent.putExtra(IntentConstants.EXTRA_NATIVE_APP, true);
		playgroundIntent.addFlags(Intent.FLAG_ACTIVITY_CLEAR_TOP);
		PendingIntent pi = PendingIntent.getActivity(this.getApplicationContext(), 0, playgroundIntent, 0);

		NativeAppService.startApp(this, 0, p_packageName, p_activityName, pi, null);

		// ZoodlesDispatcher.main = this;
		//
		// Intent l_intent = new Intent();
		// l_intent.setClass(this, ZoodlesDispatcher.class);
		// l_intent.putExtra(ZoodlesDispatcher.DISPATCHER_PACKAGE,
		// p_packageName);
		// l_intent.putExtra(ZoodlesDispatcher.DISPATCHER_ACTIVITY,
		// p_activityName);
		// l_intent.addFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
		// l_intent.addFlags(Intent.FLAG_ACTIVITY_CLEAR_TASK);
		// l_intent.addFlags(Intent.FLAG_ACTIVITY_EXCLUDE_FROM_RECENTS);
		// l_intent.addFlags(Intent.FLAG_ACTIVITY_MULTIPLE_TASK);
		// startActivity(l_intent);
	}

	public void killTempleRun()
	{
		ActivityManager activityManager = (ActivityManager) getApplicationContext().getSystemService(ACTIVITY_SERVICE);
		int processId = findNativeAppProcessId("com.imangi.templerun2");
		if (processId > 0)
		{
			Log.v("Zoodles", "Killing process ID for package: " + processId + " " + "com.imangi.templerun2");
			activityManager.killBackgroundProcesses("com.imangi.templerun2");
			Process.sendSignal(processId, android.os.Process.SIGNAL_KILL);
		}

	}

	public void bringTempleRunToFront()
	{
		Intent i = new Intent();
		i.setComponent(new ComponentName("com.imangi.templerun2", "com.prime31.UnityPlayerProxyActivity"));
		startActivity(i);
	}

	public void listRunningApps()
	{
		ActivityManager activityManager = (ActivityManager) getApplicationContext().getSystemService(ACTIVITY_SERVICE);
		List<ActivityManager.RunningAppProcessInfo> processes = activityManager.getRunningAppProcesses();

		for (ActivityManager.RunningAppProcessInfo process : processes)
		{
			Log.v("Zoodles", "process component Running:" + process.processName + " " + process.importance + " " + process.IMPORTANCE_FOREGROUND);
			String[] packages = process.pkgList;
			for (int i = 0; i < packages.length; i++)
			{
				Log.v("Zoodles", "Package Running:" + packages[i]);
			}
		}

		Log.v("Zoodles", "==========================");

		final List<ActivityManager.RunningTaskInfo> recentTasks = activityManager.getRunningTasks(100);
		for (ActivityManager.RunningTaskInfo task : recentTasks)
		{
			Log.v("Zoodles", "Activity Running:" + task.baseActivity);
		}

	}

	private int findNativeAppProcessId(String packageName)
	{
		if (packageName == null)
			return -1;

		ActivityManager activityManager = (ActivityManager) getApplicationContext().getSystemService(ACTIVITY_SERVICE);
		List<ActivityManager.RunningAppProcessInfo> processes = activityManager.getRunningAppProcesses();
		if (processes == null)
		{
			return -1;
		}

		for (ActivityManager.RunningAppProcessInfo process : processes)
		{
			String[] packages = process.pkgList;
			for (int i = 0; i < packages.length; i++)
			{
				if (packages[i].startsWith(packageName))
				{
					return process.pid;
				}
			}
		}

		Log.v("Zoodles", "Unable to find the app in question.");
		return -1;
	}

	public static boolean m_inChildLock = false;
	private boolean m_exitHint = false;
	private ScreenLock m_screenLock = null;
	private boolean m_requestFinished = false;
	private boolean m_requestIntentSent = false;
	private boolean m_isInForeground = true;
	private ArrayList<String> m_whiteList;
}