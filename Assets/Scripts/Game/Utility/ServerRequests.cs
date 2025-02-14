using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

public enum RequestType
{
	RUSH,
	SEQUENCE
}

public class RequestQueue
{
	public delegate void RequestHandler(WWW p_response);

	public class Request
	{
		public Request(RequestHandler p_handler = null)
		{
			if (p_handler != null)
				handler += p_handler;
		}

		internal virtual void _request()
		{
			init();
			
			m_params["version"] = "v2";
			m_request = Server.request(m_call, m_params, m_method, _requestComplete);
		}

		public RequestHandler handler
		{
			get { return m_handler; }
			set { m_handler = value; }
		}

		protected virtual void init()
		{}

		protected RequestQueue context
		{
			get { return m_context; }
		}

		private void _requestComplete(WWW p_response)
		{
			if (m_handler != null)
			{
				try
				{
					m_handler(p_response);
				}
				finally
				{
					m_handler = null;
				}
			}
		}

		public void removeCallBack()
		{
			m_handler = null;
		}

		public void dispose ()
		{
			removeCallBack();

			if (null != m_request)
			{
				Server.removeCall(m_request);
			}
		}

		protected string m_call = "";
		protected Hashtable m_params = new Hashtable();
		protected CallMethod m_method = CallMethod.GET;
		private RequestHandler m_handler = null;
		internal RequestQueue m_context = null;
		private WebRequest m_request = null;
	}

	public RequestQueue()
	{}

	public void reset()
	{
		int l_numRequests = m_requests.Count;
		for (int i = 0; i < l_numRequests; ++i)
		{
			Request l_request = m_requests[i];
			l_request.dispose();
		}

		m_total = -1;
		is_used = false;
		m_requests.Clear();
	}

	public void dispose ()
	{
		int l_numRequests = m_requests.Count;
		for (int i = 0; i < l_numRequests; ++i)
		{
			Request l_request = m_requests[i];
			l_request.dispose();
		}
		
		m_total = -1;
		m_requests.Clear();
	}

	public void request(RequestType p_type = RequestType.RUSH)
	{
		m_total = m_requests.Count;
		m_type = p_type;
		if (RequestType.RUSH == p_type)
		{
			for (int i = 0; i < m_total; ++i)
			{
				m_requests[i].handler += _requestComplete;
				m_requests[i]._request();
			}
		}
		else
		{
			if(m_requests.Count > 0)
			{
				m_requests[0].handler += _requestComplete;
				m_requests[0]._request();
			}
		}
	}
	
	public void add(Request p_request)
	{
		m_requests.Add(p_request);
		p_request.m_context = this;
	}

	public void setVariable(string p_name, object p_value)
	{
		m_variables[p_name] = p_value;
	}

	public object getVariable(string p_name)
	{
		return m_variables[p_name];
	}

	public bool Completed()
	{
		if(m_total == 0)
		{
			is_used = true;
			m_total = -1;
			return true;
		}
		else if(m_total == -1)
		{
			return true;
		}
		else
		{
			return false;
		}
		//return (m_total == 0);
	}

	public bool isCompleted()
	{
		return Completed () && is_used;
	}

//	public void removeRequestCallBack()
//	{
//		for (int i = 0; i < m_total; ++i)
//		{
//			m_requests[i].removeCallBack();
//		}
//	}
	
	private void _requestComplete(WWW p_response)
	{
		--m_total;
		if (RequestType.SEQUENCE == m_type)
		{
			if(m_requests.Count != 0)
				m_requests.RemoveAt(0);
			if (m_total > 0 && (null == SessionHandler.getInstance().errorName || string.Empty.Equals(SessionHandler.getInstance().errorName)) )
			{
				m_requests[0].handler += _requestComplete;
				m_requests[0]._request();
			}
		}
	}

	private List<Request> m_requests = new List<Request>();
	private int m_total = -1;
	private bool is_used = false;
	private RequestType m_type = RequestType.RUSH;
	private Hashtable m_variables = new Hashtable();
}

// -----------------------------------------------------------
// Game Requests
// -----------------------------------------------------------

// Get client id
public class ClientIdRequest : RequestQueue.Request
{
	public ClientIdRequest(RequestQueue.RequestHandler p_handler = null) : base(p_handler)
	{}

	protected override void init()
	{
		//Server can't get 'screen_size' if I post this param and I don't konw why. So I send it on URL.
		m_call = ZoodlesConstants.REST_CLIENT_ID_URL+"?"+ZoodlesConstants.PARAM_SIZE+"="+Math.Round(Math.Sqrt(Screen.width * Screen.width + Screen.height * Screen.height) / Screen.dpi,1);
		m_params [ZoodlesConstants.PARAM_FLASH] = SessionHandler.getInstance().flashInstall;
		Debug.Log(Application.platform);
		if(Application.platform == RuntimePlatform.Android)
		{
			m_params [ZoodlesConstants.PARAM_FAMILY] = "android";
		}
		else if (Application.platform == RuntimePlatform.IPhonePlayer || 
		         Application.platform == RuntimePlatform.OSXEditor)
		{
			m_params [ZoodlesConstants.PARAM_FAMILY] = "apple";
		}
		else if (Application.platform == RuntimePlatform.WindowsEditor)
		{
			m_params [ZoodlesConstants.PARAM_FAMILY] = "windows";
		}
		else if (Application.platform == RuntimePlatform.WindowsWebPlayer)
		{
			//TODO
			m_params [ZoodlesConstants.PARAM_FAMILY] = "chrome"; 
		}

		m_params [ZoodlesConstants.PARAM_SCREEN_WIDTH] = Screen.width;
		m_params [ZoodlesConstants.PARAM_SCREEN_HEIGHT] = Screen.height;
		m_params [ZoodlesConstants.PARAM_DENSITY] = (int)Screen.dpi;
		m_params [ZoodlesConstants.PARAM_CHANNEL] = ZoodlesConstants.GOOGLE;
		m_params [ZoodlesConstants.PARAM_FIXED] = "True";

		#if UNITY_ANDROID && !UNITY_EDITOR
		AndroidJavaObject l_DI = new AndroidJavaObject ( "com.zoodles.ellipsiskids.features.DeviceInfo" );
		m_params [ZoodlesConstants.PARAM_OS_VERSION] = l_DI.Call<string>("getRelease");
		m_params [ZoodlesConstants.PARAM_BRAND] = l_DI.Call<string>("getBrand");
		// Sean: vzw
		//honda: this is for other devices except verizon devices
		m_params [ZoodlesConstants.PARAM_MANUFACTURER] = l_DI.Call<string>("getManufacturer");
		//honda: this is only for verizon device
//		m_params [ZoodlesConstants.PARAM_MANUFACTURER] = "Quanta";
		// end vzw
		m_params [ZoodlesConstants.PARAM_DEVICE] = l_DI.Call<string>("getDevice");
		m_params [ZoodlesConstants.PARAM_MODEL] = l_DI.Call<string>("getModel");

		SessionHandler.getInstance().deviceName = l_DI.Call<string>("getManufacturer");			
		#else

		string l_os = SystemInfo.operatingSystem;
		string[] l_osPara = l_os.Split (ZoodlesConstants.BLANK.ToCharArray());
		if(l_osPara.Length >= 3)
		{
			m_params [ZoodlesConstants.PARAM_OS_VERSION] = l_osPara[2];
		}
		else
		{
			m_params [ZoodlesConstants.PARAM_OS_VERSION] = l_os;
		}
		
		string l_brand = SystemInfo.deviceModel;
		string[] l_brandPara = l_brand.Split (ZoodlesConstants.BLANK.ToCharArray());
		if(l_osPara.Length >= 2)
		{
			m_params [ZoodlesConstants.PARAM_BRAND] = l_brandPara[0];
			m_params [ZoodlesConstants.PARAM_MANUFACTURER] = l_brandPara[0];
		}
		else
		{
			m_params [ZoodlesConstants.PARAM_BRAND] = l_brand;
			m_params [ZoodlesConstants.PARAM_MANUFACTURER] = l_brand;
		}
		m_params [ZoodlesConstants.PARAM_DEVICE] = SystemInfo.deviceName;
		m_params [ZoodlesConstants.PARAM_MODEL] = SystemInfo.deviceModel;

		//honda: add these to test create premium screen on unity editor
//		m_params [ZoodlesConstants.PARAM_MANUFACTURER] = "Quanta";
//		m_params [ZoodlesConstants.PARAM_MODEL] = "QTAQZ3KID";
		//end
		#endif

		m_method = CallMethod.POST;
	}
}

// Sign up
public class SignUpRequest : RequestQueue.Request
{
	public SignUpRequest(string p_name, string p_password, RequestQueue.RequestHandler p_handler = null) : base(p_handler)
	{
		m_name = p_name;
		m_password = p_password;
	}

	protected override void init()
	{
		m_call = ZoodlesConstants.REST_USER_URL;
		m_params[ZoodlesConstants.PARAM_CLIENT_ID] = (int)SessionHandler.getInstance().clientId;
		m_params[ZoodlesConstants.PARAM_EMAIL] = m_name;
		m_params[ZoodlesConstants.PARAM_PASSWORD] = m_password;
		if(!string.Empty.Equals(SessionHandler.getInstance().creditCardNum))
		{
			m_params[ZoodlesConstants.CREDIT_CARD_NUMBER] = SessionHandler.getInstance().creditCardNum;
			m_params[ZoodlesConstants.CREDIT_CARD_TYPE] = CreditCardHelper.parseType(m_params[ZoodlesConstants.CREDIT_CARD_NUMBER].ToString());
			m_params[ZoodlesConstants.CREDIT_CARD_YEAR] = SessionHandler.getInstance().cardYear;
			m_params[ZoodlesConstants.CREDIT_CARD_MONTH] = SessionHandler.getInstance().cardMonth;
		}

		m_method = CallMethod.POST;
	}

	private string m_name;
	private string m_password;
}

// Login
public class LoginRequest : RequestQueue.Request
{
	public LoginRequest(string p_name, string p_password, RequestQueue.RequestHandler p_handler = null) : base(p_handler)
	{
		m_name = p_name;
		m_password = p_password;
		
	}

	protected override void init()
	{
		m_call = ZoodlesConstants.REST_NEW_LOGIN_URL;
		m_params[ZoodlesConstants.PARAM_CLIENT_ID] = (int)SessionHandler.getInstance().clientId;
		m_params[ZoodlesConstants.PARAM_LOGIN] = m_name;
		m_params[ZoodlesConstants.PARAM_PASSWORD] = m_password;
		m_method = CallMethod.POST;
	}

	private string m_name;
	private string m_password;
}

// Get kid list
public class GetKidListRequest : RequestQueue.Request
{
	public GetKidListRequest(RequestQueue.RequestHandler p_handler = null) : base(p_handler)
	{
	}

	protected override void init()
	{
		m_call = ZoodlesConstants.REST_KIDS_URL;
		m_params[ZoodlesConstants.PARAM_TOKEN] = SessionHandler.getInstance().token.getSecret();
		m_method = CallMethod.GET;
	}
}

public class GetKidRequest : RequestQueue.Request
{
	public GetKidRequest(int p_kidId, RequestQueue.RequestHandler p_handler = null) : base(p_handler)
	{
		m_kidId = p_kidId;
	}
	
	protected override void init()
	{
		m_call = ZoodlesConstants.REST_KID_URL;
		m_params[ZoodlesConstants.PARAM_TOKEN] = SessionHandler.getInstance().token.getSecret();
		m_params[ZoodlesConstants.PARAM_KID_ID] = m_kidId.ToString();
		m_method = CallMethod.GET;
	}

	private int m_kidId = -1;
}

// Get user setting
public class GetUserSettingRequest : RequestQueue.Request
{
	public GetUserSettingRequest(RequestQueue.RequestHandler p_handler = null) : base(p_handler)
	{
		handler += _requestComplete;
	}

	protected override void init()
	{
		m_call = "/api/user_settings/show_settings";
		m_params[ZoodlesConstants.PARAM_TOKEN] = SessionHandler.getInstance().token.getSecret();
		m_method = CallMethod.GET;
	}
	
	private void _requestComplete(WWW p_response)
	{
		if(null == p_response.error)
		{
			Hashtable l_data = MiniJSON.MiniJSON.jsonDecode(p_response.text) as Hashtable;
			l_data = (l_data["jsonResponse"] as Hashtable)["response"] as Hashtable;
			
			SessionHandler l_session = SessionHandler.getInstance();
			if (l_data.ContainsKey ("child_lock"))
			{
				l_session.childLockSwitch = (bool)l_data["child_lock"];
			}
			if (l_data.ContainsKey ("incoming_calls"))
			{
				l_session.allowCall = (bool)l_data["incoming_calls"];

				//honda: vzw version comment out incoming call due to no use
//				#if UNITY_ANDROID && !UNITY_EDITOR
//				if( false == l_session.allowCall )
//					IncomingCallControl.StartBlock();
//				#endif
			}
			if (l_data.ContainsKey ("today_tips"))
				l_session.tip = (bool)l_data["today_tips"];
			if (l_data.ContainsKey ("push_free_weekly_apps"))
				l_session.freeWeeklyApp = (bool)l_data["push_free_weekly_apps"];
			if (l_data.ContainsKey ("push_new_apps_added"))
				l_session.newAddApp = (bool)l_data["push_new_apps_added"];
			if (l_data.ContainsKey ("push_smart_selection"))
				l_session.smartSelect = (bool)l_data["push_smart_selection"];
			if (l_data.ContainsKey ("music_volume"))
			{
				l_session.musicVolum = int.Parse(l_data["music_volume"].ToString());
				SoundManager.getInstance().musicVolume = (float)l_session.musicVolum/100;
			}
			if (l_data.ContainsKey ("effects_volume"))
			{
				l_session.effectsVolum = int.Parse(l_data["effects_volume"].ToString());
				SoundManager.getInstance().effectVolume = (float)l_session.effectsVolum/100;
			}
			if (l_data.ContainsKey ("master_volume"))
			{
				l_session.masterVolum = int.Parse(l_data["master_volume"].ToString());
				SoundManager.getInstance().masterVolume = (float)l_session.masterVolum/100;
			}
			if (l_data.ContainsKey ("enable_lock_pin"))
			{
				l_session.verifyBirth = !(bool)l_data["enable_lock_pin"];
			}
			if (l_data.ContainsKey ("lock_pin") && l_data["lock_pin"] != null)
			{
				l_session.childLockPassword = l_data["lock_pin"].ToString();
			}
			SessionHandler.getInstance ().initSettingCache ();
			Debug.Log("GetUserSetting SessionHandler master volume: " + SessionHandler.getInstance().masterVolum);
			Debug.Log("GetUserSetting SessionHandler music volume: " + SessionHandler.getInstance().musicVolum);
			Debug.Log("GetUserSetting SessionHandler effects volume: " + SessionHandler.getInstance().effectsVolum);
			Debug.Log("GetUserSetting SoundManager master volume: " + SoundManager.getInstance().masterVolume);
			Debug.Log("GetUserSetting SoundManager music volume: " + SoundManager.getInstance().musicVolume);
			Debug.Log("GetUserSetting SoundManager effects volume: " + SoundManager.getInstance().effectVolume);
			Debug.Log("GetUserSetting set volume data from session handler to setting cache");
		}
	}
}

// Get level zps info
public class GetLevelsInfoRequest : RequestQueue.Request
{
	public GetLevelsInfoRequest(RequestQueue.RequestHandler p_handler = null) : base(p_handler)
	{
		handler += _requestComplete;
	}

	protected override void init()
	{
		m_call = "/api/configuration/get_level_zps_info";
		m_params[ZoodlesConstants.PARAM_TOKEN] = SessionHandler.getInstance().token.getSecret();
		m_method = CallMethod.GET;
	}
	
	private void _requestComplete(WWW p_response)
	{
		LocalSetting.find("ServerSetting").setString(ZoodlesConstants.ZPS_LEVEL, p_response.text);
	}
}

// Get experience points info
public class GetExperiencePointsInfoRequest : RequestQueue.Request
{
	public GetExperiencePointsInfoRequest(RequestQueue.RequestHandler p_handler = null) : base(p_handler)
	{
		handler += _requestComplete;
	}
	
	protected override void init()
	{
		m_call = "/api/configuration/get_experience_points_info";
		m_params[ZoodlesConstants.PARAM_TOKEN] = SessionHandler.getInstance().token.getSecret();
		m_method = CallMethod.GET;
	}
	
	private void _requestComplete(WWW p_response)
	{
		LocalSetting.find("ServerSetting").setString(ZoodlesConstants.EXPERIENCE_POINTS, p_response.text);
	}
}

// Get categories info
public class GetCategoriesInfoRequest : RequestQueue.Request
{
	public GetCategoriesInfoRequest(RequestQueue.RequestHandler p_handler = null) : base(p_handler)
	{
		handler += _requestComplete;
	}
	
	protected override void init()
	{
		m_call = "/api/configuration/get_categories_info";
		m_params[ZoodlesConstants.PARAM_TOKEN] = SessionHandler.getInstance().token.getSecret();
		m_method = CallMethod.GET;
	}
	
	private void _requestComplete(WWW p_response)
	{
		LocalSetting.find("ServerSetting").setString(ZoodlesConstants.CATEGORIES, p_response.text);
	}
}

// Get tags info
public class GetTagsInfoRequest : RequestQueue.Request
{
	public GetTagsInfoRequest(RequestQueue.RequestHandler p_handler = null) : base(p_handler)
	{
		handler += _requestComplete;
	}
	
	protected override void init()
	{
		m_call = "/api/configuration/get_tags_info";
		m_params[ZoodlesConstants.PARAM_TOKEN] = SessionHandler.getInstance().token.getSecret();
		m_method = CallMethod.GET;
	}
	
	private void _requestComplete(WWW p_response)
	{
		LocalSetting.find("ServerSetting").setString(ZoodlesConstants.TAGS, p_response.text);
	}
}

// Get subjects info
public class GetSubjectsInfoRequest : RequestQueue.Request
{
	public GetSubjectsInfoRequest(RequestQueue.RequestHandler p_handler = null) : base(p_handler)
	{
		handler += _requestComplete;
	}
	
	protected override void init()
	{
		m_call = "/api/configuration/get_subjects_info";
		m_params[ZoodlesConstants.PARAM_TOKEN] = SessionHandler.getInstance().token.getSecret();
		m_method = CallMethod.GET;
	}
	
	private void _requestComplete(WWW p_response)
	{
		LocalSetting.find("ServerSetting").setString(ZoodlesConstants.SUBJECTS, p_response.text);
	}
}

// Get plan details
public class GetPlanDetailsRequest : RequestQueue.Request
{
	public GetPlanDetailsRequest(RequestQueue.RequestHandler p_handler = null) : base(p_handler)
	{}

	protected override void init()
	{
		m_call = "/api/user/plan_details";
		m_params[ZoodlesConstants.PARAM_TOKEN] = SessionHandler.getInstance().token.getSecret();
		m_method = CallMethod.GET;
	}
}

// Try free trial
public class TryTrialRequest : RequestQueue.Request
{
	public TryTrialRequest(RequestQueue.RequestHandler p_handler = null) : base(p_handler)
	{
		handler += _requestComplete;
	}
	
	protected override void init()
	{
		m_call = ZoodlesConstants.REST_TRY_TRIAL;
		m_params[ZoodlesConstants.PARAM_TOKEN] = SessionHandler.getInstance().token.getSecret();
		m_method = CallMethod.POST;
	}
	
	private void _requestComplete(WWW p_response)
	{
		if(null != p_response.error)
		{
			return ;
		}
		else
		{
			Hashtable l_data = MiniJSON.MiniJSON.jsonDecode(p_response.text) as Hashtable;
			if(l_data.ContainsKey("error") && (bool)l_data["error"] == false)
			{
				SessionHandler.getInstance().token.setTry(true);
				SessionHandler.getInstance().token.setCurrent(true);
			}
		}
	}
}

// Set pin
public class SetPinRequest : RequestQueue.Request
{
	public SetPinRequest(int p_pin, RequestQueue.RequestHandler p_handler = null) : base(p_handler)
	{
		m_pin = p_pin;
		handler += _requestComplete;
	}

	protected override void init()
	{
		m_call = "/api/pin/set_pin";
		m_params[ZoodlesConstants.PARAM_TOKEN] = SessionHandler.getInstance().token.getSecret();
		m_params[ZoodlesConstants.PARAM_PIN_CODE] = m_pin;
		m_method = CallMethod.POST;
	}

	private void _requestComplete(WWW p_response)
	{
		SessionHandler.getInstance().pin = m_pin;
	}

	private int m_pin;
}

// Verify pin
public class VerifyPinRequest : RequestQueue.Request
{
	public VerifyPinRequest(int p_pin, RequestQueue.RequestHandler p_handler = null) : base(p_handler)
	{
		m_pin = p_pin;
		handler += _requestComplete;
	}

	protected override void init()
	{
		m_call = "/api/pin/pin_matching";
		m_params[ZoodlesConstants.PARAM_TOKEN] = SessionHandler.getInstance().token.getSecret();
		m_params[ZoodlesConstants.PARAM_PIN_CODE] = m_pin;
		m_method = CallMethod.POST;
	}

	private void _requestComplete(WWW p_response)
	{
	}

	private int m_pin;
}

// create a child
public class CreateChildRequest : RequestQueue.Request
{
	public CreateChildRequest(string p_name, string p_birthday, string p_imageVeraible, RequestQueue.RequestHandler p_handler = null) : base(p_handler)
	{
		m_name = p_name;
		m_birthday = p_birthday;
		m_imageVariable = p_imageVeraible;
		handler += _requestComplete;
	}

	protected override void init()
	{
		m_call = "/api/kids";
		WWW l_www = context.getVariable(m_imageVariable) as WWW;
		m_params[ZoodlesConstants.PARAM_NAME] = m_name;
		m_params[ZoodlesConstants.PARAM_BIRTHDAY] = m_birthday;
		m_params[ZoodlesConstants.PARAM_TOKEN] = SessionHandler.getInstance().token.getSecret();
		m_params[ZoodlesConstants.PARAM_PICTURE] = l_www.bytes;
		m_method = CallMethod.POST;
	}

	private void _requestComplete(WWW p_response)
	{
		Hashtable l_data = MiniJSON.MiniJSON.jsonDecode(p_response.text) as Hashtable;
		Kid l_kid = new Kid(l_data);
		if(null == SessionHandler.getInstance().selectAvatar || string.Empty.Equals(SessionHandler.getInstance().selectAvatar))
			SessionHandler.getInstance().selectAvatar = "icon_avatar_gen";
		l_kid.kid_photo = Resources.Load("GUI/2048/common/avatars/" + SessionHandler.getInstance().selectAvatar) as Texture2D;
		//honda
		l_kid.saveKidPhotoLocal();
		//honda: request time limits info
		l_kid.requestTimeLimits();
		//end

		if (null == SessionHandler.getInstance ().kidList)
			SessionHandler.getInstance ().kidList = new List<Kid> ();

		SessionHandler.getInstance().inputedChildName = string.Empty;
		SessionHandler.getInstance().inputedbirthday = string.Empty;
		SessionHandler.getInstance().selectAvatar = null;
		SessionHandler.getInstance().kidList.Add(l_kid);
		SessionHandler.getInstance().getSingleKidApplist(l_kid);

		//cynthia
		ArrayList l_list = new ArrayList();
		foreach (Kid k in SessionHandler.getInstance().kidList) {
			l_list.Add(k.toHashTable());
		}
		String encodedString = MiniJSON.MiniJSON.jsonEncode(l_list);
		SessionHandler.SaveKidList(encodedString);

	}
	
	private string m_name;
	private string m_birthday;
	private string m_imageVariable;
}

// Request an image
public class ImageRequest : RequestQueue.Request
{
	public ImageRequest(string p_variable, string p_imagePath, RequestQueue.RequestHandler p_handler = null) : base(p_handler)
	{
		m_variable = p_variable;
		m_call = p_imagePath;
		handler += _requestComplete;
	}

	private void _requestComplete(WWW p_response)
	{
		if (p_response.error == null)
		{
			context.setVariable(m_variable, p_response);
		}
		else
		{
			Debug.Log(p_response.error);
		}
	}

	private string m_variable;
}

// Request an app icon
public class IconRequest : RequestQueue.Request
{
	public IconRequest(App p_app, UIElement p_element = null, RequestQueue.RequestHandler p_handler = null) : base(p_handler)
	{
		m_app = p_app;
		m_element = p_element;
		if(null != m_app.iconUrl)
			m_call = m_app.iconUrl;
		if(null == p_handler)
			handler += _requestComplete;
	}
	
	private void _requestComplete(WWW p_response)
	{
		if(null != p_response.error)
		{
			_Debug.log(p_response);
		}
		else
		{
			m_app.iconDownload = true;
			m_app.icon = p_response.texture;
			if(null != m_element)
			{
				UIImage l_image = m_element.getView("appImage") as UIImage;

				if( null == l_image )
				{
					return;
				}

				l_image.setTexture(p_response.texture);
			}
		}
	}

	private App m_app;
	private UIElement m_element;

}

// Request an drawing icon
public class DrawingRequest : RequestQueue.Request
{
	public DrawingRequest(Drawing p_drawing, UIElement p_element, RequestQueue.RequestHandler p_handler = null) : base(p_handler)
	{
		m_drawing = p_drawing;
		m_element = p_element;
		if(null != m_drawing.largeUrl)
			m_call = m_drawing.largeUrl;
		
		handler += _requestComplete;
	}
	
	private void _requestComplete(WWW p_response)
	{
		if(null != p_response.error)
		{
			_Debug.log(p_response);
		}
		else
		{
			m_drawing.largeIcon = p_response.texture;
			if(null != m_element)
			{
				UIImage l_image = m_element.getView("artImage") as UIImage;

				if( null == l_image )
				{
					return;
				}

				l_image.setTexture(p_response.texture);
			}
		}
	}

	private UIElement m_element;
	private Drawing m_drawing;
}

// Request an book icon
public class BookIconRequest : RequestQueue.Request
{
	public BookIconRequest(Book p_book, UIElement p_element = null, RequestQueue.RequestHandler p_handler = null) : base(p_handler)
	{
		m_book = p_book;
		m_element = p_element;
		if(null != m_book.coverUrl)
			m_call = m_book.coverUrl;
		
		handler += _requestComplete;
	}
	
	private void _requestComplete(WWW p_response)
	{
		if(null != p_response.error)
		{
			_Debug.log(p_response);
		}
		else
		{
			m_book.icon = p_response.texture;
			if(null != m_element)
			{
				UIImage l_image = m_element.getView("bookImage") as UIImage;

				if( null == l_image )
				{
					return;
				}

				l_image.setTexture(p_response.texture);
			}
		}
	}
	
	private Book m_book;
	private UIElement m_element;
	
}

// Request an audio
public class AudioRequest : RequestQueue.Request
{
	public AudioRequest( string p_audioPath, int p_bookId, int p_pageId, RequestQueue.RequestHandler p_handler = null) : base(p_handler)
	{
		m_call = p_audioPath;
		m_bookId = p_bookId;
		m_pageId = p_pageId;

		handler += _requestComplete;
	}

	private void _requestComplete( WWW p_response )
	{
		if (p_response.error != null)
			_Debug.log ( p_response.error );
		else
		{
			AudioClip l_clip = p_response.GetAudioClip( false, false, AudioType.WAV );
			
			AudioSave.Save( m_bookId + "//" + m_pageId, l_clip );
			
			UnityEngine.Object.Destroy(l_clip);
		}
	}

	private int m_bookId;
	private int m_pageId;
}

// Web content request
public class WebContentRequest : RequestQueue.Request
{
	public WebContentRequest(RequestQueue.RequestHandler p_handler = null) : base(p_handler)
	{}

	protected override void init()
	{
		Debug.Log ("spinner: " + SessionHandler.getInstance ().currentKid.id);
		m_call = "/api/kids" + ZoodlesConstants.SLASH + SessionHandler.getInstance().currentKid.id + ZoodlesConstants.REST_LINKS_LIST_URL_SUFFIX;
		m_params[ZoodlesConstants.PARAM_TOKEN] = SessionHandler.getInstance().token.getSecret();
		m_params[ZoodlesConstants.PARAM_CLIENT_ID] = SessionHandler.getInstance().clientId;
		#if UNITY_ANDROID && !UNITY_EDITOR
		AndroidJavaObject l_DI = new AndroidJavaObject ( "com.zoodles.ellipsiskids.features.DeviceInfo" );
		string l_locale = l_DI.Call<string>("getLanguage");
		m_params["locale"] = l_locale.Replace ("_","-");
		#endif
		m_method = CallMethod.GET;
	}
}

// Book list request
public class BookListRequest : RequestQueue.Request
{
	public BookListRequest(bool p_topRequest, RequestQueue.RequestHandler p_handler = null) : base(p_handler)
	{
		m_topRequest = p_topRequest;
	}

	protected override void init()
	{
//		m_call = ZoodlesConstants.REST_BOOKS_URL + "/books_for_kid"; 
		m_call = ZoodlesConstants.REST_BOOKS_URL + "/books_and_readings"; 
		m_params[ZoodlesConstants.PARAM_TOKEN] = SessionHandler.getInstance().token.getSecret();
		m_params[ZoodlesConstants.PARAM_CLIENT_ID] = SessionHandler.getInstance().clientId;	
		m_params[ZoodlesConstants.PARAM_KID_ID] = SessionHandler.getInstance().currentKid.id;

		switch( Application.systemLanguage )
		{
		case SystemLanguage.English :
			m_params["locale"] = "en";
			break;
		case SystemLanguage.French :
			m_params["locale"] = "fr";
			break;
		case SystemLanguage.German :
			m_params["locale"] = "de";
			break;
		case SystemLanguage.Spanish :
			m_params["locale"] = "es";
			break;
		case SystemLanguage.Korean :
			m_params["locale"] = "ko";
			break;
		case SystemLanguage.Chinese :
			#if UNITY_ANDROID && !UNITY_EDITOR
			AndroidJavaObject l_DI = new AndroidJavaObject ( "com.zoodles.ellipsiskids.features.DeviceInfo" );
			string l_locale = l_DI.Call<string>("getLanguage");
			
			switch ( l_locale )
			{
			case "zh_CN":
				m_params["locale"] = "zh-CN";
				break;
			case "zh_TW":
				m_params["locale"] = "zh-TW";
				break;
			default :
				m_params["locale"] = "zh";
				break;
			}
			#else
			m_params["locale"] = "zh";
			#endif
			break;
		case SystemLanguage.Japanese :
			m_params["locale"] = "ja";
			break;
		case SystemLanguage.Italian :
			m_params["locale"] = "it";
			break;
		case SystemLanguage.Dutch :
			m_params["locale"] = "nl";
			break;

		default :
			m_params["locale"] = "en";
			break;
		}

		if( m_topRequest )
		{
			m_params["top_request"] = "true";
		}
		else
		{
			m_params["top_request"] = "false";	
		}
		m_method = CallMethod.GET;
	}

	private bool m_topRequest = false;
}

// Drawing list request
public class DrawingListRequest : RequestQueue.Request
{
	public DrawingListRequest(RequestQueue.RequestHandler p_handler = null) : base(p_handler)
	{}

	protected override void init()
	{
		m_call = "/api/kids/" + SessionHandler.getInstance().currentKid.id + "/drawings";
		m_params[ZoodlesConstants.PARAM_TOKEN] = SessionHandler.getInstance().token.getSecret();
		m_method = CallMethod.GET;
	}
}

// Delete child request.
public class DeleteChildRequest : RequestQueue.Request
{
	public DeleteChildRequest(RequestQueue.RequestHandler p_handler = null) : base(p_handler)
	{}
	
	protected override void init()
	{
		m_call = ZoodlesConstants.DELETE_CHILD;
		m_params[ZoodlesConstants.PARAM_TOKEN] = SessionHandler.getInstance().token.getSecret();
		m_params[ZoodlesConstants.PARAM_KID_ID] = SessionHandler.getInstance().currentKid.id;
		m_method = CallMethod.GET;
	}

}

// New drawing request
public class NewDrawingRequest : RequestQueue.Request
{
	public NewDrawingRequest(byte[] p_file, RequestQueue.RequestHandler p_handler = null) : base(p_handler)
	{
		m_file = p_file;
//		handler += _requestComplete;
	}

	protected override void init()
	{
		m_call = "/api/kids/" + SessionHandler.getInstance().currentKid.id + "/drawings";
		m_params[ZoodlesConstants.PARAM_TOKEN] = SessionHandler.getInstance().token.getSecret();
		m_params["file"] = m_file;
		m_method = CallMethod.POST;
	}

//	private void _requestComplete(WWW p_response)
//	{
//		if (p_response.error == null)
//			_Debug.log("#####################: " + p_response.text);
//	}

	private byte[] m_file;
}

// Save drawing request
public class SaveDrawingRequest : RequestQueue.Request
{
	public SaveDrawingRequest(byte[] p_file, RequestQueue.RequestHandler p_handler = null) : base(p_handler)
	{
		m_file = p_file;
//		handler += _requestComplete;
	}
	
	protected override void init()
	{
		m_call = "/api/kids/" + SessionHandler.getInstance().currentKid.id + "/drawings/" + SessionHandler.getInstance().currentDrawing.id + "/update_drawing";
		m_params[ZoodlesConstants.PARAM_TOKEN] = SessionHandler.getInstance().token.getSecret();
		m_params["file"] = m_file;
		m_method = CallMethod.POST;
	}

//	private void _requestComplete(WWW p_response)
//	{
//		if (p_response.error == null)
//			_Debug.log(p_response.text);
//	}

	private byte[] m_file;
}

//added by joshua 11-28
public class GetLanguagesRequest : RequestQueue.Request
{
	public GetLanguagesRequest(RequestQueue.RequestHandler p_handler = null) : base(p_handler)
	{}
	
	protected override void init()
	{
		m_call = "/api/kids/" + SessionHandler.getInstance ().currentKid.id + ZoodlesConstants.REST_PROMOTE_LANGUAGES_SUFFIX;
		m_method = CallMethod.GET;
		m_params[ZoodlesConstants.PARAM_TOKEN] = SessionHandler.getInstance().token.getSecret();
	}
}

//added by joshua 11-28
public class GetTimeLimitsRequest : RequestQueue.Request
{
	public GetTimeLimitsRequest(RequestQueue.RequestHandler p_handler = null) : base(p_handler)
	{}
	
	protected override void init()
	{
		m_call = "/api/kids/" + SessionHandler.getInstance ().currentKid.id + ZoodlesConstants.REST_TIMER_INFO_SUFFIX;
		m_method = CallMethod.GET;
		m_params[ZoodlesConstants.PARAM_TOKEN] = SessionHandler.getInstance().token.getSecret();
	}
}

//honda: added
public class GetKidTimeLimitsRequest : RequestQueue.Request
{
	private int kid_id;

	public GetKidTimeLimitsRequest(int kidId, RequestQueue.RequestHandler p_handler = null) : base(p_handler)
	{
		kid_id = kidId;
	}
	
	protected override void init()
	{
		m_call = "/api/kids/" + kid_id + ZoodlesConstants.REST_TIMER_INFO_SUFFIX;
		m_method = CallMethod.GET;
		m_params[ZoodlesConstants.PARAM_TOKEN] = SessionHandler.getInstance().token.getSecret();
	}
}
//end

//added by joshua 11-28
public class SetSubjectsRequest : RequestQueue.Request
{
	public SetSubjectsRequest(Hashtable p_params, RequestQueue.RequestHandler p_handler = null) : base
		(p_handler)
	{
		m_params = p_params;
		handler += _requestComplete;
	}
	
	protected override void init()
	{
		m_call = "/api/kids/update_kid";
		m_method = CallMethod.POST;
		m_params [ZoodlesConstants.PARAM_TOKEN] 	= SessionHandler.getInstance ().token.getSecret();
		m_params [ZoodlesConstants.PARAM_KID_ID] 	= SessionHandler.getInstance ().currentKid.id;
	}
	
	private void _requestComplete(WWW p_response)
	{
		string l_string = "";
		
		l_string = UnicodeDecoder.Unicode(p_response.text);
		l_string = UnicodeDecoder.UnicodeToChinese(l_string);
		l_string = UnicodeDecoder.CoverHtmlLabel(l_string);

		Hashtable l_data = MiniJSON.MiniJSON.jsonDecode(l_string) as Hashtable;
		
		Kid l_kidData = new Kid( l_data );
		l_kidData.requestPhoto();
		
		for( int i = 0; i < SessionHandler.getInstance().kidList.Count; i++ )
		{
			if( SessionHandler.getInstance().kidList[i].id == l_kidData.id)
			{
				SessionHandler.getInstance().kidList[i].maxViolence = l_kidData.maxViolence;
				SessionHandler.getInstance().kidList[i].weightMath = l_kidData.weightMath;
				SessionHandler.getInstance().kidList[i].weightReading = l_kidData.weightReading;
				SessionHandler.getInstance().kidList[i].weightScience = l_kidData.weightScience;
				SessionHandler.getInstance().kidList[i].weightSocialStudies = l_kidData.weightSocialStudies;
				SessionHandler.getInstance().kidList[i].weightCognitiveDevelopment = l_kidData.weightCognitiveDevelopment;
				SessionHandler.getInstance().kidList[i].weightCreativeDevelopment = l_kidData.weightCreativeDevelopment;
				SessionHandler.getInstance().kidList[i].weightLifeSkills = l_kidData.weightLifeSkills;
			}
		}
	}
}

public class SetTimeLimitsRequest : RequestQueue.Request
{
	public SetTimeLimitsRequest(Hashtable p_params, RequestQueue.RequestHandler p_handler = null) : base(p_handler)
	{
		m_params = p_params;
	}
	
	protected override void init()
	{
		m_call = "/api/kids/" + SessionHandler.getInstance ().currentKid.id + ZoodlesConstants.REST_TIMER_SET_SUFFIX;
		m_method = CallMethod.POST;
	}
}


public class SetLanguagesRequest : RequestQueue.Request
{
	public SetLanguagesRequest(string p_param, RequestQueue.RequestHandler p_handler = null) : base(p_handler)
	{
		m_paramPath = p_param;
	}
	
	protected override void init()
	{
		m_call = "/api/kids/" + SessionHandler.getInstance().currentKid.id + ZoodlesConstants.REST_PROMOTE_LANGUAGES_UPDATE;
		m_call += m_paramPath;
		m_params[ZoodlesConstants.PARAM_TOKEN] = SessionHandler.getInstance().token.getSecret();
		m_method = CallMethod.POST;
	}

	private string m_paramPath;
}

public class GetTimeSpendRequest : RequestQueue.Request
{
	public GetTimeSpendRequest(RequestQueue.RequestHandler p_handler = null) : base(p_handler)
	{}
	
	protected override void init()
	{
		m_call = "/api/kids/" + SessionHandler.getInstance ().currentKid.id + ZoodlesConstants.REST_TIME_FOR_SUBJECTS_SUFFIX;
		m_method = CallMethod.GET;
		m_params[ZoodlesConstants.PARAM_TOKEN] = SessionHandler.getInstance().token.getSecret();
		m_params["period"] = 30;
	}
}

// get app list by page
public class GetAppByPageRequest : RequestQueue.Request
{
	public GetAppByPageRequest(int p_age, string p_channel,int p_page, RequestQueue.RequestHandler p_handler = null) : base(p_handler)
	{
		m_age = p_age;
		m_channel = p_channel;
		m_page = p_page;
	}
	
	protected override void init()
	{
		m_call = "/api/apps/recommended_apps";
		m_params[ZoodlesConstants.PARAM_TOKEN] = SessionHandler.getInstance().token.getSecret();
		m_params[ZoodlesConstants.PARAM_CLIENT_ID] = SessionHandler.getInstance().clientId;
		m_params[ZoodlesConstants.PARAM_KID_AGE] = m_age;
		m_params[ZoodlesConstants.PARAM_PAGE] = m_page;
		if(null != m_channel)
			m_params[ZoodlesConstants.PARAM_CHANNEL] = m_channel;
		m_method = CallMethod.GET;
	}

	
	private int m_age;
	private int m_page;
	private string m_channel;
}

//New get app list by page
public class NewGetAppByPageRequest : RequestQueue.Request
{
	public NewGetAppByPageRequest(Kid p_kid, string p_channel,int p_page, RequestQueue.RequestHandler p_handler = null) : base(p_handler)
	{
		m_kid = p_kid;
		m_channel = p_channel;
		m_page = p_page;
		handler += _requestComplete;
	}
	
	protected override void init()
	{
		m_call = ZoodlesConstants.REST_APP_RECOMMEND_NEW;
		m_params[ZoodlesConstants.PARAM_TOKEN] = SessionHandler.getInstance().token.getSecret();
		m_params[ZoodlesConstants.PARAM_CLIENT_ID] = SessionHandler.getInstance().clientId;
		m_params[ZoodlesConstants.PARAM_KID_AGE] = m_kid.age;
		m_params[ZoodlesConstants.PARAM_PAGE] = m_page;
		#if UNITY_ANDROID && !UNITY_EDITOR
		AndroidJavaObject l_DI = new AndroidJavaObject ( "com.zoodles.ellipsiskids.features.DeviceInfo" );
		string l_locale = l_DI.Call<string>("getLanguage");
		m_params["locale"] = l_locale.Replace ("_","-");
		#endif
		if(null != SessionHandler.getInstance().appOwn)
			m_params[ZoodlesConstants.PARAM_OWNED_REQUEST] = "false";
		else
			m_params[ZoodlesConstants.PARAM_OWNED_REQUEST] = "true";
		if(null != m_channel)
			m_params[ZoodlesConstants.PARAM_CHANNEL] = m_channel;
		m_method = CallMethod.GET;
	}

	private void _requestComplete(WWW p_response)
	{
		if(null == p_response.error)
		{
			string l_string = UnicodeDecoder.Unicode(p_response.text);
			l_string = UnicodeDecoder.UnicodeToChinese(l_string);
			Hashtable l_jsonResponse = MiniJSON.MiniJSON.jsonDecode(l_string) as Hashtable;
			if(l_jsonResponse.ContainsKey("jsonResponse"))
			{
				Hashtable l_response = l_jsonResponse["jsonResponse"] as Hashtable;
				if(l_response.ContainsKey("response"))
				{
					Hashtable l_result = l_response["response"] as Hashtable;
					if(l_result.ContainsKey("app_owned"))
					{
						SessionHandler.getInstance().appOwn = l_result["app_owned"] as Hashtable;
					}
					if(l_result.ContainsKey("apps"))
					{
						ArrayList l_data = l_result["apps"] as ArrayList;
						int l_dataCount = l_data.Count;
						Hashtable l_appOwn = SessionHandler.getInstance ().appOwn;
						if(null == m_kid.appList)
							m_kid.appList = new List<object>();
						for(int l_i = 0; l_i < l_dataCount; l_i++)
						{
							Hashtable l_table = l_data[l_i] as Hashtable;
							App l_app = new App(l_table);
							if(null != l_table)
							{
								l_app.own = l_appOwn.ContainsKey(l_app.id.ToString());
							}
							m_kid.appList.Add(l_app);
						}
						//SessionHandler.getInstance().getAppIconByKid(m_kid);
					}
				}
			}

		}
	}
	
	private string m_IsFirst;
	private Kid m_kid;
	private int m_page;
	private string m_channel;
}

// get app list
public class GetAppRequest : RequestQueue.Request
{
	public GetAppRequest(int p_age, string p_channel, RequestQueue.RequestHandler p_handler = null) : base(p_handler)
	{
		m_age = p_age;
		m_channel = p_channel;
		handler += _requestComplete;
	}
	
	protected override void init()
	{
		m_call = "/api/apps/recommended_apps";
		m_params[ZoodlesConstants.PARAM_TOKEN] = SessionHandler.getInstance().token.getSecret();
		m_params[ZoodlesConstants.PARAM_CLIENT_ID] = SessionHandler.getInstance().clientId;
		m_params[ZoodlesConstants.PARAM_KID_AGE] = m_age;
		#if UNITY_ANDROID && !UNITY_EDITOR
		AndroidJavaObject l_DI = new AndroidJavaObject ( "com.zoodles.ellipsiskids.features.DeviceInfo" );
		string l_locale = l_DI.Call<string>("getLanguage");
		m_params["locale"] = l_locale.Replace ("_","-");
		#endif
		if(null != m_channel)
			m_params[ZoodlesConstants.PARAM_CHANNEL] = m_channel;
		m_method = CallMethod.GET;
	}
	
	private void _requestComplete(WWW p_response)
	{
		string l_string = UnicodeDecoder.Unicode(p_response.text);
		l_string = UnicodeDecoder.UnicodeToChinese(l_string);
		ArrayList l_data = MiniJSON.MiniJSON.jsonDecode (l_string) as ArrayList;
		int l_dataCount = l_data.Count;
		List<App> l_list = new List<App> ();
		Hashtable l_appOwn = SessionHandler.getInstance ().appOwn;
		for(int l_i = 0; l_i < l_dataCount; l_i++)
		{
			Hashtable l_table = l_data[l_i] as Hashtable;
			App l_app = new App(l_table);
			if(null != l_table)
			{
				l_app.own = l_appOwn.ContainsKey(l_app.id.ToString());
			}
			l_list.Add(l_app);
		}
		//SessionHandler.getInstance ().appList = l_list;
	}
	
	private int m_age;
	private string m_channel;
}

// get book list
public class GetBookRequest : RequestQueue.Request
{
	public GetBookRequest(string p_pageRequest, RequestQueue.RequestHandler p_handler = null) : base(p_handler)
	{
		handler += _requestComplete;
		m_pageRequest = p_pageRequest;
	}
	
	protected override void init()
	{
		m_call = ZoodlesConstants.REST_BOOKS_URL + "/book_list";
		if(!(null == m_pageRequest || string.Empty.Equals(m_pageRequest)))
			m_params[ZoodlesConstants.PARAM_APP_PAGES] = m_pageRequest;
		#if UNITY_ANDROID && !UNITY_EDITOR
		AndroidJavaObject l_DI = new AndroidJavaObject ( "com.zoodles.ellipsiskids.features.DeviceInfo" );
		string l_locale = l_DI.Call<string>("getLanguage");
		m_params["locale"] = l_locale.Replace ("_","-");
		#endif
		m_params[ZoodlesConstants.PARAM_TOKEN] = SessionHandler.getInstance().token.getSecret();
		m_method = CallMethod.GET;
	}
	
	private void _requestComplete(WWW p_response)
	{
		if(null == p_response.error)
		{
			string l_string = "";
			
			l_string = UnicodeDecoder.Unicode(p_response.text);
			l_string = UnicodeDecoder.UnicodeToChinese(l_string);
			l_string = UnicodeDecoder.CoverHtmlLabel(l_string);
			Hashtable l_jsonResponse = MiniJSON.MiniJSON.jsonDecode(l_string) as Hashtable;
			if(l_jsonResponse.ContainsKey("jsonResponse"))
			{
				Hashtable l_response = l_jsonResponse["jsonResponse"] as Hashtable;
				if(l_response.ContainsKey("response"))
				{
					Hashtable l_bookData = l_response["response"] as Hashtable;
					ArrayList l_own = l_bookData["owned"] as ArrayList;
					ArrayList l_unown = l_bookData["unowned"] as ArrayList;
					int l_dataCount = l_own.Count;
					List<Book> l_list = new List<Book> ();
					for(int l_i = 0; l_i < l_dataCount; l_i++)
					{
						Hashtable l_table = l_own[l_i] as Hashtable;
						Book l_book = new Book(l_table);
						l_list.Add(l_book);
					}
					l_dataCount = l_unown.Count;
					for(int l_i = 0; l_i < l_dataCount; l_i++)
					{
						Hashtable l_table = l_unown[l_i] as Hashtable;
						Book l_book = new Book(l_table);
						l_list.Add(l_book);
					}
					SessionHandler.getInstance ().bookList = l_list;
				}
			}
			//SessionHandler.getInstance ().getBookIcon();
		}
	}

	private string m_pageRequest;
}

// get drawing list
public class GetDrawingRequest : RequestQueue.Request
{
	public GetDrawingRequest(RequestQueue.RequestHandler p_handler = null) : base(p_handler)
	{
		handler += _requestComplete;
	}
	
	protected override void init()
	{
		m_call = "/api/kids/" + SessionHandler.getInstance().currentKid.id + ZoodlesConstants.REST_DRAWINGS_SUFFIX;
		m_params[ZoodlesConstants.PARAM_TOKEN] = SessionHandler.getInstance().token.getSecret();
		m_method = CallMethod.GET;
	}
	
	private void _requestComplete(WWW p_response)
	{
		ArrayList l_data = MiniJSON.MiniJSON.jsonDecode (p_response.text) as ArrayList;
		int l_dataCount = l_data.Count;
		List<Drawing> l_list = new List<Drawing> ();
		for(int l_i = 0; l_i < l_dataCount; l_i++)
		{
			Hashtable l_table = l_data[l_i] as Hashtable;
			Drawing l_drawing = new Drawing(l_table);
			l_list.Add(l_drawing);
		}
		SessionHandler.getInstance ().drawingList = l_list;
	}
}

// buy book
public class BuyBookRequest : RequestQueue.Request
{
	public BuyBookRequest(Book p_book,UIButton p_button,UICanvas p_canvas, UIElement p_element = null,RequestQueue.RequestHandler p_handler = null) : base(p_handler)
	{
		handler += _requestComplete;
		m_book = p_book;
		m_button = p_button;
		m_element = p_element;
		m_confirmCanvas = p_canvas;
	}
	
	protected override void init()
	{
		m_call = ZoodlesConstants.REST_BUY_BOOK;
		m_params [ZoodlesConstants.PARAM_TOKEN] = SessionHandler.getInstance().token.getSecret();
		m_params [ZoodlesConstants.PARAM_BOOK_ID] = m_book.id;
		m_method = CallMethod.POST;
	}
	
	private void _requestComplete(WWW p_response)
	{
		UIElement l_element = m_button.parent.parent;
		if(null == p_response.error)
		{
			List<Kid> l_kid = SessionHandler.getInstance().kidList;
			foreach (Kid l_k in l_kid)
			{
				l_k.gems -= m_book.gems;
			}

			m_book.owned = true;
			setActive(l_element);

			if(null != m_element)
			{
				setActive(m_element);
			}

			if(null != m_confirmCanvas)
			{
				List<Vector3> l_pointListOut = new List<Vector3>();
				UIElement l_currentPanel = m_confirmCanvas.getView ("mainPanel");
				l_pointListOut.Add( l_currentPanel.transform.localPosition );
				l_pointListOut.Add( l_currentPanel.transform.localPosition - new Vector3( 0, 800, 0 ));
				l_currentPanel.tweener.addPositionTrack( l_pointListOut, 0f );
			}
		}
	}

	private void setActive(UIElement p_element)
	{
		UIImage l_lockImage = p_element.getView ("lockImage") as UIImage;
		UIButton l_buyButton = p_element.getView ("buyBookButton") as UIButton;
		UILabel l_unlockLabel = p_element.getView ("unlockText") as UILabel;
		
		l_lockImage.active = false;
		l_buyButton.active = false;
		l_unlockLabel.active = false;
	}

	private UICanvas m_confirmCanvas;
	private Book m_book;
	private UIButton m_button;
	private UIElement m_element;
}

public class GetStarChartListRequest : RequestQueue.Request
{
	public GetStarChartListRequest(int p_rootId, RequestQueue.RequestHandler p_handler = null) : base(p_handler)
	{
		m_rootId = p_rootId;
	}
	
	protected override void init()
	{
		m_call = "/api/kids/" + SessionHandler.getInstance().currentKid.id + "/overview/root_courses";
		m_method = CallMethod.GET;
		m_params[ZoodlesConstants.PARAM_TOKEN] = SessionHandler.getInstance().token.getSecret();
		m_params["subject_id"] = m_rootId;
	}

	int m_rootId;
}

public class GetStarChartRequest : RequestQueue.Request
{
	public GetStarChartRequest(int p_rootId, int p_page, RequestQueue.RequestHandler p_handler = null) : base(p_handler)
	{
		m_rootId = p_rootId;
		m_page = p_page;
	}
	
	protected override void init()
	{
		m_call = "/api/kids/" + SessionHandler.getInstance().currentKid.id + "/overview/page_courses_star";
		m_method = CallMethod.GET;
		m_params[ZoodlesConstants.PARAM_TOKEN] = SessionHandler.getInstance().token.getSecret();
		m_params["subject_id"] = m_rootId;
		m_params["page"] = m_page;
	}

	int m_rootId;
	int m_page;
}

public class SetAddAppRequest : RequestQueue.Request
{
	public SetAddAppRequest(string p_param, RequestQueue.RequestHandler p_handler = null) : base(p_handler)
	{
		m_paramJson = p_param;
	}
	
	protected override void init()
	{
		m_call = "/api/apps/create_app";
		m_params[ZoodlesConstants.PARAM_TOKEN] = SessionHandler.getInstance().token.getSecret();
		m_params [ZoodlesConstants.PARAM_APPS] = m_paramJson;
		m_method = CallMethod.POST;
	}
	
	private string m_paramJson;
}

//Get user's app own hashtable.
public class GetAppOwnRequest : RequestQueue.Request
{
	public GetAppOwnRequest(RequestQueue.RequestHandler p_handler = null) : base(p_handler)
	{
		handler += _requestComplete;
	}
	
	protected override void init()
	{
		m_call = "/api/apps/owned_apps";
		m_params[ZoodlesConstants.PARAM_TOKEN] = SessionHandler.getInstance().token.getSecret();
		m_method = CallMethod.GET;
	}

	private void _requestComplete(WWW p_response)
	{
		if(null == p_response.error)
		{
			Hashtable l_jsonResponse = MiniJSON.MiniJSON.jsonDecode(p_response.text) as Hashtable;
			if(l_jsonResponse.ContainsKey("jsonResponse"))
			{
				Hashtable l_response = l_jsonResponse["jsonResponse"] as Hashtable;
				if(l_response.ContainsKey("response"))
				{
					SessionHandler.getInstance().appOwn = l_response["response"] as Hashtable;
				}
			}
		}
	}
}

//Buy app.
public class BuyRecommendAppRequest : RequestQueue.Request
{
	public BuyRecommendAppRequest(App p_app,RequestQueue.RequestHandler p_handler = null) : base(p_handler)
	{
		m_app = p_app;
	}

	protected override void init()
	{
		m_call = "/api/apps/purchase_app";
		m_params[ZoodlesConstants.PARAM_TOKEN] = SessionHandler.getInstance().token.getSecret();
		m_params[ZoodlesConstants.NEW_PARAM_APP_ID] = m_app.id;
		m_method = CallMethod.POST;
	}

	private App m_app;
}

//New Buy app request.
public class BuyAppRequest : RequestQueue.Request
{
	public BuyAppRequest(int p_appId, RequestQueue.RequestHandler p_handler = null) : base(p_handler)
	{
		m_appId = p_appId;
	}
	
	protected override void init()
	{
		m_call = "/api/apps/purchase_app";
		m_params[ZoodlesConstants.PARAM_TOKEN] = SessionHandler.getInstance().token.getSecret();
		m_params[ZoodlesConstants.NEW_PARAM_APP_ID] = m_appId;
		m_method = CallMethod.POST;
	}

	private int m_appId = -1;
}

public class GetSiteListRequest : RequestQueue.Request
{
	public GetSiteListRequest(RequestQueue.RequestHandler p_handler = null) : base(p_handler)
	{}
	
	protected override void init()
	{
		m_call = "/api/kids/" + SessionHandler.getInstance ().currentKid.id + ZoodlesConstants.REST_SITES_SUFFIX;
		m_params[ZoodlesConstants.PARAM_TOKEN] = SessionHandler.getInstance().token.getSecret();
		m_method = CallMethod.GET;
	}
}

public class SetSiteListRequest : RequestQueue.Request
{
	public SetSiteListRequest( string p_param, RequestQueue.RequestHandler p_handler = null) : base(p_handler)
	{
		m_params [ZoodlesConstants.PARAM_BLOCKED_SITES] = p_param;
	}
	
	protected override void init()
	{
		m_call = "/api/kids/" + SessionHandler.getInstance ().currentKid.id + ZoodlesConstants.REST_SITES_SUFFIX;
		m_params[ZoodlesConstants.PARAM_TOKEN] = SessionHandler.getInstance().token.getSecret();
		m_method = CallMethod.POST;
	}
}

public class UpdatePhotoRequest : RequestQueue.Request
{
	public UpdatePhotoRequest(string p_imageVeraible, RequestQueue.RequestHandler p_handler = null) : base(p_handler)
	{
		m_imageVariable = p_imageVeraible;
	}
	
	protected override void init()
	{
		m_call = "/api/kids/update_photo";
		WWW l_www = context.getVariable(m_imageVariable) as WWW;
		m_params[ZoodlesConstants.PARAM_KID_ID] = SessionHandler.getInstance ().currentKid.id;
		m_params[ZoodlesConstants.PARAM_TOKEN] = SessionHandler.getInstance().token.getSecret();
		m_params[ZoodlesConstants.PARAM_PICTURE] = l_www.bytes;
		m_method = CallMethod.POST;
	}

	private string m_imageVariable;
}

//Get plan details
public class ShowPlanDetailsRequest : RequestQueue.Request
{
	public ShowPlanDetailsRequest(RequestQueue.RequestHandler p_handler = null) : base(p_handler)
	{
		
	}

	protected override void init()
	{
		m_call = ZoodlesConstants.SHOW_PLAN_DETAILS;
		m_params[ZoodlesConstants.PARAM_TOKEN] = SessionHandler.getInstance().token.getSecret();
		m_method = CallMethod.GET;
	}
}


//Cancel subcription.
public class CancelSubcriptionRequest : RequestQueue.Request
{
	public CancelSubcriptionRequest(string p_reason,string p_description,RequestQueue.RequestHandler p_handler = null) : base(p_handler)
	{
		
		m_reason = p_reason;
		m_description = p_description;
	}
	
	protected override void init()
	{
		m_call = ZoodlesConstants.REST_SUBSCRIPTION_CANCEL_NEW;
		m_params[ZoodlesConstants.PARAM_TOKEN] = SessionHandler.getInstance().token.getSecret();
		m_params ["reason"] = m_reason;
		m_params ["comment"] = m_description;
		m_method = CallMethod.GET;
	}

	private string m_reason;
	private string m_description;
}

public class CreateReadingsRequest : RequestQueue.Request
{
	public CreateReadingsRequest(RequestQueue.RequestHandler p_handler = null) : base(p_handler)
	{}
	
	protected override void init()
	{
		m_call = ZoodlesConstants.REST_READINGS + "/create_readings";
		m_params[ZoodlesConstants.PARAM_TOKEN] 		= SessionHandler.getInstance().token.getSecret();
		m_params [ZoodlesConstants.PARAM_BOOK_ID] 	= SessionHandler.getInstance ().currentBook.id;

		string l_kidIds = "";
		foreach( Kid l_kid in SessionHandler.getInstance ().recordKidList)
		{
			l_kidIds = l_kidIds + l_kid.id + ",";
		}

		l_kidIds = l_kidIds.TrimEnd (',');

		m_params [ZoodlesConstants.PARAM_KID_IDS] 	= l_kidIds;
		m_method = CallMethod.POST;
	}
}

public class GetReadingsRequest : RequestQueue.Request
{
	public GetReadingsRequest(RequestQueue.RequestHandler p_handler = null) : base(p_handler)
	{}
	
	protected override void init()
	{
		m_call = "/api/kids/" + SessionHandler.getInstance().currentKid.id + "/readings/kid_readings";
		m_params[ZoodlesConstants.PARAM_TOKEN] 		= SessionHandler.getInstance().token.getSecret();
		m_method = CallMethod.GET;
	}
}

public class GetZPs : RequestQueue.Request
{
	public GetZPs(int p_actionId, RequestQueue.RequestHandler p_handler = null) : base(p_handler)
	{
		m_actionId = p_actionId;
	}

	protected override void init()
	{
		m_call = "/api/kid_level/get_zps";
		m_params["id"] = m_actionId;
		m_params[ZoodlesConstants.PARAM_KID_ID] = SessionHandler.getInstance ().currentKid.id;
		m_params[ZoodlesConstants.PARAM_TOKEN] = SessionHandler.getInstance().token.getSecret();
		m_method = CallMethod.POST;
	}

	private int m_actionId;
}

//View Gems price.
public class ViewGemsRequest : RequestQueue.Request
{
	public ViewGemsRequest(RequestQueue.RequestHandler p_handler = null) : base(p_handler)
	{
		
	}
	
	protected override void init()
	{
		m_call = ZoodlesConstants.REST_VIEW_GEMS;
		m_params[ZoodlesConstants.PARAM_TOKEN] = SessionHandler.getInstance().token.getSecret();
		m_method = CallMethod.GET;
	}
}

//Cancel Lock Request
public class CancelLockRequest : RequestQueue.Request
{
	public CancelLockRequest(RequestQueue.RequestHandler p_handler = null) : base(p_handler)
	{
		
	}

	protected override void init()
	{
		m_call = ZoodlesConstants.REST_CANCEL_LOCK;
		m_params[ZoodlesConstants.PARAM_TOKEN] = SessionHandler.getInstance().token.getSecret();
		m_method = CallMethod.GET;
	}
}

//Enable Lock Request
public class EnableLockRequest : RequestQueue.Request
{
	public EnableLockRequest(string p_lockPin,RequestQueue.RequestHandler p_handler = null) : base(p_handler)
	{
		m_lockPin = p_lockPin;
		
	}
	
	protected override void init()
	{
		m_call = ZoodlesConstants.REST_ENABLE_LOCK;
		m_params[ZoodlesConstants.PARAM_TOKEN] = SessionHandler.getInstance().token.getSecret();
		if(null != m_lockPin)
			m_params[ZoodlesConstants.LOCK_PIN] = m_lockPin;
		m_method = CallMethod.POST;
	}

	private string m_lockPin;
}

//Update Setting Request
public class UpdateSettingRequest : RequestQueue.Request
{
	public UpdateSettingRequest(string p_childLock,RequestQueue.RequestHandler p_handler = null) : base(p_handler)
	{
		m_childLock = p_childLock;
		
	}
	
	protected override void init()
	{
		m_call = ZoodlesConstants.REST_UPDATE_SETTING;
		m_params[ZoodlesConstants.CHILD_LOCK] = m_childLock;
		m_params[ZoodlesConstants.PARAM_TOKEN] = SessionHandler.getInstance().token.getSecret();
		m_method = CallMethod.POST;
	}

	private string m_childLock;
}

//Update Notification Setting Request.
public class UpdateNotificateRequest : RequestQueue.Request
{
	public UpdateNotificateRequest(bool p_newAppAdded,bool p_smartSelectNotification,bool p_weeklyAppsNotification,RequestQueue.RequestHandler p_handler = null) : base(p_handler)
	{
		m_newAppAdded = p_newAppAdded;
		m_smartSelectNotification = p_smartSelectNotification;
		m_weeklyAppsNotification = p_weeklyAppsNotification;
		
	}
	
	protected override void init()
	{
		m_call = ZoodlesConstants.UPDATE_NOTIFICATION;
		setValue (ZoodlesConstants.PUSH_NEW_APPS_ADDED,m_newAppAdded);
		setValue (ZoodlesConstants.PUSH_SMART_SELECTION,m_smartSelectNotification);
		setValue (ZoodlesConstants.PUSH_FREE_WEEKLY_APPS,m_weeklyAppsNotification);
		m_params[ZoodlesConstants.PARAM_TOKEN] = SessionHandler.getInstance().token.getSecret();
		m_method = CallMethod.POST;
	}

	private void setValue(string p_fieldName ,bool p_condition)
	{
		if(p_condition)
		{
			m_params[p_fieldName] = "true";
		}
		else
		{
			m_params[p_fieldName] = "false";
		}
	}

	private bool m_newAppAdded;
	private bool m_smartSelectNotification;
	private bool m_weeklyAppsNotification;
}

//Update Device Option Request
public class UpdateDeviceOptionRequest : RequestQueue.Request
{
	public UpdateDeviceOptionRequest(string p_incomingCalls,string p_todayTips,int p_masterVolume,int p_musicVolume,int p_effectsVolume,RequestQueue.RequestHandler p_handler = null) : base(p_handler)
	{
		m_incomingCalls = p_incomingCalls;
		m_todayTips = p_todayTips;
		m_masterVolume = p_masterVolume;
		m_musicVolume = p_musicVolume;
		m_effectsVolume = p_effectsVolume;
		
	}
	
	protected override void init()
	{
		m_call = ZoodlesConstants.REST_UPDATE_SETTING;
		m_params[ZoodlesConstants.INCOMING_CALL] = m_incomingCalls;
		m_params[ZoodlesConstants.TODAY_TIPS] = m_todayTips;
		m_params[ZoodlesConstants.MASTER_VOLUME] = m_masterVolume;
		m_params[ZoodlesConstants.MUSIC_VOLUM] = m_musicVolume;
		m_params[ZoodlesConstants.EFFECTS_VOLUME] = m_effectsVolume;
		m_params[ZoodlesConstants.PARAM_TOKEN] = SessionHandler.getInstance().token.getSecret();
		m_method = CallMethod.POST;
	}
	
	private string m_incomingCalls;
	private string m_todayTips;
	private int m_masterVolume;
	private int m_musicVolume;
	private int m_effectsVolume;
}

//Update Setting Request
public class SendPinRequest : RequestQueue.Request
{
	public SendPinRequest(RequestQueue.RequestHandler p_handler = null) : base(p_handler)
	{}
	
	protected override void init()
	{
		m_call = ZoodlesConstants.SEND_PIN;
		m_params[ZoodlesConstants.PARAM_TOKEN] = SessionHandler.getInstance().token.getSecret();
		m_method = CallMethod.POST;
	}
}

//Get top recommend app request.
public class GetTopRecommandRequest : RequestQueue.Request
{
	public GetTopRecommandRequest(string p_channel,Kid p_kid, RequestQueue.RequestHandler p_handler = null) : base(p_handler)
	{
		m_channel = p_channel;
		m_kid = p_kid;
		if(null == p_handler)
			handler += _requestComplete;
	}
	
	protected override void init()
	{
		m_call = ZoodlesConstants.GET_TOP_RECOMMEND_APP;
		m_params[ZoodlesConstants.PARAM_TOKEN] = SessionHandler.getInstance().token.getSecret();
		m_params [ZoodlesConstants.PARAM_CLIENT_ID] = SessionHandler.getInstance().clientId;
		m_params[ZoodlesConstants.PARAM_KID_AGE] = m_kid.age;
		#if UNITY_ANDROID && !UNITY_EDITOR
		AndroidJavaObject l_DI = new AndroidJavaObject ( "com.zoodles.ellipsiskids.features.DeviceInfo" );
		string l_locale = l_DI.Call<string>("getLanguage");
		m_params["locale"] = l_locale.Replace ("_","-");
		#endif
		if(null != m_channel)
			m_params[ZoodlesConstants.PARAM_CHANNEL] = m_channel;
		m_method = CallMethod.GET;
	}

	private void _requestComplete(WWW p_response)
	{
		if(null == p_response.error && !"null".Equals(p_response.text))
		{
			string l_string = UnicodeDecoder.Unicode(p_response.text);
			l_string = UnicodeDecoder.UnicodeToChinese(l_string);
			Hashtable l_hashTable = MiniJSON.MiniJSON.jsonDecode(l_string) as Hashtable;
			App l_app = new App(l_hashTable);
			if(l_hashTable.ContainsKey("owned"))
			{
				l_app.own = (bool)l_hashTable["owned"];
			}
			m_kid.topRecommendedApp = l_app;
		}
	}
	private string m_channel;
	private Kid m_kid;
}


// create a child
public class EditChildRequest : RequestQueue.Request
{
	public EditChildRequest(string p_name, string p_birthday, RequestQueue.RequestHandler p_handler = null) : base(p_handler)
	{
		m_call = ZoodlesConstants.EDIT_CHILD;
		m_name = p_name;
		m_birthday = p_birthday;
		handler += _requestComplete;
	}
	
	protected override void init()
	{
		m_params[ZoodlesConstants.ID] = SessionHandler.getInstance ().currentKid.id;
		m_params[ZoodlesConstants.PARAM_NAME] = m_name;
		m_params[ZoodlesConstants.PARAM_BIRTHDAY] = m_birthday;
		m_params[ZoodlesConstants.PARAM_TOKEN] = SessionHandler.getInstance().token.getSecret();
		m_method = CallMethod.POST;
	}
	
	private void _requestComplete(WWW p_response)
	{
		string l_string = "";
		
		l_string = UnicodeDecoder.Unicode(p_response.text);
		l_string = UnicodeDecoder.UnicodeToChinese(l_string);
		l_string = UnicodeDecoder.CoverHtmlLabel(l_string);

		Hashtable l_data = MiniJSON.MiniJSON.jsonDecode(l_string) as Hashtable;
		Kid l_kid = new Kid(l_data);
		if (null == SessionHandler.getInstance ().selectAvatar || string.Empty.Equals (SessionHandler.getInstance ().selectAvatar))
			l_kid.kid_photo = SessionHandler.getInstance().currentKid.kid_photo;
		else
			l_kid.kid_photo = Resources.Load("GUI/2048/common/avatars/" + SessionHandler.getInstance().selectAvatar) as Texture2D;
		List<Kid> l_list = SessionHandler.getInstance ().kidList;
		if (null != l_list)
		{
			int l_count = l_list.Count;
			for(int l_i = 0; l_i < l_count; l_i++)
			{
				Kid l_currentKid = l_list[l_i] as Kid;
				if(l_currentKid.id == l_kid.id)
				{
					SessionHandler.getInstance().kidList[l_i].wholeName = l_kid.wholeName;
					SessionHandler.getInstance().kidList[l_i].name = l_kid.name;
					SessionHandler.getInstance().kidList[l_i].birthday = l_kid.birthday;
					SessionHandler.getInstance().kidList[l_i].kid_photo = l_kid.kid_photo;
					SessionHandler.getInstance().kidList[l_i].saveKidPhotoLocal();
					string birthday = l_kid.birthday;
					
					int l_age = 0;
					DateTime l_date = DateTime.Parse (birthday);
					l_age = DateTime.Now.Year - l_date.Year;
					DateTime l_now = DateTime.Now;
					
					if( l_now.Month < l_date.Month )
					{
						l_age--;
					}
					else if( l_now.Month == l_date.Month && l_now.Day < l_date.Day )
					{
						l_age--;
					}
					SessionHandler.getInstance().kidList[l_i].age = l_age;
					
					SessionHandler.getInstance ().currentKid = SessionHandler.getInstance().kidList[l_i];
					SessionHandler.getInstance ().getSingleKidApplist (SessionHandler.getInstance ().currentKid);
					break;
				}
			}
		}

		//honda: save latest kid rogile to local
		ArrayList arraylist = new ArrayList();
		foreach (Kid k in SessionHandler.getInstance().kidList) {
			arraylist.Add(k.toHashTable());
		}
		String encodedString = MiniJSON.MiniJSON.jsonEncode(arraylist);
		SessionHandler.SaveKidList(encodedString);
		//end
	
		SessionHandler.getInstance ().inputedChildName = string.Empty;
		SessionHandler.getInstance ().inputedbirthday = string.Empty;
		SessionHandler.getInstance ().selectAvatar = null;
	}

	private string m_name;
	private string m_birthday;
}

public class DeleteDrawingRequest : RequestQueue.Request
{
	public DeleteDrawingRequest(RequestQueue.RequestHandler p_handler = null) : base(p_handler)
	{}

	protected override void init()
	{
		m_call = "/api/kids/" + SessionHandler.getInstance ().currentKid.id + ZoodlesConstants.REST_DRAWINGS_SUFFIX 
			+ "/" + SessionHandler.getInstance ().currentDrawing.id + "/destroy_drawing";
		m_params[ZoodlesConstants.PARAM_TOKEN] = SessionHandler.getInstance().token.getSecret();
		m_method = CallMethod.GET;
	}
}

//Send tell friend email by server.
public class SendTellFriendRequest : RequestQueue.Request
{
	public SendTellFriendRequest(string p_from,string p_to, string p_option,RequestQueue.RequestHandler p_handler = null) : base(p_handler)
	{
		m_from = p_from;
		m_to = p_to;
		m_option = p_option;
	}
	
	protected override void init()
	{
		m_call = ZoodlesConstants.SEND_TELL_FRIEND_EMAIL;
		m_params[ZoodlesConstants.PARAM_TOKEN]      = SessionHandler.getInstance().token.getSecret();
		m_params[ZoodlesConstants.FROM] 	        = m_from;
		m_params[ZoodlesConstants.TO]               = m_to;
		m_params[ZoodlesConstants.OPTIONAL_MESSAGE] = m_option;
		m_method = CallMethod.POST;
	}

	private string m_from;
	private string m_to;
	private string m_option;
}

public class VisitLinkTrackRequest
{
	public VisitLinkTrackRequest()
	{}

	public void send(int p_linkId, int p_duration)
	{
		DateTime l_date = DateTime.Now;
		l_date = l_date.AddSeconds(-p_duration);
		string l_formatDate = l_date.ToString("yyyy-MM-ddTHH:mm:sszz00");
		
		Hashtable l_visitData = new Hashtable();
		l_visitData["created_at"] = l_formatDate;
		l_visitData["duration"] = p_duration;
		l_visitData["link_id"] = p_linkId;
		l_visitData["blocked"] = false;
		ArrayList l_visits = new ArrayList();
		l_visits.Add(l_visitData);
		
		Hashtable l_kidData = new Hashtable();
		l_kidData["kid_id"] = SessionHandler.getInstance().currentKid.id;
		l_kidData["created_at"] = l_formatDate;
		l_kidData["duration"] = p_duration;
		l_kidData["visits"] = l_visits;
		ArrayList l_sessions = new ArrayList();
		l_sessions.Add(l_kidData);
		
		Hashtable l_clientData = new Hashtable();
		l_clientData["client_id"] = (int)SessionHandler.getInstance().clientId;
		l_clientData["current_time"] = l_formatDate;
		l_clientData["sessions"] = l_sessions;
		
		string l_jsonData = MiniJSON.MiniJSON.jsonEncode(l_clientData);
		byte[] l_bytes = Encoding.UTF8.GetBytes(l_jsonData);

		UploadHelper l_upload = new UploadHelper("/offline/activity");
		l_upload.AddTextParameter("token", SessionHandler.getInstance().token.getSecret());
		l_upload.AddByteParameter("file", l_bytes);

		l_upload.SendAsync(_onActivityUploadComplete);
	}

	private void _onActivityUploadComplete(object p_sender, UploadDataCompletedEventArgs p_event)
	{
		_Debug.log(Encoding.UTF8.GetString(p_event.Result));
	}
}

//===================
//This for online link counter since the VisitLinkTrackRequest method seems to be offline only
public class LinkVisitRequest : RequestQueue.Request
{
	
	public int linkId;
	
	public LinkVisitRequest(int linkId, RequestQueue.RequestHandler p_handler = null) : base(p_handler)
	{
		
		this.linkId = linkId;
		
	}
	
	protected override void init()
	{
		m_call = "/api/sessions/visit";
		
		//		 m_params = new Hashtable ();
		m_params [ZoodlesConstants.PARAM_TOKEN] 	= SessionHandler.getInstance().token.getSecret();
		
		m_params [ZoodlesConstants.PARAM_CLIENT_ID] 	= SessionHandler.getInstance().clientId;
		m_params [ZoodlesConstants.PARAM_LINK_ID] 	= linkId;
		m_params [ZoodlesConstants.PARAM_KID_ID] 	= SessionHandler.getInstance ().currentKid.id;
		m_method = CallMethod.GET;
	}
	
	private void _requestComplete(WWW p_response)
	{
		string l_string = "";
		
		l_string = UnicodeDecoder.Unicode(p_response.text);

	}

}

public class VisitBookRequest : RequestQueue.Request
{
	public VisitBookRequest(RequestQueue.RequestHandler p_handler = null) : base(p_handler)
	{}
	
	protected override void init()
	{
		m_call = "/api/books/visit";
		m_params [ZoodlesConstants.PARAM_TOKEN] 	= SessionHandler.getInstance().token.getSecret();
		m_params [ZoodlesConstants.PARAM_BOOK_ID] 	= SessionHandler.getInstance ().currentBook.id;
		m_params [ZoodlesConstants.PARAM_KID_ID] 	= SessionHandler.getInstance ().currentKid.id;
		m_method = CallMethod.POST;
	}
}

public class PaymentRequest : RequestQueue.Request
{
	public PaymentRequest(string p_planId, string p_cardType, string p_cardNumber, string p_cardMonth, string p_cardYear, RequestQueue.RequestHandler p_handler = null) : base(p_handler)
	{
		m_planId = p_planId;
		m_cardType = p_cardType;
		m_cardNumber = p_cardNumber;
		m_cardMonth = p_cardMonth;
		m_cardYear = p_cardYear;
	}

	protected override void init()
	{
		m_call = "/api/accounts/billing";
		m_params[ZoodlesConstants.PARAM_TOKEN] = SessionHandler.getInstance().token.getSecret();
		m_params["creditcard[type]"] = m_cardType;
		m_params["creditcard[number]"] = m_cardNumber;
		m_params["creditcard[month]"] = m_cardMonth;
		m_params["creditcard[year]"] = m_cardYear;
		m_params["plan_id"] = m_planId;
		m_params["use_old_card"] = "false";
		m_method = CallMethod.POST;
	}

	private string m_planId;
	private string m_cardType;
	private string m_cardNumber;
	private string m_cardMonth;
	private string m_cardYear;
}

public class PremiumDetailsRequest : RequestQueue.Request
{
	public PremiumDetailsRequest(RequestQueue.RequestHandler p_handler = null) : base(p_handler)
	{}
	
	protected override void init()
	{
		m_call = "/api/subscriptions/premium_detail";
		m_params [ZoodlesConstants.PARAM_TOKEN] = SessionHandler.getInstance().token.getSecret();
		m_method = CallMethod.GET;
	}
}

public class CheckFreePremiumRequest : RequestQueue.Request
{
	public CheckFreePremiumRequest(RequestQueue.RequestHandler p_handler = null) : base(p_handler)
	{}
	
	protected override void init()
	{
		m_call = "/api/clients/check_free_premium";
		m_params [ZoodlesConstants.PARAM_CLIENT_ID] = SessionHandler.getInstance().clientId;
		m_method = CallMethod.GET;
	}
}

public class PurchaseGemsRequest : RequestQueue.Request
{
	public PurchaseGemsRequest(string p_package, string p_orderId, string p_SKU, string p_purchaseToken, string p_developerPayload, RequestQueue.RequestHandler p_handler = null) : base(p_handler)
	{
		m_orderId = p_orderId;
		m_SKU = p_SKU;
		m_purchaseToken = p_purchaseToken;
		m_developerPayload = p_developerPayload;
	}

	protected override void init()
	{
		m_call = "/api/purchase/check_purchase_gem";
		m_params[ZoodlesConstants.IAB_PACKAGE] = m_package;
		m_params[ZoodlesConstants.IAB_ORDER_ID] = m_orderId;
		m_params[ZoodlesConstants.IAB_SKU] = m_SKU;
		m_params[ZoodlesConstants.IAB_PURCHASE_TOKEN] = m_purchaseToken;
		m_params[ZoodlesConstants.IAB_DEVELOPER_PAYLOAD] = m_developerPayload;
		m_params[ZoodlesConstants.PARAM_TOKEN] = SessionHandler.getInstance().token.getSecret();
		m_method = CallMethod.POST;
	}

	private string m_package = "";
	private string m_orderId = "";
	private string m_SKU = "";
	private string m_purchaseToken = "";
	private string m_developerPayload = "";
}

public class CheckAccountRequest : RequestQueue.Request
{
	public CheckAccountRequest(string p_username, RequestQueue.RequestHandler p_handler = null) : base(p_handler)
	{
		m_username = p_username;
	}

	protected override void init()
	{
		m_call = ZoodlesConstants.CHECK_USERNAME;
		m_params [ZoodlesConstants.PARAM_EMAIL] = m_username;
	}

	private string m_username;
}

public class GetBookByIdRequest : RequestQueue.Request
{
	public GetBookByIdRequest(int p_id,RequestQueue.RequestHandler p_handler = null) : base(p_handler)
	{
		m_bookId = p_id;
	}
	
	protected override void init()
	{
		m_call = ZoodlesConstants.REST_BOOKS_URL+"/"+m_bookId;
		m_params [ZoodlesConstants.PARAM_TOKEN] = SessionHandler.getInstance().token.getSecret();
	}
	
	private int m_bookId;
}