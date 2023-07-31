using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using SimpleJSON;
using System;
using System.Text;

public class SetManager : MonoBehaviour
{
    public Toggle autologin;
    public Toggle notauto;
    public Toggle imageType;
    public Toggle videoType;
    public InputField ipTxt;
    public GameObject popup;
    public Text err_str;

    string[] slideImgs;
    string videoUrl;

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

    public void onBack()
    {
        if (Global.is_from_splash)
        {
            SceneManager.LoadScene("login");
        }
        else
        {
            SceneManager.LoadScene("main");
        }
    }

    public void Save()
    {
        if (ipTxt.text == "")
        {
            err_str.text = "ip를 확인하세요.";
            popup.SetActive(true);
            return;
        }
        if (autologin.isOn)
        {
            PlayerPrefs.SetInt("autoSave", 1);
            Global.setinfo.is_auto_login = true;
        }
        else
        {
            PlayerPrefs.SetInt("autoSave", 0);
            Global.setinfo.is_auto_login = false;
        }
        Global.server_address = ipTxt.text;
        PlayerPrefs.SetString("ip", ipTxt.text);
        Global.api_url = "http://" + Global.server_address + ":" + Global.api_server_port + "/";
        Global.socket_server = "ws://" + Global.server_address + ":" + Global.api_server_port;
        Global.setinfo.monitorSet = new MonitorSet();
        if (imageType.isOn)
        {
            Global.setinfo.monitorSet.type = 0;
            if (slideImgs == null || slideImgs.Length == 0)
            {
                Global.setinfo.monitorSet.images = "";
                PlayerPrefs.SetString("monitorImages", Global.setinfo.monitorSet.images);
                PlayerPrefs.SetInt("monitorType", Global.setinfo.monitorSet.type);
                err_str.text = "성공적으로 저장되었습니다.";
                popup.SetActive(true);
                return;
            }
            string str = "";
            for (int i = 0; i < slideImgs.Length; i++)
            {
                if (i < slideImgs.Length - 1)
                {
                    str += slideImgs[i] + ",";
                }
                else
                {
                    str += slideImgs[i];
                }
            }
            Global.setinfo.monitorSet.images = str;
            PlayerPrefs.SetString("monitorImages", Global.setinfo.monitorSet.images);
        }
        else
        {
            Global.setinfo.monitorSet.type = 1;
            if (!videoUrl.Contains("://"))
            {
                videoUrl = "file://" + videoUrl;
            }
            Global.setinfo.monitorSet.video_url = videoUrl;
            Debug.Log(videoUrl + " saved.");
            PlayerPrefs.SetString("monitorVideo", Global.setinfo.monitorSet.video_url);
        }
        PlayerPrefs.SetInt("monitorType", Global.setinfo.monitorSet.type);
        err_str.text = "성공적으로 저장되었습니다.";
        popup.SetActive(true);
    }

    void Start()
    {
        ipTxt.text = Global.server_address;
        autologin.isOn = Global.setinfo.is_auto_login;
        notauto.isOn = !Global.setinfo.is_auto_login;
        if(Global.setinfo.monitorSet.type == 0)
        {
            imageType.isOn = true;
        }
        else
        {
            videoType.isOn = true;
        }
    }

    public void openGallery()
    {
        slideImgs = null;
        videoUrl = "";
        if (imageType.isOn)
        {
            if (NativeGallery.CanSelectMultipleFilesFromGallery())
            {
                NativeGallery.GetImagesFromGallery((paths) =>
                {
                    if (paths != null)
                    {
                        slideImgs = paths;
                    }
                }, title: "출력이미지 선택", mime: "image/*");
            }
        }
        else
        {
            NativeGallery.Permission permission = NativeGallery.GetVideoFromGallery((path) =>
            {
                if (path != null)
                {
                    videoUrl = path;
                    Debug.Log(videoUrl);
                }
            }, "출력동영상 선택", mime: "video/*");
        }
    }

    public void onClosePopup()
    {
        popup.SetActive(false);
    }

    public void onExit()
    {
        Application.Quit();
    }
}
