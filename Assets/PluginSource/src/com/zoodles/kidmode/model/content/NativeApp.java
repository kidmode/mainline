package com.zoodles.kidmode.model.content;

import java.io.Serializable;
import java.util.Comparator;

import android.os.Parcelable;

/**
 * Base Model object for a Promoted Application
 */
public abstract class NativeApp implements Parcelable {
	
	/**
	 * Simple utility class for sorting NativeApps by Name
	 *
	 */
	@SuppressWarnings("serial")
	public static class NameComparator implements Comparator<NativeApp>, Serializable {

		@Override
		public int compare( NativeApp app1, NativeApp app2 ) {
			return app1.getName().compareToIgnoreCase( app2.getName() );
		}		
	}	
	
	/**
	 * Sort Review Apps by ( Edu * Fun ), if both ( Edu * Fun ) equal,then compare cost, if they are tied, 
	 * then compare edu score, if they are tied, then compare fun score
	 * 
	 */
	public static class ReviewComparator implements Comparator<NativeApp> {

		@Override
		public int compare( NativeApp lhs, NativeApp rhs ) {
			return 0;
/*			if( lhs.getReview() == null || rhs.getReview() == null ){
				return 0;
			}
			
			int lhsScore = lhs.getReview().getEdu() * lhs.getReview().getFun();
			int rhsScore = rhs.getReview().getEdu() * rhs.getReview().getFun();
			
			// Score is not tied, we can return right here.
			if( lhsScore != rhsScore ){
				return rhsScore - lhsScore;
			}
			
			// Compare cost, since 'F', 'P' and 'UN' order alphabetically, we can compare character for the cost
			int cost = lhs.getReview().getCost().code().compareTo( rhs.getReview().getCost().code() );
			if( cost != 0 ){
				return cost;
			}
			
			// Compare edu score
			int eduScore = rhs.getReview().getEdu() - lhs.getReview().getEdu();
			
			if( eduScore != 0 ){
				return eduScore;
			}
			
			// Compare fun score
			return rhs.getReview().getFun() - lhs.getReview().getFun();
*/			
		}

		
	}
	//////////////////////////////////////////////////////////
	// Members
	//////////////////////////////////////////////////////////	

	/**
	 * Name of the application to be promoted
	 */
	private String mName;	
	
	/**
	 * Application's package name used for linking to the Android Market
	 */
	private String mPackage;	
	
	/**
	 * Every App can have a Zoodles Review
	 */
//	private AppReview mReview;
	
	//////////////////////////////////////////////////////////
	// Constructors
	//////////////////////////////////////////////////////////
	
	public NativeApp() { }
	public NativeApp( String name, String pkg ) {
		setName( name );
		setPackage( pkg );
	}
	
	//////////////////////////////////////////////////////////
	// Name
	//////////////////////////////////////////////////////////

	/**
	 * @return - name of the application
	 */
	public String getName() 						{ return mName; }
	public void   setName( String name )			{ mName = name == null ? "" : name; }
	
	//////////////////////////////////////////////////////////
	// Package Name
	//////////////////////////////////////////////////////////
	
	/**
	 * @return - java package name of the application used for linking into the Market
	 */
	public String getPackage() 				{ return mPackage; }
	public void   setPackage( String pkg )	{ mPackage = pkg == null ? "" : pkg;  }
	
	//////////////////////////////////////////////////////////
	// Review
	//////////////////////////////////////////////////////////
	
//	public AppReview getReview()				{ return mReview; }
//	public void setReview( AppReview r )		{ mReview = r;    }
	
	/**
	 * need to override equals() for easy removal from Lists/Collections and
	 * what not.
	 */
	@Override
	public boolean equals(Object o) {
		if( o == this ) 					{ return true; }
		if( !( o instanceof NativeApp ) ) 	{ return false; }
		
		NativeApp app = ( NativeApp ) o;
		boolean equals = app.getPackage().equals( getPackage() );
		equals &= app.getName().equals( getName() );
		return equals;
	}
	
	@Override
	public int hashCode() {
		return ( getName() + getPackage() ).hashCode(); 
	}
	
	@Override
	public String toString() {
		StringBuilder sb = new StringBuilder();
		sb.append( "name: ").append( getName() );
		sb.append( ", package: ").append( getPackage() );
		return sb.toString();
	}
}

