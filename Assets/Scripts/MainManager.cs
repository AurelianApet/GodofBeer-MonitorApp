using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using SocketIO;

public class MainManager : MonoBehaviour
{
    public GameObject slideObj;
    public GameObject ImgSlidePrefab;
    public GameObject ImgSlideParent;
    public GameObject videoPlayer;
    public GameObject clientMonitor;

    public GameObject monitorTagItemPrefab;
    public GameObject monitorTagParent;
    public GameObject monitorMenuItemPrefab;
    public GameObject monitorMenuParent;
    public GameObject monitorUsageItemPrefab;
    public GameObject monitorUsageParent;
    public Text monitorTagPriceTxt;
    public Text monitorMenuPriceTxt;
    public Text monitorTotalPriceTxt;
    public Text monitorPayPriceTxt;
    public GameObject menuNoticeObj;
    public GameObject menuPriceObj;
    public Text tagSumNoticeTxt;
    public Text tagNameTxt;
    public Text tagCntTxt;
    public Text tagPriceTxt;
    public GameObject background;
    public GameObject socketPrefab;

    GameObject socketObj;
    SocketIOComponent socket;
    public static TableUsageInfo tableUsageInfo = new TableUsageInfo();
    public static PrepayTagUsageInfo prepayUsageInfo = new PrepayTagUsageInfo();
    bool is_socket_open = false;

    IEnumerator Destroy_Object(GameObject obj)
    {
        DestroyImmediate(obj);
        yield return null;
    }

    IEnumerator ShowMonitorItems(int type = 0)
    {
        while (monitorTagParent.transform.childCount > 0)
        {
            StartCoroutine(Destroy_Object(monitorTagParent.transform.GetChild(0).gameObject));
        }
        while (monitorTagParent.transform.childCount > 0)
        {
            yield return new WaitForFixedUpdate();
        }
        while (monitorMenuParent.transform.childCount > 0)
        {
            StartCoroutine(Destroy_Object(monitorMenuParent.transform.GetChild(0).gameObject));
        }
        while (monitorMenuParent.transform.childCount > 0)
        {
            yield return new WaitForFixedUpdate();
        }
        while (monitorUsageParent.transform.childCount > 0)
        {
            StartCoroutine(Destroy_Object(monitorUsageParent.transform.GetChild(0).gameObject));
        }
        while (monitorUsageParent.transform.childCount > 0)
        {
            yield return new WaitForFixedUpdate();
        }
        videoPlayer.GetComponent<MediaPlayerCtrl>().Stop();
        videoPlayer.SetActive(false);
        slideObj.SetActive(false);
        background.SetActive(true);
        clientMonitor.SetActive(true);
        if(type == 0)
        {
            menuNoticeObj.SetActive(true);
            menuPriceObj.SetActive(true);
            tagNameTxt.text = "TAG";
            tagCntTxt.text = "이용건수";
            tagPriceTxt.text = "금액";
            tagSumNoticeTxt.text = "TAG합계";
            for (int i = 0; i < tableUsageInfo.tagUsageList.Count; i++)
            {
                GameObject monitorTmp = Instantiate(monitorTagItemPrefab);
                monitorTmp.transform.SetParent(monitorTagParent.transform);
                monitorTmp.transform.Find("name").GetComponent<Text>().text = tableUsageInfo.tagUsageList[i].tagName;
                if (tableUsageInfo.tagUsageList[i].status == 1 || tableUsageInfo.tagUsageList[i].status == 3)
                {
                    monitorTmp.transform.Find("name").GetComponent<Text>().color = Color.red;
                }
                monitorTmp.transform.Find("count").GetComponent<Text>().text = tableUsageInfo.tagUsageList[i].tagUsageCnt.ToString();
                monitorTmp.transform.Find("price").GetComponent<Text>().text = Global.GetPriceFormat(tableUsageInfo.tagUsageList[i].tagUsagePrice);
                if (tableUsageInfo.tagUsageList[i].is_checked)
                {
                    monitorTmp.transform.Find("check").GetComponent<Toggle>().isOn = true;
                    for (int j = 0; j < tableUsageInfo.tagUsageList[i].tagMenuOrderList.Count; j++)
                    {
                        GameObject monitorTmp1 = Instantiate(monitorUsageItemPrefab);
                        monitorTmp1.transform.SetParent(monitorUsageParent.transform);
                        monitorTmp1.transform.Find("tag").GetComponent<Text>().text = tableUsageInfo.tagUsageList[i].tagName;
                        monitorTmp1.transform.Find("menu").GetComponent<Text>().text = tableUsageInfo.tagUsageList[i].tagMenuOrderList[i].name;
                        monitorTmp1.transform.Find("size").GetComponent<Text>().text = tableUsageInfo.tagUsageList[i].tagMenuOrderList[i].amount.ToString();
                        monitorTmp1.transform.Find("time").GetComponent<Text>().text = tableUsageInfo.tagUsageList[i].tagMenuOrderList[i].reg_datetime;
                        monitorTmp1.transform.Find("price").GetComponent<Text>().text = Global.GetPriceFormat(tableUsageInfo.tagUsageList[i].tagMenuOrderList[i].price);
                        monitorTmp1.transform.Find("check").GetComponent<Toggle>().isOn = true;
                    }
                }
                else
                {
                    monitorTmp.transform.Find("check").GetComponent<Toggle>().isOn = false;
                }
            }
            for (int i = 0; i < tableUsageInfo.menuOrderList.Count; i++)
            {
                GameObject monitorTmp = Instantiate(monitorMenuItemPrefab);
                monitorTmp.transform.SetParent(monitorMenuParent.transform);
                monitorTmp.transform.Find("name").GetComponent<Text>().text = tableUsageInfo.menuOrderList[i].name;
                monitorTmp.transform.Find("time").GetComponent<Text>().text = tableUsageInfo.menuOrderList[i].reg_datetime;
                monitorTmp.transform.Find("amount").GetComponent<Text>().text = tableUsageInfo.menuOrderList[i].amount.ToString();
                monitorTmp.transform.Find("price").GetComponent<Text>().text = Global.GetPriceFormat(tableUsageInfo.menuOrderList[i].price);
                if (tableUsageInfo.menuOrderList[i].is_checked)
                {
                    monitorTmp.transform.Find("check").GetComponent<Toggle>().isOn = true;
                }
                else
                {
                    monitorTmp.transform.Find("check").GetComponent<Toggle>().isOn = false;
                }
            }
            monitorTagPriceTxt.text = Global.GetPriceFormat(tableUsageInfo.tagPrice);
            monitorMenuPriceTxt.text = Global.GetPriceFormat(tableUsageInfo.menuPrice);
            monitorTotalPriceTxt.text = Global.GetPriceFormat(tableUsageInfo.totalPrice);
            monitorPayPriceTxt.text = Global.GetPriceFormat(tableUsageInfo.payPrice);
        }
        else
        {
            menuNoticeObj.SetActive(false);
            menuPriceObj.SetActive(false);
            tagNameTxt.text = "카드";
            tagCntTxt.text = "충전시간";
            tagPriceTxt.text = "충전금액";
            tagSumNoticeTxt.text = "충전합계";
            for (int i = 0; i < prepayUsageInfo.chargeList.Count; i++)
            {
                GameObject monitorTmp = Instantiate(monitorTagItemPrefab);
                monitorTmp.transform.SetParent(monitorTagParent.transform);
                monitorTmp.transform.Find("name").GetComponent<Text>().text = prepayUsageInfo.chargeList[i].card + " " + prepayUsageInfo.chargeList[i].cardno;
                monitorTmp.transform.Find("count").GetComponent<Text>().text = prepayUsageInfo.chargeList[i].regtime;
                monitorTmp.transform.Find("price").GetComponent<Text>().text = Global.GetPriceFormat(prepayUsageInfo.chargeList[i].price);
            }
            for (int i = 0; i < prepayUsageInfo.tagUsageList.Count; i++)
            {
                GameObject monitorTmp = Instantiate(monitorUsageItemPrefab);
                monitorTmp.transform.SetParent(monitorUsageParent.transform);
                monitorTmp.transform.Find("tag").GetComponent<Text>().text = prepayUsageInfo.tagName;
                monitorTmp.transform.Find("menu").GetComponent<Text>().text = prepayUsageInfo.tagUsageList[i].menu_name;
                monitorTmp.transform.Find("size").GetComponent<Text>().text = prepayUsageInfo.tagUsageList[i].size.ToString();
                monitorTmp.transform.Find("time").GetComponent<Text>().text = prepayUsageInfo.tagUsageList[i].regtime;
                monitorTmp.transform.Find("price").GetComponent<Text>().text = Global.GetPriceFormat(prepayUsageInfo.tagUsageList[i].price);
                monitorTmp.transform.Find("check").GetComponent<Toggle>().isOn = true;
            }
            monitorTagPriceTxt.text = Global.GetPriceFormat(prepayUsageInfo.chargePrice);
            monitorTotalPriceTxt.text = Global.GetPriceFormat(prepayUsageInfo.totalPrice);
            monitorPayPriceTxt.text = Global.GetPriceFormat(prepayUsageInfo.payPrice);
        }
    }

    void ShowIdleMonitor()
    {
        if (Global.setinfo.monitorSet.type == 0)
        {
            background.SetActive(false);
            slideObj.SetActive(true);
            videoPlayer.GetComponent<MediaPlayerCtrl>().Stop();
            videoPlayer.SetActive(false);
            clientMonitor.SetActive(false);
            if (Global.setinfo.monitorSet.images == "" || Global.setinfo.monitorSet.images == ",")
                return;
            //저장된 이미지
            try
            {
                string[] imgPath = Global.setinfo.monitorSet.images.Split(',');
                for (int i = 0; i < imgPath.Length; i++)
                {
                    GameObject slideobj = Instantiate(ImgSlidePrefab);
                    slideobj.transform.SetParent(ImgSlideParent.transform);
                    Texture2D tex = NativeGallery.LoadImageAtPath(imgPath[i]/*, 1024*/); // image will be downscaled if its width or height is larger than 1024px
                    if (tex != null)
                    {
                        slideobj.GetComponent<Image>().sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0, 0), 8f, 0, SpriteMeshType.FullRect);
                    }
                    slideobj.transform.localScale = Vector3.one;
                    slideobj.transform.localPosition = Vector3.zero;
                    if (imgPath.Length == 1)
                    {
                        slideobj.GetComponent<Image>().type = Image.Type.Simple;
                    }
                    else
                    {
                        slideobj.GetComponent<Image>().type = Image.Type.Sliced;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Log(ex);
            }
        }
        else
        {
            background.SetActive(false);
            slideObj.SetActive(false);
            clientMonitor.SetActive(false);
            videoPlayer.SetActive(true);
            try
            {
                videoPlayer.GetComponent<MediaPlayerCtrl>().m_strFileName = Global.setinfo.monitorSet.video_url;
                videoPlayer.GetComponent<MediaPlayerCtrl>().Play();
            }
            catch (Exception ex)
            {
                Debug.Log(ex);
            }
        }
    }

    IEnumerator GotoScene(string sceneName)
    {
        socket.Close();
        socket.OnDestroy();
        socket.OnApplicationQuit();
        DestroyImmediate(socketObj);
        yield return new WaitForSeconds(0.1f);
        SceneManager.LoadScene(sceneName);
    }

    public void onSet()
    {
        Global.is_from_splash = false;
        StartCoroutine(GotoScene("set"));
    }

    // Start is called before the first frame update
    void Start()
    {
        socketObj = Instantiate(socketPrefab);
        //socketObj = GameObject.Find("SocketIO");
        socket = socketObj.GetComponent<SocketIOComponent>();
        socket.On("open", socketOpen);
        socket.On("payInfo", payInfo);
        socket.On("prepayInfo", prepayInfo);
        socket.On("endpay", endPay);
        socket.On("error", socketError);
        socket.On("close", socketClose);
        ShowIdleMonitor();
    }

    public void socketOpen(SocketIOEvent e)
    {
        if (is_socket_open)
            return;
        is_socket_open = true;
        string pub_id = "{\"pub_id\":\"" + Global.userinfo.pub.id + "\"}";
        socket.Emit("monitorSetInfo", JSONObject.Create(pub_id));
        Debug.Log("[SocketIO] Open received: " + e.name + " " + e.data);
    }

    public void payInfo(SocketIOEvent e)
    {
        JSONNode jsonNode = SimpleJSON.JSON.Parse(e.data.ToString());
        Debug.Log(jsonNode);
        tableUsageInfo.tagPrice = jsonNode["tagPrice"].AsInt;
        tableUsageInfo.menuPrice = jsonNode["menuPrice"].AsInt;
        tableUsageInfo.totalPrice = jsonNode["totalPrice"].AsInt;
        tableUsageInfo.payPrice = jsonNode["payPrice"].AsInt;
        JSONNode tlist = JSON.Parse(jsonNode["taglist"].ToString());
        tableUsageInfo.tagUsageList = new List<TableTagUsageInfo>();
        for (int i = 0; i < tlist.Count; i++)
        {
            TableTagUsageInfo tinfo = new TableTagUsageInfo();
            tinfo.tagName = tlist[i]["tagName"];
            tinfo.tagUsageCnt = tlist[i]["count"].AsInt;
            tinfo.tagUsagePrice = tlist[i]["price"].AsInt;
            tinfo.status = tlist[i]["status"].AsInt;
            tinfo.is_checked = tlist[i]["is_checked"].AsBool;
            JSONNode tulist = JSON.Parse(tlist[i]["tagUsage"].ToString());
            tinfo.tagMenuOrderList = new List<TagMenuOrderInfo>();
            for (int j = 0; j < tulist.Count; j++)
            {
                TagMenuOrderInfo tuinfo = new TagMenuOrderInfo();
                tuinfo.amount = tulist[j]["amount"].AsInt;
                tuinfo.name = tulist[j]["name"];
                tuinfo.price = tulist[j]["price"].AsInt;
                tuinfo.reg_datetime = tulist[j]["reg_datetime"];
                tuinfo.status = tulist[j]["status"].AsInt;
                tinfo.tagMenuOrderList.Add(tuinfo);
            }
            tableUsageInfo.tagUsageList.Add(tinfo);
        }
        JSONNode mlist = JSON.Parse(jsonNode["menulist"].ToString());
        tableUsageInfo.menuOrderList = new List<TableMenuOrderInfo>();
        for(int i = 0; i < mlist.Count; i++)
        {
            TableMenuOrderInfo minfo = new TableMenuOrderInfo();
            minfo.amount = mlist[i]["amount"].AsInt;
            minfo.is_checked = mlist[i]["is_checked"].AsBool;
            minfo.name = mlist[i]["name"];
            minfo.price = mlist[i]["price"].AsInt;
            minfo.reg_datetime = mlist[i]["reg_datetime"];
            minfo.status = mlist[i]["status"].AsInt;
            tableUsageInfo.menuOrderList.Add(minfo);
        }
        StartCoroutine(ShowMonitorItems());
    }

    public void prepayInfo(SocketIOEvent e)
    {
        JSONNode jsonNode = SimpleJSON.JSON.Parse(e.data.ToString());
        Debug.Log(jsonNode);
        prepayUsageInfo.tagName = jsonNode["tagName"];
        prepayUsageInfo.chargePrice = jsonNode["chargePrice"].AsInt;
        prepayUsageInfo.totalPrice = jsonNode["totalPrice"].AsInt;
        prepayUsageInfo.payPrice = jsonNode["payPrice"].AsInt;
        JSONNode clist = JSON.Parse(jsonNode["chargelist"].ToString());
        prepayUsageInfo.chargeList = new List<ChargeInfo>();
        for (int i = 0; i < clist.Count; i++)
        {
            ChargeInfo ci = new ChargeInfo();
            ci.regtime = clist[i]["regtime"];
            ci.card = clist[i]["card"];
            ci.cardno = clist[i]["cardno"];
            ci.price = clist[i]["price"].AsInt;
            prepayUsageInfo.chargeList.Add(ci);
        }
        JSONNode ulist = JSON.Parse(jsonNode["usagelist"].ToString());
        prepayUsageInfo.tagUsageList = new List<TagUsageInfo>();
        for (int i = 0; i < ulist.Count; i++)
        {
            TagUsageInfo ui = new TagUsageInfo();
            ui.menu_name = ulist[i]["menu_name"];
            ui.regtime = ulist[i]["regtime"];
            ui.price = ulist[i]["price"].AsInt;
            ui.size = ulist[i]["size"].AsInt;
            prepayUsageInfo.tagUsageList.Add(ui);
        }
        StartCoroutine(ShowMonitorItems(1));
    }

    public void endPay(SocketIOEvent e)
    {
        Debug.Log("endpay");
        ShowIdleMonitor();
    }

    public void socketError(SocketIOEvent e)
    {
        Debug.Log("[SocketIO] Error received: " + e.name + " " + e.data);
    }

    public void socketClose(SocketIOEvent e)
    {
        Debug.Log("[SocketIO] Close received: " + e.name + " " + e.data);
    }

    public void OnApplicationQuit()
    {
        socket.Close();
        socket.OnDestroy();
        socket.OnApplicationQuit();
    }
}
