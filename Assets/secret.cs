using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;
using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Runtime.Serialization.Formatters.Binary;

public class secret : MonoBehaviour {

	void Start () {

		certName = ZoodlesConstants.CLIENT_CERT;//ZoodlesConstants.CLIENTCERTNAME;

		m_cerPwd = GameObject.Find ("cerpwd").GetComponent<InputField>();
		m_encpwd = GameObject.Find ("encpwd").GetComponent<InputField>();
		m_genPwd = GameObject.Find ("genPwd").GetComponent<Button>();
		m_encCer = GameObject.Find ("encCer").GetComponent<Button>();
		if(File.Exists(Application.dataPath + "/StreamingAssets/data.dat"))
		{
			FileStream fs = new FileStream(Application.dataPath + "/StreamingAssets/data.dat", FileMode.Open);
			BinaryFormatter bf = new BinaryFormatter();
			m_pwdStore = bf.Deserialize (fs) as PwdStore;
			fs.Close ();
			m_cerPwd.text = m_pwdStore.cerPwd;
			m_encpwd.text = m_pwdStore.encPwd;
		}
		else
		{
			m_pwdStore = new PwdStore();
		}

		m_genPwd.onClick.AddListener (onGenPwdClick);
		m_encCer.onClick.AddListener (onEncCerClick);
	}

	void Update () {
		
	}

	private void onEncCerClick()
	{
		if(File.Exists(Application.dataPath + "/StreamingAssets" + certName))
		{
			FileEncrypt.EncryptFile (Application.dataPath + "/StreamingAssets" + certName,m_pwdStore.encPwd,progress);
		}

		if(File.Exists(Application.persistentDataPath + certName))
		{
			File.Delete(Application.persistentDataPath + certName);
		}
	}

	public void progress(int max, int value)
	{
		_Debug.log (value + "/" + max);
		Debug.Log ("Encrypt Certificate successfully!");
	}

	private void onGenPwdClick()
	{
		if (!String.Empty.Equals (m_cerPwd.text))
			m_pwdStore.cerPwd = m_cerPwd.text;
		if (!String.Empty.Equals (m_encpwd.text))
			m_pwdStore.encPwd = m_encpwd.text;
		if(File.Exists(Application.dataPath + "/StreamingAssets/data.dat"))
		{
			File.Delete(Application.dataPath + "/StreamingAssets/data.dat");
		}
		FileStream fs = new FileStream(Application.dataPath + "/StreamingAssets/data.dat", FileMode.Create);

		BinaryFormatter bf = new BinaryFormatter();
		bf.Serialize(fs, m_pwdStore);
		fs.Close();
		if(File.Exists(Application.persistentDataPath + "/data.dat"))
		{
			File.Delete(Application.persistentDataPath + "/data.dat");
		}
		Debug.Log ("Generate password successfully!");
	}

	private PwdStore m_pwdStore;
	private InputField m_cerPwd;
	private InputField m_encpwd;
	private Button m_genPwd;
	private Button m_encCer;

	private string certName;
}
