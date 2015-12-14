using UnityEngine;
using System.Collections;

public class ZoodlesPointRequest : MonoBehaviour {

//	[SerializeField]
//	private Game game;
	private RequestQueue m_queue;
	private GameController m_gameController;

	void Start () 
	{
		GameObject gameLogic = GameObject.FindWithTag("GameController");
		Game game = gameLogic.GetComponent<Game>();
		m_gameController = game.gameController;

		m_queue = new RequestQueue();
		m_queue.add(new GetZPs(2, _requestComplete));
		m_queue.request(RequestType.RUSH);
	}

	private void _requestComplete(HttpsWWW p_response)
	{
		if (p_response.error != null) 
		{
			m_gameController.changeState(ZoodleState.SERVER_ERROR);
		}
		else
		{
			Debug.Log("  _requestComplete  =============   " );

			Hashtable l_jsonResponse = MiniJSON.MiniJSON.jsonDecode(p_response.text) as Hashtable;
			if (l_jsonResponse.ContainsKey("jsonResponse"))
			{
				Hashtable l_response = l_jsonResponse["jsonResponse"] as Hashtable;
				if (l_response.ContainsKey("response"))
				{
					Hashtable l_data = l_response["response"] as Hashtable;
					Kid l_kid = SessionHandler.getInstance().currentKid;
					l_kid.level = int.Parse(l_data["level"].ToString());
					l_kid.stars = int.Parse(l_data["zps"].ToString());
				}
			}
		}
	}
}
