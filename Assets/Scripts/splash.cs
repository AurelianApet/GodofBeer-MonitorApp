using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using SimpleJSON;

public class splash : MonoBehaviour
{
    // Start is called before the first frame update
    public float delay_time = 0.5f;

    IEnumerator Start()
    {
#if UNITY_IPHONE
		Global.imgPath = Application.persistentDataPath + "/wmonitor_img/";
#elif UNITY_ANDROID
        Global.imgPath = Application.persistentDataPath + "/wmonitor_img/";
#else
if( Application.isEditor == true ){ 
    	Global.imgPath = "/img/";
} 
#endif

#if UNITY_IPHONE
		Global.prePath = @"file://";
#elif UNITY_ANDROID
        Global.prePath = @"file:///";
#else
		Global.prePath = @"file://" + Application.dataPath.Replace("/Assets","/");
#endif

        //delete all downloaded images
        try
        {
            if (Directory.Exists(Global.imgPath))
            {
                Directory.Delete(Global.imgPath, true);
            }
        }
        catch (Exception)
        {

        }

        Global.server_address = PlayerPrefs.GetString("ip");
        Global.api_url = "http://" + Global.server_address + ":" + Global.api_server_port + "/";
        Global.socket_server = "ws://" + Global.server_address + ":" + Global.api_server_port;
        if (Global.server_address == "")
        {
            Global.is_from_splash = true;
            SceneManager.LoadScene("set");
        }

        if (PlayerPrefs.GetInt("idSave") == 1)
        {
            Global.is_id_saved = true;
            Global.userinfo.userID = PlayerPrefs.GetString("id");
        }
        else
        {
            Global.is_id_saved = false;
        }
        Global.setinfo.monitorSet.type = PlayerPrefs.GetInt("monitorType");
        Global.setinfo.monitorSet.images = PlayerPrefs.GetString("monitorImages");
        Global.setinfo.monitorSet.video_url = PlayerPrefs.GetString("monitorVideo");
        Global.app_type = PlayerPrefs.GetInt("app_type");
        if (PlayerPrefs.GetInt("autoSave") == 1)
        {
            Debug.Log("auto save");
            Global.setinfo.is_auto_login = true;
            Global.userinfo.userID = PlayerPrefs.GetString("id");
            Global.userinfo.password = PlayerPrefs.GetString("pwd");
            WWWForm form = new WWWForm();
            form.AddField("userID", Global.userinfo.userID);
            form.AddField("password", Global.userinfo.password);
            form.AddField("type", Global.app_type);
            WWW www = new WWW(Global.api_url + Global.login_api, form);
            StartCoroutine(ProcessLogin(www));
        }
        else
        {
            Global.setinfo.is_auto_login = false;
            yield return new WaitForSeconds(delay_time);
            SceneManager.LoadScene("login");
        }
    }

    IEnumerator ProcessLogin(WWW www)
    {
        yield return www;
        if (www.error == null)
        {
            JSONNode jsonNode = SimpleJSON.JSON.Parse(www.text);
            if (jsonNode["suc"].AsInt == 1)
            {
                //Global.userinfo.id = jsonNode["uid"].AsInt;
                Global.userinfo.pub = new PubInfo();
                PubInfo pinfo = new PubInfo();
                pinfo.id = jsonNode["pub_id"];
                pinfo.name = jsonNode["pub_name"];
                Global.userinfo.pub = pinfo;
                yield return new WaitForSeconds(delay_time);
                SceneManager.LoadScene("main");
            }
            else
            {
                yield return new WaitForSeconds(delay_time);
                SceneManager.LoadScene("login");
            }
        }
        else
        {
            yield return new WaitForSeconds(delay_time);
            SceneManager.LoadScene("login");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
