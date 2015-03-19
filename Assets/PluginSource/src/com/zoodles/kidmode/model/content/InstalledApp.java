package com.zoodles.kidmode.model.content;

import android.graphics.drawable.Drawable;
import android.os.Parcel;
import android.os.Parcelable;

public class InstalledApp extends NativeApp implements Parcelable {

	// ////////////////////////////////////////////////////////
	// Private Data Members
	// ////////////////////////////////////////////////////////

	/**
	 * The application's logo loaded from the package manager
	 */
	private Drawable mImage;

	/**
	 * Flag to indicate whether or not this App has been approved by the parent
	 */
	private boolean mApproved;

	/**
	 * Since a package name can have multiple activities, we need to capture the
	 * name of actual activity to launch. This value is optional since some
	 * applications will have a target activity because a package name is enough
	 * for launching.
	 */
	private String mActivity;

	// ////////////////////////////////////////////////////////
	// Constructors
	// ////////////////////////////////////////////////////////

	/**
	 * Constructor.
	 * 
	 * @param name
	 *            App name "Angry Birds"
	 * @param pkg
	 *            Package Name "com.rovio.angrybirds"
	 * @param activity
	 *            Activity class name
	 */
	public InstalledApp(String name, String pkg, String activity) {
		super(name, pkg);
		setActivity(activity);
	}

	// ////////////////////////////////////////////////////////
	// Icon Image
	// ////////////////////////////////////////////////////////

	public Drawable getImage() {
		return mImage;
	}

	public void setImage(Drawable i) {
		mImage = i;
	}

	// ////////////////////////////////////////////////////////
	// Approved
	// ////////////////////////////////////////////////////////

	public boolean isApproved() {
		return mApproved;
	}

	public void setApproved(boolean b) {
		mApproved = b;
	}

	// ////////////////////////////////////////////////////////
	// Target Activity
	// ////////////////////////////////////////////////////////

	public String getActivity() {
		return mActivity;
	}

	public void setActivity(String a) {
		mActivity = a == null ? "" : a;
	}

	/**
	 * need to override equals() for easy removal from Lists/Collections and
	 * what not.
	 */
	@Override
	public boolean equals(Object o) {
		if (o == this) {
			return true;
		}
		if (!(o instanceof InstalledApp)) {
			return false;
		}

		InstalledApp app = (InstalledApp) o;
		boolean equals = app.getPackage().equals(getPackage());
		equals &= app.getName().equals(getName());
		equals &= app.getActivity().equals(getActivity());
		return equals;
	}

	@Override
	public int hashCode() {
		int hashCode = (getName() + getPackage() + getActivity()).hashCode();
		return hashCode;
	}

	@Override
	public String toString() {
		StringBuilder sb = new StringBuilder();
		sb.append("name: ").append(getName());
		sb.append(", package: ").append(getPackage());
		sb.append(", activity: ").append(getActivity());
		return sb.toString();
	}

	// ////////////////////////////////////////////////////////
	// Parcelable Interface
	// ////////////////////////////////////////////////////////

	@Override
	public int describeContents() {
		return 0;
	}

	@Override
	public void writeToParcel(Parcel dest, int flags) {
		// NB: mIconImage is not parceled, since it is a Drawable which is not
		// Parcelable.
		// This will no doubt bite someone in the ass one day. With luck, it
		// will not be me.

		dest.writeString(getName());
		dest.writeString(getPackage());
		dest.writeByte((byte) (isApproved() ? 0x1 : 0x0));
		dest.writeString(getActivity());
	}

	public static final Parcelable.Creator<InstalledApp> CREATOR = new Parcelable.Creator<InstalledApp>() {
		public InstalledApp createFromParcel(Parcel in) {
			return new InstalledApp(in);
		}

		public InstalledApp[] newArray(int size) {
			return new InstalledApp[size];
		}
	};

	public InstalledApp(Parcel in) {
		setName(in.readString());
		setPackage(in.readString());
		setApproved(in.readByte() == 0x1);
		setActivity(in.readString());
	}

}
