using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Manager : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

   /* 
    * Initialize AdGyde SDK with appkey & default channel id "Organic".
    * When applictaion is installed from Google Play Store without any campaign the Channel will be Organic as specified in Init    Function
    * In case the applictaion is installed through a campaign link then the Default channel will be overriden and value from the campaign link will be passed
    */
    public void Init()
    {
        AdgydeManager.SharedInstance.Adgyde_Init("Your app key","Organic");
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
    public void SimpleEvent()
    {
        AdgydeManager.SharedInstance.SimpleEvent("Click_Reward_Ads");
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
    public void CountingEvent()
    {
        Dictionary<string, string> parameter = new Dictionary<string, string>();
        // Multiple values can be passed through this event and each value will be counted and displayed in panel seperately
        // Under Counting event -> News event there will be 3 Values - "local", "National", "International" showing 1 count each. 
        parameter.Add("Play-Type", "Quick- Play");

        // Event is triggered with EventId and Parameters prepared above, the same are passed in this function
        AdgydeManager.SharedInstance.CountingEvent("Play-Type", parameter);
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
    public void ComputingEvent()
    {
        Dictionary<string, string> parameter = new Dictionary<string, string>();
        // Passing a computing event is a little complex
        // First the Sub-Category needs to be specified in a Parameter + Value combination
        // Then the Weightage of the Value needs to be specified in a Value + Weightage Combination
        // In below Example quiz is a Sub-Category and 1 is the Weightage of the same
        parameter.Add("Play-Type", "quiz");
        parameter.Add("quiz", "5");

        // Event is triggered with EventId and Parameters prepared above, the same are passed in this function
        AdgydeManager.SharedInstance.CountingEvent("Play-Type", parameter);
    }

    /* 
     * Unique Event
     * =============
     * Unique Event is useful to track event which needs to be tracked once in a time period.
     * AdGyde SDK provides Unique Events in three types:- 
     *        onDailyUnique.
     *        onPermanentUnique.
     *        onCustomUnique.
     * You can implement these unique events as per your need.
     * This event is useful to track event which needs to be tracked once / Uniquely in a Day.
     * Multiple values Can be passed in the Event using multiple Parameters, but Uniqueness will be as per Event ID only
     * 
     * 
     * NOTE : Creating the Unique Event on Console with Event ID, Parameter is Compulsory
     *
     */

    public void DailyUnique()
    {
        Dictionary<string, string> parameter = new Dictionary<string, string>();
        // The paramter being passed in unique event are in combination of ParamterName and Value same as shown below
        // param.put( paramName, valueName );
        parameter.Add("DailyUniqueEvent", "DailyUniqueEvent");

        // Event is triggered with EventId and Parameters prepared above, the same are passed in this function        
        AdgydeManager.SharedInstance.DailyUniqueEvent("onDailyUniqueEvent", parameter);
    }

   /*
    * Permanent Unique event allows you to keep a event unique for user lifetime. 
    * In case you want to find out how many Unique users clicked on Article page in app lifetime, then you can use this event
    */

    public void PermanentUnique()
    {
        Dictionary<string, string> param = new Dictionary<string, string>();
        // The paramter being passed in unique event are in combination of ParamterName and Value same as shown below
        // param.put( paramName, valueName );
        param.Add("PermanentUniqueEvent", "PermanentUniqueEvent");

        // Event is triggered with EventId and Parameters prepared above, the same are passed in this function
        AdgydeManager.SharedInstance.PermanentUniqueEvent("PermanentUniqueEvent", param);

        DebugLog.text = "In Permanent Event Method";
    }


   /*
    * Custom Unique event allows you to keep a event unique for custom time you require. 
    * In case you want to find out how many Unique users clicked on Article page during last 72 Hours, then you can use this event
    */
    public void CustomUnique()
    {
        Dictionary<string, string> param = new Dictionary<string, string>();
        // The param being passed in unique event are in combination of ParamterName and Value same as shown below
        // param.put( paramName, valueName );
        param.Add("CustomUniqueEvent", "CustomUniqueEvent");

        // Event is triggered with EventId and Parameters prepared above, the same are passed in this function
        // The third parameter is time in hours where you need to put the hour.
        // Track this Custom Unique events on hourly basis. 
        AdgydeManager.SharedInstance.CustomUniqueEvent("CustomUniqueEvent", param,2);

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
    public void OnRevenueEvent()
    {
        // Revenue Event only requires the Revenue Value to be passed
        AdgydeManager.SharedInstance.OnRevenue(5);
    }

}
