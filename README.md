# AdGate Rewards Unity SDK by AdGate Media 
Full documentation available here:
## Installation

1. Clone this repository into a local folder on your computer.
2. You will find a file named package.json in the root directory of this repository.
3. Follow the instructions here => https://docs.unity3d.com/Manual/upm-ui-local.html to install the package into your Unity project.
4. This SDK depends on the following third-party assets, if you have any of the following in your project please do not reimport them.
        1. JsonDotNet
        2. UniWebView
5. Locate the config file located in AdGate/Resources/Config/AdGateConfig.asset. You can turn debug mode on or off depending on your requirements. Debug messages will be completely ignored by the SDK when this is turned off.
6. (You may choose to install this package with the git url of this repository. However, this will make it impossible to open the ExampleScene).

## Using SDK
To start using the SDK, You need to implement usage of the “AdGate” namespace by adding this above the script.

using AdGate;

Now you can use any of the SDK methods.

### Load Offer Wall
``` c#
AdGateManager.LoadOfferWall(string wallCode, string userId, List<string> subIds = null, Action onOfferWallLoadedSuccess = null, Action<int> onOfferWallLoadingFailed = null);
```

### Example Code:

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

### Parameters:
#### wallCode: A string that corresponds to the ID of your offer wall, which you can retrieve from [this page.](https://panel.adgatemedia.com/affiliate/vc-walls)
#### userid: A string that corresponds to the current user accessing the wall
#### subIds: A list of subids you wish to use. They may be any string up to 255 characters long. The available sub ids are s2, s3, s4, and s5.
#### onOfferWallLoadedSuccess: A C# delegate action that is called as soon as the offerwall loading succeeded. It does not pass any variable back as it assumes anytime you get this callback, the offerwall loading was successful.
#### onOfferWallLoadingFailed: A C# delegate action with an error code variable. If this is ever fired, the error code can tell you more about what has happened. You can get more information about the meaning of the codes [here.](https://developer.android.com/reference/android/webkit/WebViewClient) 

### Show OfferWall
``` c#
AdGateManager.ShowOfferWall(Action onOfferWallShown = null, Action onOfferWallClosed = null);
```
### Example Code:

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
3.	AdGateManager.GetConversionDetails(string wallCode, string userId, List<string> subIds = null, Action<ConversionResponse> onConversionDetailsAvailable = null, Action<string, int> onConversionDetailsFailedToLoad = null);
```
### Example Code:

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

#### wallCode: A string that corresponds to the ID of your offer wall
#### userid: A string that corresponds to the current user accessing the wall
#### subIds: A list of subids you wish to use. They may be any string up to 255 characters long. The available sub ids are s2, s3, s4, and s5
#### onConversionDetailsAvailable: A C# delegate action that is called as soon as conversion details are received from the server. It passes out a variable of ConversionResponse type. 

#### Response Class Structure:

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
You can download an Example project from [SDK github page.](https://github.com/adgatemedia/adgate-rewards-unity-sdk/releases/download/v1.0.0/AdGateExampleProjectV1.0.0.unitypackage) In this project you would find some example code on how to use the SDK. 

## Updating the SDK
Please follow the same installation guide above but remember to backup and restore AdGate/Resources/Config/AdGateConfig.asset. Otherwise, your config choice will be lost.
Please also remember that if you made prior changes to the SDK code. It will be overwritten by the new code.
