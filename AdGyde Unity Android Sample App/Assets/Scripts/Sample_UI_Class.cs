using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using UnityEngine.UI;

public class Sample_UI_Class : MonoBehaviour
{
    private static Sample_UI_Class _instance = null;
    public static Sample_UI_Class SharedInstance
    {
        get
        {
            // if the instance hasn't been assigned then search for it
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType(typeof(Sample_UI_Class)) as Sample_UI_Class;
            }

            return _instance;
        }
    }

    public Text DebugLog;


    /* 
  * Initialize AdGyde SDK with appkey & default channel id "Organic".
  * When applictaion is installed from Google Play Store without any campaign the Channel will be Organic as specified in Init    Function
  * In case the applictaion is installed through a campaign link then the Default channel will be overriden and value from the campaign link will be passed
  * You can add your screen name in user flow by passing screen name on "AdgydeManager.SharedInstance.setCurrentScreen("Screen_name")" method.
  * Call "AdgydeManager.SharedInstance.getDeeplinkDataURl();" enable Deeplinking.
  *
 */
    void Awake()
    {
	AdgydeManager.SharedInstance.Adgyde_Init("Your_App_Key", "Organic");

        AdgydeManager.SharedInstance.setCurrentScreen("Home_Page"); //Custom User Flow

        AdgydeManager.SharedInstance.OnImeiPermission(true);
    }

    // Use this for initialization
    void Start()
    {
        DebugLog.text = "In Start Method";
        Debug.Log("UNITY : In Start Method");

        AdgydeManager.SharedInstance.CallOnStart();

        StartCoroutine(CallDelay(75.0f));
        //call this method when app get FCM itself
        Sample_Init_Firebase();

        //Call below method when using AdGyde default FCM OR call AdGyde FIID service directly in app Manifest.xml
        //AdgydeManager.SharedInstance.callAdGydeFcmService();

    }

    IEnumerator StartFCMService(float waitMin)
    {
        yield return new WaitForSeconds(waitMin);
        Sample_Init_Firebase();
    }

    IEnumerator StartFCMWithoutCAll(float waitMin)
    {
        yield return new WaitForSeconds(waitMin);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Sample_Init_Firebase()
    {
        Firebase.Messaging.FirebaseMessaging.TokenReceived += OnTokenRecieved;
        Firebase.Messaging.FirebaseMessaging.MessageReceived += OnMessageRecieved;
    }

    /* 
      * AdGyde's Uninstall Tracking functionality allows you to track the number of uninstalls for your application.
      * For the uninstall functionality to work AdGyde requires the FCM token.
      * 
      * Application can pass the FCM token directly to AdGyde by calling AdGyde's "com.adgyde.android.FIIDService" Service in Manifest file or else
      *
      * If application has multiple InstanceIDListener services, then the same can be passed using application's pre-existing Listener 
      * Just pass the Token to AdGyde's API through the application's pre-existing Listener
      *
      * Syntax :-  AdgydeManager.SharedInstance.onFcmTokenRecieved(token.Token.ToString());
      */
    public void OnTokenRecieved(object sender, Firebase.Messaging.TokenReceivedEventArgs token)
    {
        AdgydeManager.SharedInstance.Token = token.Token.ToString();

        //fcm TOKEN PASS TO AdGyde when app get it.
        AdgydeManager.SharedInstance.onFcmTokenRecieved(token.Token.ToString());
        StartCoroutine(CallDelay(45.0f));
    }

    public void OnMessageRecieved(object sender, Firebase.Messaging.MessageReceivedEventArgs e)
    {
        Debug.Log("Recieved a new message from : " + e.Message.From);
    }

    public void Init_Method()
    {
        DebugLog.text = "In Init Method";

        AdgydeManager.SharedInstance.CallOnStart();
        // Sample_Init_Firebase();
        //StartCoroutine(CallDelay (7.5f));

        DebugLog.text = "In Init Method After CallDelay";
    }

    IEnumerator StartDelay(float waitMin)
    {
        yield return new WaitForSeconds(waitMin);
        Init_Method();
    }

    IEnumerator CallDelay(float waitTime)
    {
        //DebugLog.text = "UNITY : In delay Method ";
        yield return new WaitForSeconds(waitTime);
        AdgydeManager.SharedInstance.getUserID();
        //DebugLog.text = AdgydeManager.SharedInstance.UserID;

        DebugLog.text = AdgydeManager.SharedInstance.getdDldata();
    }


    /*  
         * Simple Event
         * =============
         * The below code is the example to pass a simple event to the AdGyde SDK.
         * This event requires only 1 Parameter which is the Event ID.
         * 
         * NOTE : Creating the Simple Event on Console with Event ID is Compulsory
         *
         */
    public void SimpleEvent_Method()
    {
        AdgydeManager.SharedInstance.SimpleEvent("simpleEvent");
        //AdgydeManager.SharedInstance.SetUserId("ADGY100208");
        DebugLog.text = "In Simple Event Method";
    }


    /* 
    * Counting Event
    * =============
    * The below code is the example to pass a Counting event to the AdGyde SDK.
    * This event is used to get Sub-Category Counting values.
    * Multiple values Can be passed for getting counted using same parameter.
    * When user passes multiple values, the console shows the counting of each value seperately
    * 
    * NOTE : Creating the Counting Event on Console with Event ID, Parameter is Compulsory
    *
    */
    public void CountingEvent_Method()
    {

        Dictionary<string, string> param = new Dictionary<string, string>();
        // Multiple values can be passed through this event and each value will be counted and displayed in panel seperately
        // Under Counting event -> News event there will be 3 Values - "local", "National", "International" showing 1 count each. 
        param.Add("Play-Type", "Quick- Play");

        // Event is triggered with EventId and Parameters prepared above, the same are passed in this function
        AdgydeManager.SharedInstance.CountingEvent("Play-Type", param);

        DebugLog.text = "In Counting Event Method";

    }

    /*  
     * Computing Event
     * =============
     * The below code is the example to pass a Computing event .
     * This event is used to get Sub-Category counting as per weightage of the Sub-Category
     * Multiple values Can be passed for getting the computed values
     * When user passes multiple values, the console shows the computed values of each value relatively
     * 
     * NOTE : Creating the Computing Event on Console with Event ID, Parameter is Compulsory
     *
     */
    public void ComputingEvent_Method()
    {
        Dictionary<string, string> param = new Dictionary<string, string>();
        // Passing a computing event is a little complex
        // First the Sub-Category needs to be specified in a Parameter + Value combination
        // Then the Weightage of the Value needs to be specified in a Value + Weightage Combination
        // In below Example quiz is a Sub-Category and 1 is the Weightage of the same
        param.Add("Play-Type", "quiz");
        param.Add("quiz", "5");

        // Event is triggered with EventId and Parameters prepared above, the same are passed in this function
        AdgydeManager.SharedInstance.CountingEvent("Play-Type", param);

        DebugLog.text = "In Computing Event Method";

    }

    /* 
    * Unique Event
    * =============
    * Unique Event is useful to track event which needs to be tracked once in a time period.
    * AdGyde SDK provides Unique Events in three types:- 
    *        onDailyUnique.
    *		  onPermanentUnique.
    *		  onCustomUnique.
    * You can implement these unique events as per your need.
    * This event is useful to track event which needs to be tracked once / Uniquely in a Day.
    * Multiple values Can be passed in the Event using multiple Parameters, but Uniqueness will be as per Event ID only
    * 
    * 
    * NOTE : Creating the Unique Event on Console with Event ID, Parameter is Compulsory
    *
    */

    public void DailyUnique_Method()
    {
        Dictionary<string, string> param = new Dictionary<string, string>();
        // The paramter being passed in unique event are in combination of ParamterName and Value same as shown below
        // param.put( paramName, valueName );
        param.Add("DailyUniqueEvent", "DailyUniqueEvent");

        // Event is triggered with EventId and Parameters prepared above, the same are passed in this function
        AdgydeManager.SharedInstance.DailyUniqueEvent("DailyUniqueEvent", param);

        DebugLog.text = "In Daily Unique Event Method";
    }

    /*
     * Permanent Unique Event
     * =====================
     * Permanent Unique event allows you to keep a event unique for user lifetime. 
     * In case you want to find out how many Unique users clicked on Article page in app lifetime, then you can use this event
     */

    public void PermanentUnique_Method()
    {
        Dictionary<string, string> param = new Dictionary<string, string>();
        // The paramter being passed in unique event are in combination of ParamterName and Value same as shown below
        // param.put( paramName, valueName );
        param.Add("PermanentUniqueEvent", "PermanentUniqueEvent");

        // Event is triggered with EventId and Parameters prepared above, the same are passed in this function
        AdgydeManager.SharedInstance.PermanentUniqueEvent("PermanentUniqueEvent", param);

        DebugLog.text = "In Permanent Event Method";
    }

    /* Custom Unique Event
     * ===================
     * Custom Unique event allows you to keep a event unique for custom time you require. 
     * In case you want to find out how many Unique users clicked on Article page during last 72 Hours, then you can use this event
     */
    public void CustomUnique_Method()
    {
        Dictionary<string, string> param = new Dictionary<string, string>();
        // The param being passed in unique event are in combination of ParamterName and Value same as shown below
        // param.put( paramName, valueName );
        param.Add("CustomUniqueEvent", "CustomUniqueEvent");

        // Event is triggered with EventId and Parameters prepared above, the same are passed in this function
        // The third parameter is time in hours where you need to put the hour.
        // Track this Custom Unique events counts on hourly basis. 
        AdgydeManager.SharedInstance.CustomUniqueEvent("CustomUniqueEvent", param, 2);

        DebugLog.text = "In Custom Unique Event Method";

    }

    /* 
	 * Revenue Event
	 * =============
     * The below code is the example to pass a Revenue event to the AdGyde SDK.
	 * This event is useful to track revenue generated by the user in-app.
	 * Unit of the currency need not be passed, by default revenue is calculated in INR (Indian Rupees)
	 * 
	 * NOTE : There is no Need to create the Revenue Event on Console
	 *
	 */
    public void Revenue_Method()
    {
        // Revenue Event only requires the Revenue Value to be passed
        AdgydeManager.SharedInstance.OnRevenue(5);

        DebugLog.text = "In Revenue Event Method";


    }

    /* Demogarphy
     * ========== 
     * AdGyde demography data provides details of Age and Gender wise segregation of Users.
     * This data needs to be passed by Applictaion to show the same in the console
     */

    /*
	* Age data can be passed to SDK by following functions which are shown in below code:-
	*
	* Syntax Type 2 :- AdgydeManager.SharedInstance.OnsetAge(Your age);;
	*
	*/

    public void setAge()
    {
        // Set Age Value in this function
        AdgydeManager.SharedInstance.OnsetAge(20);

        DebugLog.text = "setAge Click";
    }

    /* Gender value can be passed to the SDK using the below function.
				 * Only the below 3 Values can be passed to the function for Gender
				 * Male (M)
				 * Female (F)
				 * Others (O)
				 *
				 * Syntax :-  AdgydeManager.SharedInstance.OnsetGender("M");
				 * 
				 */

    public void setGender()
    {
        AdgydeManager.SharedInstance.OnsetGender("M");
        DebugLog.text = "setGender Click";
    }


    public void UserId()
    {
        AdgydeManager.SharedInstance.OnSetUserId("ADG1045984");
        DebugLog.text = "Set userID";
    }


    /* Deeplinking Data.
        * If you want deeplinking data then call the below method get the data into String.
		*/
    public void getDeeplinkData()
    {
        string Data = AdgydeManager.SharedInstance.getdDldata();
    }
}
