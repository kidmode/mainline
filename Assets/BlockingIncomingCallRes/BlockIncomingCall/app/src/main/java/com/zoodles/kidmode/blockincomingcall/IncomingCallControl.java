package com.zoodles.kidmode.blockincomingcall;

import android.content.IntentFilter;
import com.unity3d.player.UnityPlayer;

/**
 * Created by Joshua.Li on 2015/1/7.
 */
public class IncomingCallControl {
private static PhoneStatReceiver phoneStatReceiver = new PhoneStatReceiver();

    public void Register()
    {
        IntentFilter filter = new IntentFilter();
        filter.addAction("android.intent.action.PHONE_STATE");
        filter.setPriority(1000);
        UnityPlayer.currentActivity.registerReceiver(phoneStatReceiver, filter);
    }

    public void Unregister()
    {
        UnityPlayer.currentActivity.unregisterReceiver(phoneStatReceiver);
    }
}
