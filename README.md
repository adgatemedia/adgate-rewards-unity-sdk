# AdGate Rewards Unity SDK by AdGate Media 
Full documentation available here :
## Installation from SDK
1.	Download the latest Unity Package Release .
2.	 Double-click on the package or drag the package into your Unity scene to import it.
3.	This SDK depends on the following third party assets, if you have any of the following in your project please do not reimport them.
1.	JsonDotNet
2.	UniWebView
6. Click import to complete the SDK installation
7. Locate the config file located in AdGate/Config/Resources/Config. You can turn debug mode on or off depending on your requirements. Debug messages will be completely ignored by the SDK when this is turned off.
## Cloning entire project.
The project was created using Unity 2020.2.1f1, if you intend to open the project, you should use that exact Unity version. Nothing else is required for you to run the project. 
### Things to note:
1.	Scripts are in the AdGate/Scripts folder.
2.	Thirdparty Assets used in project are in the AdGate/ThirdParty folder.
3.	The Example scene is in AdGate/Examples folder
4.	There is no special installation that needs to occur before you can make changes 

### Exporting packages:
	You can follow the description available here https://docs.unity3d.com/2018.1/Documentation/Manual/HOWTO-exportpackage.html , remember to export only the AdGate folder

## Using SDK
You can see some example code in the AdGate/Examples folder. This documentation explains this a bit more. 
To start using the SDk, You need to implement usuage of the AdGate namespace by adding this above the script.
using AdGate;
Once this is done, you can now use any of the methods available. 
You have 3 major methods:
``` c#
AdGateManager.LoadOfferWall(string wallCode, string userId, List<string> subIds = null, Action onOfferWallLoadedSuccess = null, Action<int> onOfferWallLoadingFailed = null);
```
### Parameters Required:
#### wallCode:
#### userid:
#### subIds:
#### onOfferWallLoadedSuccess: A C# delegate action that is called as soon as the offerwall loading succeeded. It does not pass any variable back as it assumes anytime you get this callback, the offerwall loading was successful.
#### onOfferWallLoadingFailed: A C# delegate action with an error code variable. If this is ever fired, the error code can tell you more about what has happened. You can get more information about the meaning of the codes here https://developer.android.com/reference/android/webkit/WebViewClient 
``` c#
AdGateManager.ShowOfferWall(Action onOfferWallShown = null, Action onOfferWallClosed = null);
```
onOfferWallShown: A C# delegate that is fired once the offerwall has been displayed to the user.
onOfferWallClosed: A C# delegate that is fired once the offerwall has been closed by the user.
``` c#
AdGateManager.GetConversionDetails(string vcCode, string userId, List<string> subIds = null, Action<ConversionResponse> onConversionDetailsAvailable = null, Action<string, int> onConversionDetailsFailedToLoad = null);
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
```
If this callback is fired, it is assumed that connection to the server was successful. However, the conversions variable can be null or an empty array if there is no conversion available at the moment the server request was made.

onConversionDetailsFailedToLoad: A C# delegate action with an error and error code variable. If this is ever fired, the error code can tell you more about what has happened. This would be a standard server error code, while the error message would be any message sent from the server.
