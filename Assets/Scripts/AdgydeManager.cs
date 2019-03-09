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

    public string UserID;

    public void getUserID()
    {
        using (activityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))   {
            activityContext = activityClass.GetStatic<AndroidJavaObject>("currentActivity");
        }
        using (pluginClass = new AndroidJavaClass("com.adgyde.android.PAgent"))
        {
            Debug.Log("UNITY :: result ");
            Sample_UI_Class.SharedInstance.DebugLog.text = "UNITY :: result";
            if (pluginClass != null)
            {
                //UserID = pluginClass.CallStatic <string> ("getUserId");
                UserID = pluginClass.GetStatic<string>("utest");
            }
            else
            {
            }
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
        _currentUser.user = UserID;
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
    void onUniqueEvent(string eventName, Dictionary<string, string> eventData)
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
                    pluginClass.CallStatic("onUniqueEvent", eventName, attrs);
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

    void onDeepLink()
    {
        using (activityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        using (pluginClass = new AndroidJavaClass("com.adgyde.android.PAgent"))
        {
            Debug.Log("UNITY :: onDeepLink 2222 ");
            if (pluginClass != null)
            {
                pluginClass.CallStatic("onDeepLink", activityContext);
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

    }

    void Start()
    {
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
        //UnityIntent ();
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
                onDeepLink();

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
        flush();
    }

    public void CountingEvent(string EventName, Dictionary<string, string> param)
    {
        Dictionary<string, string> parameter = new Dictionary<string, string>();
        parameter.Add("CountingEvent", "CountingEvent");
        onEvent(EventName, param);
        flush();
    }

    public void ComputingEvent(string EventName, Dictionary<string, string> param)
    {
        Dictionary<string, string> parameter = new Dictionary<string, string>();
        parameter["ComputingEvent"] = "name";
        parameter["name"] = "23";
        onEvent(EventName, param);
        flush();
    }

    public void UniqueEvent(string EventName, Dictionary<string, string> param)
    {
        Dictionary<string, string> parameter = new Dictionary<string, string>();
        parameter.Add("UniqueEvent", "UniqueEvent");
        onUniqueEvent(EventName, param);
        flush();
    }

    public void EventEnd(string EventName)
    {
        onEventEnd(EventName);
        flush();
    }

    public void Flush()
    {
        flush();
    }

    public void OnRevenue(int Amount)
    {
        onRevenue(Amount);
        flush();
    }

    public void DeepLinking()
    {
        onDeepLink();
        flush();
    }

    public void AdgydeToken()
    {
        // Use below lines when using Google Firebase
        onTokenRefresh(Token);
        Debug.Log("USER DATA UPLOAD");

        // Use below lines when not using Google Firebase
        //onTokenWithoutFCMRefresh ();
        //Debug.Log ("USER DATA UPLOAD without FCM");

        // Used to upload the user Token to server
        uploadTokenforRegisteration();
    }

    public void AdgydeTokenwithoutFCM()
    {
        onTokenWithoutFCMRefresh();
        Debug.Log("USER DATA UPLOAD without FCM");
        uploadTokenforRegisteration();
    }

    public string Token;

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
        AdgydeToken();
    }
    public void OnMessageRecieved(object sender, Firebase.Messaging.MessageReceivedEventArgs e)
    {
        Debug.Log("Recieved a new message from : " + e.Message.From);
    }
}
