package com.zoodles.kidmode;

import android.annotation.SuppressLint;
import android.app.admin.DeviceAdminReceiver;
import android.app.admin.DevicePolicyManager;
import android.content.ComponentName;
import android.content.Context;
import android.content.Intent;
import android.util.Log;

public class ZoodlesDeviceOwner extends DeviceAdminReceiver 
{
    @SuppressLint("NewApi")
	@Override
    public void onEnabled(Context context, Intent intent) 
    {
        DevicePolicyManager manager = (DevicePolicyManager) context.getSystemService(Context.DEVICE_POLICY_SERVICE);
        ComponentName componentName = new ComponentName(context, ZoodlesDeviceOwner.class);
        manager.setLockTaskPackages(componentName, new String [] {"com.zoodles.kidmode"});
        Log.v("ZoodlesDeviceOwner", "Enabled and task lock set.");
    }
}
