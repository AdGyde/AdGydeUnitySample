

<img src="https://www.adgyde.com/img/logo.png"  width="75">


# AdGyde - Unity Android Integration
----
In case you face any issue or need any kind of clarification / understanding, We would request you to Kindly contact our support team at support@adgyde.com

# Overview

AdGyde is a Mobile Application Attribution tool which helps application owners / marketers in better advertisement targeting and in optimizing the ad expenditure. 

Attribution
AdGyde through its attribution SDK can determine which channel / partner the app install was delivered through. Post install events, User Flow and how the application is being used is shown by AdGyde. This data can help advertiser / Application owner to specifically target user segments, find out best channel to improve upon ROI.

To get started contact AdGyde support - support@adgyde.com
---

# Integration Process for AdGyde Unity Android SDK `v3.3.0`
You can also access the complete documentation from belo link as well
https://www.adgyde.com/documents.php

**NOTE : AdGyde Currently only supports Android in Unity**


## Table of content

- [Get Your App Key](#getyourkey)
- [Download Unity Android SDK](#downloadsdk)
- [Integrate SDK Into Project](#integratesdk)
   - [3.1 Add library files into your project](#addlibrary)
   - [3.2 Follow below steps to import .aar file into your projects](#importaar)
   - [3.3 Initializing PAgent](#initpagent)
   - [3.4 Embed Google Play Services into Your App](#embedplayservice)
   - [3.5 Add Install receiver code in the androidmanifest.xml](#addinstallreceiver)
   - [3.6 Add permissions to project](#addpermission)
   - [3.7 Add dependency to project](#adddependency)
- [Uninstall Tracking](#uninstalltracking)


### <a id="getyourkey">
#  Get your App Key
Sign-in to your AdGyde Console, the credentials would have already been provided by AdGyde support team. In case you have not yet received the same, please contact the AdGyde Support Team. 

Please follow the given steps :- 
- Step 1) Visit the AdGyde Website - https://www.adgyde.com
- Step 2) Go to console
- Step 3) Sign-in with your credentials
- Step 4) Go to Setup -> Applications from the menu option
- Step 5) Click on "Create an application" option on Top Right corner
- Step 6) Fill in the Application Name and Package Name
- Step 7) Note down the App Key for integration reference


### <a id="downloadsdk">
#  Download Unity SDK
Please contact AdGyde support team at support@adgyde.com to get the SDK.

Integrate the downloaded SDK using the below steps

### <a id="integratesdk">
# Integrate SDK into project

### <a id="addlibrary">
#### 3.1) Add library files into your project
 - Unzip AdGyde Android Unity SDK
 - Add ADGYDE_ANDROID_Unity_SDK file to your Unity project

### <a id="importaar">
#### 3.2) Follow below steps to import .aar file into your projects 
 - Open Unity Project and switch platform to Android
 - Click Assets —> Import Package —> Custom Package (Adgyde Unity Package)
 - Select All files in import window and click import
 - Create an empty game object in Hierarchy tab
 - Add the AdgydeDemo.cs file to the empty game object

### <a id="initpagent">
#### 3.3) Initializing PAgent
Android Unity SDK needs to be initialized in your main Script. Please check Example project on Android Unity SDK for complete code.

```
using System.Collections; 
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class Sample_UI_Class : MonoBehaviour {
	private static Sample_UI_Class _instance = null;

	public static Sample_UI_Class SharedInstance {
		get {
			// if the instance hasnt been assigned then search for it
			if (_instance == null) {
				_instance = GameObject.FindObjectOfType(typeof(Sample_UI_Class)) as Sample_UI_Class;
		}

		return _instance;
	}
}

public Text DebugLog;

void Awake(){
	AdgydeManager.SharedInstance.Adgyde_Init();
	// initialized AdGyde
}
```

### <a id="embedplayservice">
#### 3.4) Embed Google Play Services into Your App 
Install the Google Play Services SDK and import it into your project. For download details, <a href="https://developers.google.com/android/guides/setup">Click here</a>.
Add the below code / entry to the AndroidManifest.xml as the last entry in the application tag (Just before &lt;/application&gt;)

```<meta-data android:name="com.google.android.gms.version" android:value="@integer/google_play_services_version"/>```

**NOTE:**
AdGyde recommends to always use the latest version of google play service.

For more details see the link below.
https://developer.android.com/google/play-services/setup.html
Please refer below code

```
< application
    android:icon="@drawable/app_icon"
    android:label="@string/app_name">

    <activity android:name="com.google.firebase.MessagingUnityPlayerActivity"
        android:label="@string/app_name"
        android:icon="@drawable/app_icon"
        android:configChanges="fontScale|keyboard|keyboardHidden|locale|mnc|mcc|navigation|orientation|
		        screenLayout|screenSize|smallestScreenSize|uiMode|touchscreen">

        <intent-filter>
            <action android:name="android.intent.action.MAIN" />
            <category android:name="android.intent.category.LAUNCHER" />
        </intent-filter>

        <meta-data android:name="unityplayer.UnityActivity" android:value="true" />
    </activity>

    <service 
	    android:name="com.google.firebase.messaging.MessageForwardingService"
	    android:exported="false"/>

    <service android:name="com.adgyde.android.AppJobService" 
	    android:exported="true" 
	    android:permission="android.permission.BIND_JOB_SERVICE" /> 
</application> 
```

### <a id="addinstallreceiver">
#### 3.5) Add Install receiver code in the androidmanifest.xml

**Note:** 
Please make sure that following receiver tag is kept inside application tag.

```
<receiver android:name="com.adgyde.android.InstallReceiver" android:exported="true">
	<intent-filter>
		<action android:name="com.android.vending.INSTALL_REFERRER" />
	</intent-filter>
</receiver>
```

If an application uses multiple INSTALL_REFERRER receivers, you should use com.adgyde.android.MultiInstallReceiver instead of com.adgyde.android.InstallReceiver.
MultiInstallReceiver MUST be the first receiver on Top of all the other INSTALL_REFERRER receivers

```
<receiver android:name="com.adgyde.android.MultiInstallReceiver" android:exported="true">
	<intent-filter>
		<action android:name="com.android.vending.INSTALL_REFERRER" />
	</intent-filter>
</receiver>
```

If you want to use multiple receivers, then the Manifest.xml file should look like this:

```
<!--The AdGyde Install Receiver should be placed first and it will broadcast to all receivers placed below it -->

<receiver android:name="com.adgyde.android.MultiInstallReceiver" android:exported="true">
	<intent-filter>
		<action android:name="com.android.vending.INSTALL_REFERRER" />
	</intent-filter>
</receiver> 

<!--All other receivers should be followed right after -->
<receiver android:name="com.google.android.abc.AbcReceiver" android:exported="true">
	<intent-filter>
		<action android:name="com.android.vending.INSTALL_REFERRER" />
	</intent-filter>
</receiver> 

<receiver android:name="com.adsmobi.android.xyz.InstallReceiver" android:exported="true">
	<intent-filter>
		<action android:name="com.android.vending.INSTALL_REFERRER" />
	</intent-filter>
</receiver>
```

### <a id="addpermission">
#### 3.6) Add permissions to project
Add following permissions to AndroidManifest.xml

```
<uses-permission android:name="android.permission.ACCESS_NETWORK_STATE"> 
</usespermission>
<uses-permission android:name="android.permission.ACCESS_WIFI_STATE" />
<uses-permission android:name="android.permission.INTERNET"> 
</uses-permission>
```

### <a id="adddependency">
#### 3.7) Add dependency to project
Add the following dependency to Android gradle file (Module : android).

```
dependencies {
	// . . .
	compile 'com.android.installreferrer:installreferrer:1.0'
	// . . .
}
```

### <a id="uninstalltracking">
# 4. Uninstall Tracking
AdGyde's Uninstall Tracking functionality allows you to track the number of uninstalls for a specified application. Uninstalls is an important index which helps you to track the quality of users and hence the campaign. 

<a href="https://www.adgyde.com/documents.php?topic=article11&platform=unity" >Un-Install Detailed Integration Process</a>
