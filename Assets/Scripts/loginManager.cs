using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class loginManager : MonoBehaviour
{
    public InputField userId;
    public InputField password;
    public Toggle idSave;
    public Toggle autoSave;
    public Toggle beerType;
    public Toggle wineType;
    public GameObject err_popup;
    public Text err_str;

    // Start is called before the first frame update
    void Start()
    {
        if (Global.is_id_saved)
        {
            userId.text = Global.userinfo.userID;
            idSave.isOn = true;
        }
        if (Global.setinfo.is_auto_login)
        {
            autoSave.isOn = true;
        }
        if(Global.app_type == 0)
        {
            //beer
            beerType.isOn = true;
        }
        else
        {
            wineType.isOn = true;
        }
    }

    public void Login()
    {
        if (userId.text == "" || password.text == "")
        {
            err_str.text = "로그인 정보를 확인하세요.";
            err_popup.SetActive(true);
        }
        else
        {
            WWWForm form = new WWWForm();
            form.AddField("userID", userId.text);
            form.AddField("password", password.text);
            if (beerType.isOn)
            {
                Global.app_type = 0;
            }
            else
            {
                Global.app_type = 1;
            }
            form.AddField("type", Global.app_type);
            Debug.Log(Global.api_url + Global.login_api);
            WWW www = new WWW(Global.api_url + Global.login_api, form);
            StartCoroutine(ProcessLogin(www, idSave.isOn, userId.text, autoSave.isOn, password.text));
        }
    }

    IEnumerator ProcessLogin(WWW www, bool is_idsave, string username, bool is_autosave, string password)
    {
        yield return www;
        if (www.error == null)
        {
            JSONNode jsonNode = SimpleJSON.JSON.Parse(www.text);
            if (jsonNode["suc"].AsInt == 1)
            {
                PlayerPrefs.SetInt("app_type", Global.app_type);
                //Global.userinfo.id = jsonNode["uid"].AsInt;
                if (is_idsave)
                {
                    PlayerPrefs.SetInt("idSave", 1);
                    PlayerPrefs.SetString("id", username);
                    Global.is_id_saved = true;
                }
                else
                {
                    PlayerPrefs.SetInt("idSave", 0);
                    Global.is_id_saved = false;
                }
                if (is_autosave)
                {
                    Debug.Log("autosave");
                    PlayerPrefs.SetInt("autoSave", 1);
                    PlayerPrefs.SetString("id", username);
                    PlayerPrefs.SetString("pwd", password);
                    Global.setinfo.is_auto_login = true;
                }
                else
                {
                    PlayerPrefs.SetInt("autoSave", 0);
                    Global.setinfo.is_auto_login = false;
                }
                Global.userinfo.userID = username;
                Global.userinfo.password = password;
                Global.userinfo.pub = new PubInfo();
                PubInfo pinfo = new PubInfo();
                pinfo.id = jsonNode["pub_id"];
                pinfo.name = jsonNode["pub_name"];
                Global.userinfo.pub = pinfo;
                SceneManager.LoadScene("main");
            }
            else
            {
                err_str.text = "로그인 정보를 확인하세요.";
                err_popup.SetActive(true);
            }
        }
        else
        {
            err_str.text = "인터넷 연결을 확인하세요.";
            err_popup.SetActive(true);
        }
    }

    public void onConfirmErrPopup()
    {
        err_popup.SetActive(false);
    }

    public void onSet()
    {
        Global.is_from_splash = true;
        SceneManager.LoadScene("set");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    float time = 0f;

    void FixedUpdate()
    {
        if (!Input.anyKey)
        {
            time += Time.deltaTime;
        }
        else
        {
            if (time != 0f)
            {
                GameObject.Find("touch").GetComponent<AudioSource>().Play();
                time = 0f;
            }
        }
    }
}
