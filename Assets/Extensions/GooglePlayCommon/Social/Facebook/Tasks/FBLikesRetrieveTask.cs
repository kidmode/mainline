using UnityEngine;
using UnionAssets.FLE;
using System.Collections;

public class FBLikesRetrieveTask : EventDispatcher {

	private string _userId;


	public static FBLikesRetrieveTask Create(){
		return new GameObject("FBLikesRetrieveTask").AddComponent<FBLikesRetrieveTask>();
	}

	public void LoadLikes(string userId) {
		_userId =  userId;
		FB.API("/" + userId + "/likes", Facebook.HttpMethod.GET, OnUserLikesResult);  
	}
	
	public void LoadLikes(string userId, string pageId ) {
		_userId =  userId;
		FB.API("/" + userId + "/likes/" + pageId, Facebook.HttpMethod.GET, OnUserLikesResult);  
	}



	public string userId {
		get {
			return _userId;
		}
	}

	private void OnUserLikesResult(FBResult result) {
		dispatch(BaseEvent.COMPLETE, result);
		Destroy(gameObject);
	}
}
