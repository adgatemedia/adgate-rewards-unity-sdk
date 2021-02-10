using AdGate;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public enum AdGateHTTPMethod
{
    GET = 0,
    POST = 1,
    DELETE = 2,
    PUT = 3,
    HEAD = 4,
    CREATE = 5,
    OPTIONS = 6,
    PATCH = 7,
    UPLOAD = 8
}

[System.Serializable]
public class AdGateDefaultResponse
{
    /// <summary>
    /// TRUE if http error OR server returns an error status
    /// </summary>
    public bool hasError;
    /// <summary>
    /// HTTP Status Code
    /// </summary>
    public int statusCode;
    /// <summary>
    /// Raw text response from the server
    /// <para>If hasError = true, this will contain the error message.</para>
    /// </summary>
    public string text;
    public string status;
    public string message;
    public string Error;
}

public class AdGateConversionResponse : AdGateDefaultResponse
{
    public AdGateConversionData data { get; set; }
}

public class AdGateConversionData
{
    public string tool_id { get; set; }
    public string user_id { get; set; }
    public AdGateConversion[] conversions { get; set; }
}

public class AdGateConversion
{
    public int offer_id { get; set; }
    public string title { get; set; }
    public int points { get; set; }
    public int payout { get; set; }
    public string s2 { get; set; }
    public string s3 { get; set; }
    public string s4 { get; set; }
    public string s5 { get; set; }
    public string tx_id { get; set; }
}

public class ConversionDetailsResponse : AdGateDefaultResponse
{

    public int offerId;
    public string title;
    public string txId;
    public double points;
    public int payout; // in cents USD
    public string subid2;
    public string subid3;
    public string subid4;
    public string subid5;
}

public class AdGateServerRequest
{
    public AdGateHTTPMethod httpMethod;
    public string url;
    public WWWForm form;
    public byte[] upload;
    public Dictionary<string, object> payload;
    public string uploadName;
    public string uploadType;
    public string jsonPayload;
    public Dictionary<string, string> extraHeaders;
    public bool useAuthToken;

    protected static Dictionary<string, string> baseHeaders = new Dictionary<string, string>() {

            { "Accept", "application/json; charset=UTF-8" },
            { "Content-Type", "application/json; charset=UTF-8" },
            { "Access-Control-Allow-Credentials", "true" },
            { "Access-Control-Allow-Headers", "Accept, X-Access-Token, X-Application-Name, X-Request-Sent-Time" },
            { "Access-Control-Allow-Methods", "GET, POST, DELETE, PUT, OPTIONS, HEAD" },
            { "Access-Control-Allow-Origin", "*" }
        };

    public UnityWebRequest CreateWebRequest()
    {
        UnityWebRequest webRequest;
        switch (httpMethod)
        {
            case AdGateHTTPMethod.UPLOAD:
                webRequest = UnityWebRequest.Post(url, form);
                break;
            case AdGateHTTPMethod.POST:
            case AdGateHTTPMethod.PATCH:
            // Defaults are fine for PUT
            case AdGateHTTPMethod.PUT:

                if (payload == null && upload != null)
                {
                    List<IMultipartFormSection> form = new List<IMultipartFormSection>
                        {
                            new MultipartFormFileSection(uploadName, upload, System.DateTime.Now.ToString(), uploadType)
                        };

                    // generate a boundary then convert the form to byte[]
                    byte[] boundary = UnityWebRequest.GenerateBoundary();
                    byte[] formSections = UnityWebRequest.SerializeFormSections(form, boundary);
                    // Set the content type - NO QUOTES around the boundary
                    string contentType = String.Concat("multipart/form-data; boundary=--", Encoding.UTF8.GetString(boundary));

                    //Debug.LogError("Content type Set: " + contentType);
                    // Make my request object and add the raw body. Set anything else you need here
                    webRequest = new UnityWebRequest();
                    webRequest.SetRequestHeader("Content-Type", "multipart/form-data; boundary=--");
                    webRequest.uri = new Uri(url);
                    Debug.Log(url);//the url is wrong in some cases
                    webRequest.uploadHandler = new UploadHandlerRaw(formSections);
                    webRequest.uploadHandler.contentType = contentType;
                    webRequest.useHttpContinue = false;

                    // webRequest.method = "POST";
                    webRequest.method = UnityWebRequest.kHttpVerbPOST;
                }
                else
                {
                    string json = (payload != null && payload.Count > 0) ? JsonConvert.SerializeObject(payload) : jsonPayload;
#if UNITY_EDITOR
                     AdGateManager.DebugMessage("REQUEST BODY = " + json);
#endif
                    byte[] bytes = System.Text.Encoding.UTF8.GetBytes(string.IsNullOrEmpty(json) ? "{}" : json);
                    webRequest = UnityWebRequest.Put(url, bytes);
                    webRequest.method = httpMethod.ToString();
                }

                break;

            case AdGateHTTPMethod.OPTIONS:
            case AdGateHTTPMethod.HEAD:
            case AdGateHTTPMethod.GET:
                // Defaults are fine for GET
                webRequest = UnityWebRequest.Get(url);
                webRequest.method = httpMethod.ToString();
                break;

            case AdGateHTTPMethod.DELETE:
                // Defaults are fine for DELETE
                webRequest = UnityWebRequest.Delete(url);
                break;
            default:
                throw new System.Exception("Invalid HTTP Method");
        }

        if (baseHeaders != null)
        {
            foreach (KeyValuePair<string, string> pair in baseHeaders)
            {
                if (pair.Key == "Content-Type" && upload != null) continue;

                webRequest.SetRequestHeader(pair.Key, pair.Value);
            }
        }

        if (extraHeaders != null)
        {
            foreach (KeyValuePair<string, string> pair in extraHeaders)
            {
                webRequest.SetRequestHeader(pair.Key, pair.Value);
            }
        }

        return webRequest;
    }
}
