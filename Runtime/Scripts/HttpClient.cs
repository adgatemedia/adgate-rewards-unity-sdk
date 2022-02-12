using AdGate;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace AdGateRequest
{
    public class HttpClient : MonoBehaviour
    {
        public static void CallAPI(MonoBehaviour monoBehaviour, string url, AdGateHTTPMethod httpMethod, string payload = null, Action<AdGateDefaultResponse> onComplete = null, bool useAuthToken = false)
        {
            AdGateServerRequest serverRequest = new AdGateServerRequest { url = url, httpMethod = httpMethod, jsonPayload = payload, useAuthToken = useAuthToken };
            monoBehaviour.StartCoroutine(SendRequest(serverRequest, onComplete));
        }

        public static IEnumerator SendRequest(AdGateServerRequest request, System.Action<AdGateDefaultResponse> OnServerResponse = null)
        {
            //Always wait 1 frame before starting any request to the server to make sure the requesters code has exited the main thread.
            yield return null;

            //Build the URL that we will hit based on the specified endpoint, query params, etc
            string url = request.url;


#if UNITY_EDITOR
            AdGateManager.DebugMessage("ServerRequest URL: " + url);
#endif

            using (UnityWebRequest webRequest = request.CreateWebRequest())
            {
                webRequest.downloadHandler = new DownloadHandlerBuffer();

                float startTime = Time.time;
                float maxTimeOut = 5f;

                yield return webRequest.SendWebRequest();
                while (!webRequest.isDone)
                {
                    yield return null;
                    if (Time.time - startTime >= maxTimeOut)
                    {
                        AdGateManager.DebugMessage("ERROR: Exceeded maxTimeOut waiting for a response from " + request.httpMethod.ToString() + " " + url);
                        yield break;
                    }
                }

                if (!webRequest.isDone)
                {
                    OnServerResponse?.Invoke(new AdGateDefaultResponse() { statusCode = 408, Error = "{\"error\": \"" + request.url + " Timed out.\"}" });
                    yield break;
                }

                try
                {
#if UNITY_EDITOR
                    AdGateManager.DebugMessage("Server Response: " + request.httpMethod + " " + request.url + " completed in " + (Time.time - startTime).ToString("n4") + " secs.\nResponse: " + webRequest.downloadHandler.text);
#endif
                }
                catch
                {
                    AdGateManager.DebugMessage(request.httpMethod.ToString(), true);
                    AdGateManager.DebugMessage(request.url, true);
                    AdGateManager.DebugMessage(webRequest.downloadHandler.text, true);
                }

                AdGateDefaultResponse response = new AdGateDefaultResponse();
                response.statusCode = (int)webRequest.responseCode;

                bool isError = false;

#if UNITY_2020_2_OR_NEWER
                isError = webRequest.result == UnityWebRequest.Result.ProtocolError || webRequest.result == UnityWebRequest.Result.ConnectionError;
#else
                isError = webRequest.isHttpError || webRequest.isNetworkError;
#endif

                if (isError || !string.IsNullOrEmpty(webRequest.error))
                {
                    switch (webRequest.responseCode)
                    {
                        case 200:
                            response.Error = "";
                            break;
                        case 400:
                            response.Error = "Bad Request -- Your request has an error";
                            break;
                        case 402:
                            response.Error = "Payment Required -- Payment failed. Insufficient funds, etc.";
                            break;
                        case 401:
                            response.Error = "Unauthroized -- Your session_token is invalid";
                            break;
                        case 403:
                            response.Error = "Forbidden -- You do not have access";
                            break;
                        case 404:
                            response.Error = "Not Found";
                            break;
                        case 405:
                            response.Error = "Method Not Allowed";
                            break;
                        case 406:
                            response.Error = "Not Acceptable -- Purchasing is disabled";
                            break;
                        case 409:
                            response.Error = "Conflict -- Your state is most likely not aligned with the servers.";
                            break;
                        case 429:
                            response.Error = "Too Many Requests -- You're being limited for sending too many requests too quickly.";
                            break;
                        case 500:
                            response.Error = "Internal Server Error -- We had a problem with our server. Try again later.";
                            break;
                        case 503:
                            response.Error = "Service Unavailable -- We're either offline for maintenance, or an error that should be solvable by calling again later was triggered.";
                            break;
                    }
#if UNITY_EDITOR
                    AdGateManager.DebugMessage("Response code: " + webRequest.responseCode);
#endif
                    response.Error += " " + webRequest.downloadHandler.text;
                    response.text = webRequest.downloadHandler.text;
                    response.success = false;
                    response.text = webRequest.downloadHandler.text;
                    OnServerResponse?.Invoke(response);

                }
                else
                {
                    response.success = true;
                    response.text = webRequest.downloadHandler.text;
                    OnServerResponse?.Invoke(response);
                }
            }

        }
    }
}