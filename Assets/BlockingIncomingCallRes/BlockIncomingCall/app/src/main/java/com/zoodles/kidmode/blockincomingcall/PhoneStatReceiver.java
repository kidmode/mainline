package com.zoodles.kidmode.blockincomingcall;

import android.app.Service;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.content.SharedPreferences;
import android.content.SharedPreferences.Editor;
import android.telephony.TelephonyManager;
import android.util.Log;

import com.android.internal.telephony.ITelephony;

import java.lang.reflect.Method;

public class PhoneStatReceiver extends BroadcastReceiver{

    String TAG = "tag";
    TelephonyManager telMgr;
    @Override
    public void onReceive(Context context, Intent intent) {
        telMgr = (TelephonyManager) context.getSystemService(Service.TELEPHONY_SERVICE);
        switch (telMgr.getCallState()) {
            case TelephonyManager.CALL_STATE_RINGING:
                String number = intent.getStringExtra(TelephonyManager.EXTRA_INCOMING_NUMBER);
                Log.v(TAG,"number:"+number);
                SharedPreferences phonenumSP = context.getSharedPreferences("in_phone_num", Context.MODE_PRIVATE);
                Editor editor = phonenumSP.edit();
                editor.putString(number,number);
                editor.commit();
                endCall();
                break;
            case TelephonyManager.CALL_STATE_OFFHOOK:
                break;
            case TelephonyManager.CALL_STATE_IDLE:
                break;
        }

    }
    /**
     * 挂断电话
     */
    private void endCall()
    {
        Class<TelephonyManager> c = TelephonyManager.class;
        try
        {
            Method getITelephonyMethod = c.getDeclaredMethod("getITelephony", (Class[]) null);
            getITelephonyMethod.setAccessible(true);
            ITelephony iTelephony = null;
            Log.e(TAG, "End call.");
            iTelephony = (ITelephony) getITelephonyMethod.invoke(telMgr, (Object[]) null);
            iTelephony.endCall();
        }
        catch (Exception e)
        {
            Log.e(TAG, "Fail to answer ring call.", e);
        }
    }
}
