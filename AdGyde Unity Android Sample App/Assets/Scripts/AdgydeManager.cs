using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Networking;

public class AdgydeManager : MonoBehaviour
{
    private static AdgydeManager _instance = null;
    public static AdgydeManager SharedInstance
    {
        get
        {
            // if the instance hasn't been assigned then search for it
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType(typeof(AdgydeManager)) as AdgydeManager;
            }

            return _instance;
        }
    }

    AndroidJavaObject instance = null;
    AndroidJavaObject activityContext = null;
    static AndroidJavaClass androidJC_PAgent;
    static AndroidJavaObject androidJO_PAgent;

    public string Fcm_Adgyde_AppKey = "";

    AndroidJavaClass activityClass;
    AndroidJavaClass pluginClass;
    AndroidJavaClass pluginEventClass;

    string dlpdata;
    public string userID;
    public string Token;
    public string utmsource;
    public string utmmediasource;
    public string campname;
    public string campid;
    public string channelname;	
    public string channelid;
	
	
    void Init(string ApiKey, string Channel)
    {
        using (activityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            activityContext = activityClass.GetStatic<AndroidJavaObject>("currentActivity");
            //Debug.Log ("UNITY :: " + activityContext.ToString ());
        }
        using (pluginClass = new AndroidJavaClass("com.adgyde.android.PAgent"))
        {
            if (pluginClass != null)
            {
                pluginClass.CallStatic("init", activityContext, ApiKey, Channel);
            }
        }
    }

    public void getUserID()
    {
        using (activityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            activityContext = activityClass.GetStatic<AndroidJavaObject>("currentActivity");
        }
        using (pluginClass = new AndroidJavaClass("com.adgyde.android.PAgent"))
        {
            Debug.Log("UNITY :: result ");
            Sample_UI_Class.SharedInstance.DebugLog.text = "UNITY :: result";
            if (pluginClass != null)
            {
                userID = pluginClass.CallStatic <string> ("getUserId");
                //userID = pluginClass.GetStatic<string>("utest");
            }
        }
    }

    public String getdDldata()
    {
        using (activityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            activityContext = activityClass.GetStatic<AndroidJavaObject>("currentActivity");
        }
        using (pluginClass = new AndroidJavaClass("com.adgyde.android.PAgent"))
        {
            Debug.Log("UNITY :: result ");
            Sample_UI_Class.SharedInstance.DebugLog.text = "UNITY :: result";
            if (pluginClass != null)
            {
                dlpdata = pluginClass.CallStatic<string>("getDeeplinkUri", activityContext);          
            }
            return dlpdata;
        }
    }

    public void callFcmService()
    {
        using (activityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        using (pluginClass = new AndroidJavaClass("com.adgyde.android.PAgent"))
        {
            if (pluginClass != null)
            {
                pluginClass.CallStatic("callFcmService", activityContext);
            }
        }
    }

    string JsonObjectString;

    void uploadTokenforRegisteration()
    {
        UserDetails _currentUser = new UserDetails();
        _currentUser.user = userID;
        _currentUser.token = Token;
        _currentUser.appkey = Fcm_Adgyde_AppKey;

        JsonObjectString = JsonUtility.ToJson(_currentUser);
        StartCoroutine(PostRequest());
    }

    public static string FCM_Token_Upload_URL = "http://log2.adgyde.com/intf/a/1/fcmtoken";
    string ResponseString;

    IEnumerator PostRequest()
    {
        WWWForm form = new WWWForm();
        form.AddField("Content-Encoding", JsonObjectString);

        using (UnityWebRequest www = UnityWebRequest.Post(FCM_Token_Upload_URL, form))
        {
            yield return www.Send();
            if (www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                ResponseString = (www.downloadHandler.text).ToString();
            }
        }
    }

    void ResponseCallBack()
    {
        var N = SimpleJSON.JSON.Parse(ResponseString);
    }

    void onEvent(string eventName)
    {
        using (activityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            activityContext = activityClass.GetStatic<AndroidJavaObject>("currentActivity");
        }
        using (pluginClass = new AndroidJavaClass("com.adgyde.android.PAgent"))
        {
            if (pluginClass != null)
            {
                pluginClass.CallStatic("onEvent", eventName);
            }
        }
    }
    void Start()
    {
        getDeeplinkDataURl();
    }

    void onEvent(string eventName, Dictionary<string, string> eventData)
    {
        using (activityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            activityContext = activityClass.GetStatic<AndroidJavaObject>("currentActivity");
        }
        using (pluginClass = new AndroidJavaClass("com.adgyde.android.PAgent"))
        {
            if (pluginClass != null)
            {
                using (AndroidJavaObject obj_HashMap = new AndroidJavaObject("java.util.HashMap"))
                {
                    IntPtr method_Put = AndroidJNIHelper.GetMethodID(obj_HashMap.GetRawClass(), "put", "(Ljava/lang/String;Ljava/lang/String;)Ljava/lang/String;");
                    object[] args = new object[2];

                    foreach (KeyValuePair<string, string> kvp in eventData)
                    {
                        using (AndroidJavaObject k = new AndroidJavaObject("java.lang.String", kvp.Key))
                        {
                            using (AndroidJavaObject v = new AndroidJavaObject("java.lang.String", kvp.Value))
                            {
                                args[0] = kvp.Key;
                                args[1] = kvp.Value;
                                AndroidJNI.CallObjectMethod(obj_HashMap.GetRawObject(), method_Put, AndroidJNIHelper.CreateJNIArgArray(args));
                            }
                        }
                    }
                    pluginClass.CallStatic("onEvent", eventName, obj_HashMap);
                }
            }
        }
    }

    void onDailyUnique(string eventName, Dictionary<string, string> eventData)
    {
        using (activityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            activityContext = activityClass.GetStatic<AndroidJavaObject>("currentActivity");
        }
        using (pluginClass = new AndroidJavaClass("com.adgyde.android.PAgent"))
        {
            if (pluginClass != null)
            {
                using (AndroidJavaObject attrs = ConvertHashMap(eventData))
                {
                    pluginClass.CallStatic("onDailyUnique", eventName, attrs);
                }
            }

        }
    }

    void onPermanentUnique(string eventName, Dictionary<string, string> eventData)
    {
        using (activityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            activityContext = activityClass.GetStatic<AndroidJavaObject>("currentActivity");
        }
        using (pluginClass = new AndroidJavaClass("com.adgyde.android.PAgent"))
        {
            if (pluginClass != null)
            {
                using (AndroidJavaObject attrs = ConvertHashMap(eventData))
                {
                    pluginClass.CallStatic("onPermanentUnique", eventName, attrs);
                }
            }

        }
    }

    void onCustomUnique(string eventName, Dictionary<string, string> eventData, long time)
    {
        using (activityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            activityContext = activityClass.GetStatic<AndroidJavaObject>("currentActivity");
        }
        using (pluginClass = new AndroidJavaClass("com.adgyde.android.PAgent"))
        {
            if (pluginClass != null)
            {
                using (AndroidJavaObject attrs = ConvertHashMap(eventData))
                {
                    pluginClass.CallStatic("onCustomUnique", eventName, attrs, time);
                }
            }
        }
    }


    private static AndroidJavaObject ConvertHashMap(Dictionary<string, string> dict)
    {
        AndroidJavaObject obj_HashMap = new AndroidJavaObject("java.util.HashMap");
        IntPtr method_Put = AndroidJNIHelper.GetMethodID(obj_HashMap.GetRawClass(), "put", "(Ljava/lang/Object;Ljava/lang/Object;)Ljava/lang/Object;");

        object[] args = new object[2];
        foreach (KeyValuePair<string, string> kvp in dict)
        {
            using (AndroidJavaObject k = new AndroidJavaObject("java.lang.String", kvp.Key))
            {
                using (AndroidJavaObject v = new AndroidJavaObject("java.lang.String", kvp.Value))
                {
                    args[0] = k;
                    args[1] = v;
                    AndroidJNI.CallObjectMethod(obj_HashMap.GetRawObject(),
                        method_Put, AndroidJNIHelper.CreateJNIArgArray(args));
                }
            }
        }
        return obj_HashMap;
    }

    void onEventEnd(string eventName)
    {
        using (activityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            activityContext = activityClass.GetStatic<AndroidJavaObject>("currentActivity");
        }
        using (pluginClass = new AndroidJavaClass("com.adgyde.android.PAgent"))
        {
            if (pluginClass != null)
            {
                pluginClass.CallStatic("onEventEnd", eventName);
            }
        }
    }

    void setAge(int year, int month, int day)
    {
        using (activityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            activityContext = activityClass.GetStatic<AndroidJavaObject>("currentActivity");
        }
        using (pluginClass = new AndroidJavaClass("com.adgyde.android.PAgent"))
        {
            if (pluginClass != null)
            {
                Debug.Log("Pepper:: if Call, Setage");
                pluginClass.CallStatic("setAge", activityContext, year, month, day);
            }
            else
            {
                Debug.Log("Pepper:: Else Call, Setage");
            }
        }
    }

    void setAge(int age)
    {
        using (activityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            activityContext = activityClass.GetStatic<AndroidJavaObject>("currentActivity");
        }
        using (pluginClass = new AndroidJavaClass("com.adgyde.android.PAgent"))
        {
            if (pluginClass != null)
            {
                Debug.Log("Pepper:: If Call, Setage");
                pluginClass.CallStatic("setAge", activityContext, age);
            }
            else
            {
                Debug.Log("Pepper:: Else Call, Setage");
            }
        }
    }

    void setGender(string i)
    {
        using (activityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            activityContext = activityClass.GetStatic<AndroidJavaObject>("currentActivity");
        }
        using (pluginClass = new AndroidJavaClass("com.adgyde.android.PAgent"))
        {
            if (pluginClass != null)
            {
                Debug.Log("Pepper:: if Call, setGender");
                pluginClass.CallStatic("setGender", activityContext, i);
            }
            else
            {
                Debug.Log("Pepper:: Else Call, setGender");
            }
        }
    }

    public void onSetCurrentScreen(String scr)
    {
        using (activityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            activityContext = activityClass.GetStatic<AndroidJavaObject>("currentActivity");
        }
        using (pluginClass = new AndroidJavaClass("com.adgyde.android.PAgent"))
        {
            if (pluginClass != null)
            {
                pluginClass.CallStatic("setCurrentScreen", activityContext, scr);
            }
        }
    }

    public void onRemoveCurrentScreen(String scr)
    {
        using (activityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            activityContext = activityClass.GetStatic<AndroidJavaObject>("currentActivity");
        }
        using (pluginClass = new AndroidJavaClass("com.adgyde.android.PAgent"))
        {
            if (pluginClass != null)
            {
                pluginClass.CallStatic("removeCurrentScreen", activityContext, scr);
            }
        }
    }


    void flush()
    {
        using (activityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            activityContext = activityClass.GetStatic<AndroidJavaObject>("currentActivity");
        }
        using (pluginClass = new AndroidJavaClass("com.adgyde.android.PAgent"))
        {
            if (pluginClass != null)
            {
                pluginClass.CallStatic("flush");
            }
        }
    }

    void onRevenue(int amount)
    {
        using (activityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            activityContext = activityClass.GetStatic<AndroidJavaObject>("currentActivity");
        }
        using (pluginClass = new AndroidJavaClass("com.adgyde.android.PAgent"))
        {
            if (pluginClass != null)
            {
                pluginClass.CallStatic("onRevenue", amount);
            }
        }
    }

    void setUserId(string id)
    {
        using (activityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            activityContext = activityClass.GetStatic<AndroidJavaObject>("currentActivity");
        }
        using (pluginClass = new AndroidJavaClass("com.adgyde.android.PAgent"))
        {
            if (pluginClass != null)
            {
                pluginClass.CallStatic("setClientUserId", id);
            }
        }
    }


    void setPhoneNumber(string phonenumber)
    {
        using (activityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            activityContext = activityClass.GetStatic<AndroidJavaObject>("currentActivity");
        }
        using (pluginClass = new AndroidJavaClass("com.adgyde.android.PAgent"))
        {
            if (pluginClass != null)
            {
                pluginClass.CallStatic("setPhoneno", activityContext, phonenumber);
            }
        }
    }

    void setEmailId(string emailId)
    {
        using (activityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            activityContext = activityClass.GetStatic<AndroidJavaObject>("currentActivity");
        }
        using (pluginClass = new AndroidJavaClass("com.adgyde.android.PAgent"))
        {
            if (pluginClass != null)
            {
                pluginClass.CallStatic("setEmail", activityContext, emailId);
            }
        }
    }



    public void getUtmSource()
    {
        using (activityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            activityContext = activityClass.GetStatic<AndroidJavaObject>("currentActivity");
        }
        using (pluginClass = new AndroidJavaClass("com.adgyde.android.PAgent"))
        {
            Debug.Log("UNITY :: result ");
            if (pluginClass != null)
            {
                utmsource = pluginClass.CallStatic<string>("getUtmSource");
                Debug.Log("utm source = " + utmsource);
                Debug.Log("Pepper, utmsource = " + utmsource);
            }
        }
    }

    public void getMediaSource()
    {
        using (activityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            activityContext = activityClass.GetStatic<AndroidJavaObject>("currentActivity");
        }
        using (pluginClass = new AndroidJavaClass("com.adgyde.android.PAgent"))
        {
            Debug.Log("UNITY :: result ");
            if (pluginClass != null)
            {
                utmmediasource = pluginClass.CallStatic<string>("getMediaSource");
                Debug.Log("mediasource = " + utmmediasource);
                Debug.Log("Pepper, mediasource = " + utmmediasource);
            }
        }
    }

    public void getCampaignName()
    {
        using (activityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            activityContext = activityClass.GetStatic<AndroidJavaObject>("currentActivity");
        }
        using (pluginClass = new AndroidJavaClass("com.adgyde.android.PAgent"))
        {
            Debug.Log("UNITY :: result ");
            if (pluginClass != null)
            {
                campname = pluginClass.CallStatic<string>("getCampaignName");
                Debug.Log("campname = " + campname);
                Debug.Log("Pepper, campname = " + campname);
            }
        }
    }

    public void getCampaignId()
    {
        using (activityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            activityContext = activityClass.GetStatic<AndroidJavaObject>("currentActivity");
        }
        using (pluginClass = new AndroidJavaClass("com.adgyde.android.PAgent"))
        {
            Debug.Log("UNITY :: result ");
            if (pluginClass != null)
            {
                campid = pluginClass.CallStatic<string>("getCampaignId");
                Debug.Log("campid = " + campid);
                Debug.Log("Pepper, campid = " + campid);
            }
        }
    }

    public void getChannelName()
    {
        using (activityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            activityContext = activityClass.GetStatic<AndroidJavaObject>("currentActivity");
        }
        using (pluginClass = new AndroidJavaClass("com.adgyde.android.PAgent"))
        {
            Debug.Log("UNITY :: result ");
            if (pluginClass != null)
            {
                channelname = pluginClass.CallStatic<string>("getChannelName");
                Debug.Log("channel name = " + channelname);
                Debug.Log("Pepper, channel name = " + channelname);
            }
        }
    }

    public void getChannelId()
    {
        using (activityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            activityContext = activityClass.GetStatic<AndroidJavaObject>("currentActivity");
        }
        using (pluginClass = new AndroidJavaClass("com.adgyde.android.PAgent"))
        {
            Debug.Log("UNITY :: result ");
            if (pluginClass != null)
            {
                channelid = pluginClass.CallStatic<string>("getChannelId");
                Debug.Log("channel name = " + channelid);
                Debug.Log("Pepper, channel name = " + channelid);
            }
        }
    }

    void onAllowImeiPermission(Boolean value)
    {
        using (activityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            activityContext = activityClass.GetStatic<AndroidJavaObject>("currentActivity");
        }
        using (pluginClass = new AndroidJavaClass("com.adgyde.android.PAgent"))
        {
            if (pluginClass != null)
            {
                pluginClass.CallStatic("allowPermissionIMEI", activityContext, value);
            }
        }

    }

    void onDeepLink(String pkgname, String classname, String dataurl)
    {
        using (activityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            activityContext = activityClass.GetStatic<AndroidJavaObject>("currentActivity");
        }

        using (pluginClass = new AndroidJavaClass("com.adgyde.android.PAgent"))
        {
            if (pluginClass != null)
            {
                pluginClass.CallStatic("getDeeplinking", activityContext, pkgname, classname, dataurl);
            }
        }
    }

    void onDeferredDeeplink() {
        using (activityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            activityContext = activityClass.GetStatic<AndroidJavaObject>("currentActivity");
        }

        using (pluginClass = new AndroidJavaClass("com.adgyde.android.PAgent"))
        {
            Debug.Log("UNITY :: onDeepLink 2222 ");
            if (pluginClass != null)
            {
                pluginClass.CallStatic("deferredDeepLink", activityContext);
            }
        }
    }

    void onTokenRefresh(string token)
    {
        using (activityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            activityContext = activityClass.GetStatic<AndroidJavaObject>("currentActivity");
        }
        using (pluginClass = new AndroidJavaClass("com.adgyde.android.PAgent"))
        {
            if (pluginClass != null)
            {
                pluginClass.CallStatic("onTokenRefresh", token);
            }
        }
    }

    void onTokenWithoutFCMRefresh()
    {
        using (activityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            activityContext = activityClass.GetStatic<AndroidJavaObject>("currentActivity");
        }
        using (pluginClass = new AndroidJavaClass("com.adgyde.android.PAgent"))
        {
            if (pluginClass != null)
            {
                pluginClass.CallStatic("callFcmService");
            }
        }
    }

    void setDebugEnabled(bool enabled)
    {
        using (activityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            activityContext = activityClass.GetStatic<AndroidJavaObject>("currentActivity");
        }
        using (pluginClass = new AndroidJavaClass("com.adgyde.android.PAgent"))
        {
            if (pluginClass != null)
            {
                pluginClass.CallStatic("setDebugEnabled", enabled);
            }
        }
    }

    void setReportLocationEnabled(bool enabled)
    {
        using (activityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            activityContext = activityClass.GetStatic<AndroidJavaObject>("currentActivity");
        }
        using (pluginClass = new AndroidJavaClass("com.adgyde.android.PAgent"))
        {
            if (pluginClass != null)
            {
                pluginClass.CallStatic("setReportLocationEnabled", enabled);
            }
        }
    }

    void setLocation(long lng, long lat)
    {
        using (activityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            activityContext = activityClass.GetStatic<AndroidJavaObject>("currentActivity");
        }
        using (pluginClass = new AndroidJavaClass("com.adgyde.android.PAgent"))
        {
            if (pluginClass != null)
            {
                pluginClass.CallStatic("setLocation", lng, lat);
            }
        }
    }

    void setSessionTimeout(int seconds)
    {
        using (activityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            activityContext = activityClass.GetStatic<AndroidJavaObject>("currentActivity");
        }
        using (pluginClass = new AndroidJavaClass("com.adgyde.android.PAgent"))
        {
            if (pluginClass != null)
            {
                pluginClass.CallStatic("setSessionTimeout", seconds);
            }
        }
    }

    void CallOnStop()
    {
        using (activityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            activityContext = activityClass.GetStatic<AndroidJavaObject>("currentActivity");
        }
        using (pluginClass = new AndroidJavaClass("com.adgyde.android.PAgent"))
        {
            if (pluginClass != null)
            {
                pluginClass.CallStatic("callOnStop", activityContext);
            }
        }
    }

    public void CallOnStart()
    {
        using (activityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            activityContext = activityClass.GetStatic<AndroidJavaObject>("currentActivity");
        }
        using (pluginClass = new AndroidJavaClass("com.adgyde.android.PAgent"))
        {
            if (pluginClass != null)
            {
                pluginClass.CallStatic("callOnStart", activityContext);
            }
        }
    }

    void CallOnResume()
    {
        using (activityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            activityContext = activityClass.GetStatic<AndroidJavaObject>("currentActivity");
        }
        using (pluginClass = new AndroidJavaClass("com.adgyde.android.PAgent"))
        {
            if (pluginClass != null)
            {
                pluginClass.CallStatic("callOnResume", activityContext);
            }
        }
    }

    void CallOnPause()
    {
        using (activityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            activityContext = activityClass.GetStatic<AndroidJavaObject>("currentActivity");
        }
        using (pluginClass = new AndroidJavaClass("com.adgyde.android.PAgent"))
        {
            if (pluginClass != null)
            {
                pluginClass.CallStatic("callOnPause", activityContext);
            }
        }
    }

    void CallOnDestroy()
    {
        using (activityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            activityContext = activityClass.GetStatic<AndroidJavaObject>("currentActivity");
        }
        using (pluginClass = new AndroidJavaClass("com.adgyde.android.PAgent"))
        {
            if (pluginClass != null)
            {
                pluginClass.CallStatic("callOnDestroy", activityContext);
            }
        }
    }

    void CallOnCreate()
    {
        using (activityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            activityContext = activityClass.GetStatic<AndroidJavaObject>("currentActivity");
        }
        using (pluginClass = new AndroidJavaClass("com.adgyde.android.PAgent"))
        {
            if (pluginClass != null)
            {
                pluginClass.CallStatic("callOnCreate", activityContext);
            }
        }
    }

    void OnApplicationQuit()
    {
        Quit();
    }

    void OnApplicationKill()
    {
        Quit();
    }

    public void Quit()
    {
        CallOnStop();
        Application.Quit();
    }

    IEnumerator CallDelay(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        CallOnDestroy();
        Application.Quit();
    }

    void OnApplicationPause(bool isPause)
    {
        if (isPause)
        {
            CallOnPause();
        }
        else
        {
            CallOnResume();
        }
    }


    // -------------------------- Plugin's method call --------------------- //

    void Awake()
    {
        getDeeplinkDataURl();
    }

    /* 
    * Initialize AdGyde SDK with appkey & default channel id "Organic".
    * When applictaion is installed from Google Play Store without any campaign the Channel will be Organic as specified in Init    Function
    * In case the applictaion is installed through a campaign link then the Default channel will be overriden and value from the campaign link will be passed
   */
    public void Adgyde_Init(String AdGyde_AppKey, String channel_name)
    {
        Init(AdGyde_AppKey, channel_name);
        flush();
        setDebugEnabled(true);
        Fcm_Adgyde_AppKey = AdGyde_AppKey;
        CallOnCreate();
    }

    void UnityIntent()
    {
        AndroidJavaClass UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject currentActivity = UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaObject intent = currentActivity.Call<AndroidJavaObject>("getIntent");
        AndroidJavaObject action = intent.Call<AndroidJavaObject>("getAction");

        string action_1 = "android.intent.action.VIEW";

        if (action.Equals(action))
        {
            try
            {
                onDeepLink("", "", "");
            }
            catch (Exception e)
            {
                e.GetType();
            }
        }
    }

    public void SimpleEvent(string EventName)
    {
        onEvent(EventName);
    }

    public void CountingEvent(string EventName, Dictionary<string, string> param)
    {
        onEvent(EventName, param);
    }

    public void ComputingEvent(string EventName, Dictionary<string, string> param)
    {
        onEvent(EventName, param);
    }

    public void DailyUniqueEvent(string EventName, Dictionary<string, string> param)
    {
        onDailyUnique(EventName, param);
    }

    public void PermanentUniqueEvent(string EventName, Dictionary<string, string> param)
    {
        onPermanentUnique(EventName, param);
    }

    public void CustomUniqueEvent(string EventName, Dictionary<string, string> param, int time)
    {
        onCustomUnique(EventName, param, time);
    }
	
    public void EventEnd(string EventName)
    {
        onEventEnd(EventName);
    }

    public void OnsetAge(int year, int month, int day)
    {
        setAge(year, month, day);
    }

    public void OnsetAge(int age)
    {
        setAge(age);
    }

    public void OnsetGender(string i)
    {
        setGender(i);
    }

    public void setCurrentScreen(String scr)
    {
        onSetCurrentScreen(scr);
    }

    public void removeCurrentScreen(String removescr)
    {
        onRemoveCurrentScreen(removescr);
    }

    public void setemailId(String emailId)
    {
        setEmailId(emailId);
    }

    public void setphoneNumber(String phonenumber)
    {
        setPhoneNumber(phonenumber);
    }

    public void Flush()
    {
        flush();
    }

    public void OnRevenue(int Amount)
    {
        onRevenue(Amount);
    }

    public void OnSetUserId(string id)
    {
        setUserId(id);
    }

    public void OngetUtmSource()
    {
        getUtmSource();
    }
	
    public void OngetMediaSource()
    {
        getMediaSource();
    }
	
    public void OngetCampaignName()
    {
        getCampaignName();
    }
	
    public void OngetCampaignId()
    {
        getCampaignId();
    }
	
    public void OngetChannelName()
    {
        getChannelName();
    }
	
    public void OngetChannelId()
    {
        getChannelId();
    }

    public void OnImeiPermission(Boolean value)
    {
        onAllowImeiPermission(value);
    }

    public void getDeeplinkDataURl()
    {
               getdDldata();
    }

   public void onFcmTokenRecieved(String token)
    {
        // Use below lines when using Google Firebase
        // Used to upload the user Token to server
        onTokenRefresh(token);
        Debug.Log("USER DATA UPLOAD");
    }

    public void callAdGydeFcmService()
    {
        onTokenWithoutFCMRefresh();
        Debug.Log("USER DATA UPLOAD without FCM");
        uploadTokenforRegisteration();
    }

    public void InitialiseFirebase()
    {
        Firebase.Messaging.FirebaseMessaging.TokenReceived += OnTokenRecieved;
        Firebase.Messaging.FirebaseMessaging.MessageReceived += OnMessageRecieved;
    }

    public void OnTokenRecieved(object sender, Firebase.Messaging.TokenReceivedEventArgs token)
    {
        Token = token.Token.ToString();
        StartCoroutine(CallDelayforUserID(15.5f));
    }
    public IEnumerator CallDelayforUserID(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        getUserID();      
    }
    public void OnMessageRecieved(object sender, Firebase.Messaging.MessageReceivedEventArgs e)
    {
        Debug.Log("Recieved a new message from : " + e.Message.From);
    }
}
