/*
 * File:			IGAFAnimationObject.cs
 * Version:			3.7.1
 * Last changed:	2014/7/25 13:33
 * Author:			Nikolay_Nikitin
 * Copyright:		© Catalyst Apps
 * Project:			UnityVS.UnityProject.CSharp
 */
using System;

public interface IGAFAnimationObject
{
	bool visible
	{
		get;
		set;
	}

	uint objectID
	{
		get;
	}

	void onCreate(GAFObjectData _Object, string _Name = null/*, GAFAnimationGroup _Group = null*/);

	void init(GAFMovieClip _Player);

    void updateToState(GAFObjectStateData _State, bool _Refresh);

}

