# AdGate Rewards Unity SDK by AdGate Media 
Full documentation available here:
## Installation
1. Download the latest Unity Package Release from https://github.com/adgatemedia/adgate-rewards-unity-sdk/releases/download/v1.0.0/AdGateSDKV1.0.0.unitypackage
2. Double-click on the package or drag the package into your Unity scene to import it.
3. This SDK depends on the following third-party assets, if you have any of the following in your project please do not reimport them.
        1. JsonDotNet
        2. UniWebView
6. Click import to complete the SDK installation.
7. Locate the config file located in AdGate/Resources/Config/AdGateConfig.asset. You can turn debug mode on or off depending on your requirements. Debug messages will be completely ignored by the SDK when this is turned off.

## Using SDK
You can see some example code in the AdGate/Examples folder. This documentation explains this a bit more. 
To start using the SDK, You need to implement usage of the “AdGate” namespace by adding this above the script.
using AdGate;
Once this is done, you can now use any of the methods available. 
You have 3 major methods:
``` c#
1.	AdGateManager.LoadOfferWall(string wallCode, string userId, List<string> subIds = null, Action onOfferWallLoadedSuccess = null, Action<int> onOfferWallLoadingFailed = null);
```

A good way to use this is by writing the following code:

``` c#
public string wallCode;
public string userId;
public List<string> subIds = new List<string>();

AdGateManager.LoadOfferWall(wallCode, userId, subIds,
() =>
{
        offerWallStatus.text = "Offers Loaded";
        offerWallLoaded = true;
        showOfferBtn.interactable = true;
},
(code) =>
{
        loadOfferBtn.interactable = true;
        showOfferBtn.interactable = false;
        offerWallStatus.text = "Offer loading failed with errorcode " + code;
});
```

### Parameters Required:
#### wallCode:
#### userid:
#### subIds:
#### onOfferWallLoadedSuccess: A C# delegate action that is called as soon as the offerwall loading succeeded. It does not pass any variable back as it assumes anytime you get this callback, the offerwall loading was successful.
#### onOfferWallLoadingFailed: A C# delegate action with an error code variable. If this is ever fired, the error code can tell you more about what has happened. You can get more information about the meaning of the codes here https://developer.android.com/reference/android/webkit/WebViewClient 
``` c#
2.	AdGateManager.ShowOfferWall(Action onOfferWallShown = null, Action onOfferWallClosed = null);
```
A good way to use this is by writing the following code:

``` c#
AdGateManager.ShowOfferWall(() =>
{
        offerWallStatus.text = "Offer was shown";
},
() =>
{
        showOfferBtn.interactable = false;
        offerWallStatus.text = "No more offers, Please load some more";
});
```

onOfferWallShown: A C# delegate that is fired once the offerwall has been displayed to the user.
onOfferWallClosed: A C# delegate that is fired once the offerwall has been closed by the user.
``` c#
3.	AdGateManager.GetConversionDetails(string vcCode, string userId, List<string> subIds = null, Action<ConversionResponse> onConversionDetailsAvailable = null, Action<string, int> onConversionDetailsFailedToLoad = null);
```
A good way to use this is by writing the following code:

``` c#
AdGateManager.GetConversionDetails(wallCode, userId,
subIds, (response) =>
{
     if (response.success)
     {
        conversionBtn.interactable = true;
        conversionDetails.text = response.text;
      }
},
(message, code) =>
{
        conversionDetails.text = "Conversion Details failed with error " + message;
        conversionBtn.interactable = true;
});
```

#### vcCode:
#### userid:
#### subIds:
#### onConversionDetailsAvailable: A C# delegate action that is called as soon as conversion details are received from the server. It passes out a variable of ConversionResponse type. 
Here is a sample of the class structure:

``` c#    
public class ConversionResponse : AdGateDefaultResponse
{
  public AdGateConversionData data { get; set; }
}

public class AdGateConversionData
{
  public string tool_id { get; set; }
  public string user_id { get; set; }
  public AdGateConversion[] conversions { get; set; }
}

public class AdGateDefaultResponse
{
   public int statusCode;
   public string text;
   public string status;
   public bool success;
   public string message;
   public string Error;
 }
```
If this callback is fired, it is assumed that connection to the server was successful. However, the conversions variable can be null or an empty array if there is no conversion available at the moment the server request was made.

onConversionDetailsFailedToLoad: A C# delegate action with an error and error code variable. If this is ever fired, the error code can tell you more about what has happened. This would be a standard server error code, while the error message would be any message sent from the server.
## Example Project
You can download an Example project from https://github.com/adgatemedia/adgate-rewards-unity-sdk/releases/download/v1.0.0/AdGateExampleProjectV1.0.0.unitypackage , In this project you would find some example code on how to use the SDK. 

## Updating the SDK
Please follow the same installation guide above but remember to Untick AdGate/Resources/Config/AdGateConfig.asset from being imported. Otherwise, your config choice will be lost.
Please also remember that if you made prior changes to the SDK code. It will be overwritten by the new code. 
