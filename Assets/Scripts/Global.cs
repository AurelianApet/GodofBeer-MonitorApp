using System.Collections.Generic;

public struct UserInfo
{
    //public int id;
    public string userID;
    public string password;
    public PubInfo pub;
}

public struct PubInfo
{
    public string id;
    public string name;
}

public struct PosSet
{
}

public struct MonitorSet
{
    public int type;//0-images, 1-video
    public string images;//출력이미지
    public string video_url;
}

public struct SetInfo
{
    public MonitorSet monitorSet;
    public int type;//0-와인, 1-맥주
    public bool is_auto_login;
}

public struct TableUsageInfo
{
    public int tagPrice;
    public int menuPrice;
    public int totalPrice;
    public int payPrice;
    public List<TableTagUsageInfo> tagUsageList;
    public List<TableMenuOrderInfo> menuOrderList;
}

public struct ChargeInfo
{
    public string regtime;
    public string card;
    public string cardno;
    public int price;
}

public struct TagUsageInfo
{
    public string menu_name;
    public string regtime;
    public int size;
    public int price;
}

public struct PrepayTagUsageInfo
{
    public string tagName;
    public int chargePrice;
    public int totalPrice;
    public int payPrice;
    public List<ChargeInfo> chargeList;
    public List<TagUsageInfo> tagUsageList;
}

public struct TableTagUsageInfo
{
    public string tagName;
    public int tagUsageCnt;
    public int tagUsagePrice;
    public int status;
    public bool is_checked;
    public List<TagMenuOrderInfo> tagMenuOrderList;
}

public struct TableMenuOrderInfo
{
    public string name;
    public string reg_datetime;
    public int amount;
    public int price;
    public int status;
    public bool is_checked;
}

public struct TagMenuOrderInfo
{
    public string name;
    public string reg_datetime;
    public int amount;
    public int status;
    public int price;
}

public class Global
{
    //setting information
    public static bool is_from_splash = false;
    public static bool is_id_saved = false;
    public static int app_type = 1;//0-beer 1-wine
    public static SetInfo setinfo = new SetInfo();
    public static UserInfo userinfo = new UserInfo();

    //image download path
    public static string imgPath = "";
    public static string prePath = "";

    //api
    public static string server_address = "";
    public static string api_server_port = "3006";
    public static string api_url = "http://" + server_address + ":" + api_server_port + "/";
    static string api_prefix = "m-api/monitor/";

    public static string login_api = api_prefix + "login";

    //socket server
    public static string socket_server = "ws://" + server_address + ":" + api_server_port;

    public static string GetPriceFormat(int price)
    {
        return string.Format("{0:N0}", price);
    }
}