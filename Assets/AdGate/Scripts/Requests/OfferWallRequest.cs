using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AdGate;

namespace AdGateRequest
{
    public class OfferWallRequest
    {
        public static void LoadOfferWall(UniWebView uniWebView, string wallCode, string userId, Action onOfferWallLoadSuccess = null, Action<int> onOfferWallLoadFailed = null, List<string> subIds = null)
        {
            if (AdGateConfig.current.debugMode && subIds != null)
            {
                string allsubids = "";
                for (int i = 0; i < subIds.Count; i++)
                    allsubids += subIds[i] + ",";
                AdGateManager.DebugMessage(string.Format("Loading offerwall for wallcode: {0} + userId: {1} and subIds: {2}", wallCode, userId, allsubids));
            }
            string url = AdGateConfig.current.GetOfferWallUrlWithParams(wallCode, userId, subIds);
            uniWebView.Load(url);
            uniWebView.OnPageFinished += (view, statusCode, url) =>
            {
                AdGateManager.DebugMessage("Finished Loading offerwall");
                onOfferWallLoadSuccess?.Invoke();
                uniWebView.SetOpenLinksInExternalBrowser(true);
                uniWebView.SetBackButtonEnabled(false);
            };

            uniWebView.OnPageErrorReceived += (view, statusCode, url) =>
            {
                AdGateManager.DebugMessage(string.Format("Failed to load offerwall with code {0}", statusCode), true);
                onOfferWallLoadFailed?.Invoke(statusCode);
            };

            uniWebView.OnMessageReceived += (view, message) =>
            {
                if (message.Path.Equals("closeadgate"))
                {
                    AdGateManager.DebugMessage("Closed offerwall");
                    AdGateManager.CloseOfferWall();
                }
            };
            if (AdGateConfig.current.debugMode)
            {
                UniWebViewLogger.Instance.LogLevel = UniWebViewLogger.Level.Verbose;
            }
        }

        public static void ShowOfferWall(UniWebView uniWebViewComponent, Action<bool> onOfferWallShown = null)
        {
            AdGateManager.DebugMessage("Showing currently loaded offerwall");
            uniWebViewComponent?.Show();
            onOfferWallShown?.Invoke(true);
        }

        public static void GetConversionDetails(MonoBehaviour couroutineStarter, string vcCode, string userId, Action<AdGateConversionResponse> onOfferWallLoadSuccess = null, Action<string, int> onOfferWallLoadFailed = null, List<string> subIds = null)
        {
            string url = AdGateConfig.current.GetConversionListUrlWithParams(vcCode, userId, subIds);
            if (AdGateConfig.current.debugMode && subIds != null)
            {
                string allsubids = "";
                for (int i = 0; i < subIds.Count; i++)
                    allsubids += subIds[i] + ",";
                AdGateManager.DebugMessage(string.Format("Getting conversion details for vcCode: {0} + userId: {1} and subIds: {2}", vcCode, userId, allsubids));
            }
            Action<AdGateDefaultResponse> onComplete = (onComplete) =>
            {
                if (onComplete.success)
                {
                    AdGateConversionResponse conversionResponse = JsonConvert.DeserializeObject<AdGateConversionResponse>(onComplete.text);
                    conversionResponse.text = onComplete.text;
                    conversionResponse.status = onComplete.status;
                    conversionResponse.success = onComplete.success;
                    conversionResponse.statusCode = onComplete.statusCode;
                    onOfferWallLoadSuccess?.Invoke(conversionResponse);
                }
                else
                {
                    onOfferWallLoadFailed?.Invoke(onComplete.Error, onComplete.statusCode);
                }
            };
            HttpClient.CallAPI(couroutineStarter, url, AdGateHTTPMethod.GET, "", onComplete);
        }
    }
}