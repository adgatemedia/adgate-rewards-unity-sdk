using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using AdGate;

namespace AdGateRequest
{
    [CreateAssetMenu(fileName = "Config", menuName = "ScriptableObjects/Config", order = 1)]
    public class AdGateConfig : ScriptableObject
    {
        public static AdGateConfig current;
        public bool debugMode = false;
        string offerWallUrlLive = "https://wall.adgaterewards.com/{0}/{1}?";
        string conversionUrl = "https://wall.adgaterewards.com/apiv1/vc/{0}/users/{1}/get_latest_conversions";
        string url;

        public string GetOfferWallUrlWithParams(string wallCode, string userId, List<string> parameters = null)
        {
            url = "";
            url += GetDefaultUrlWithParams(offerWallUrlLive, wallCode, userId, parameters);
            url += GetDefaultDeviceIdentifiers();
            return url;
        }

        public string GetConversionListUrlWithParams(string wallCode, string userId, List<string> parameters = null)
        {
            url = "";
            url += GetDefaultUrlWithParams(conversionUrl, wallCode, userId, parameters);
            return url;
        }

        string GetDefaultUrlWithParams(string defaultUrl, string wallCode, string userId, List<string> parameters = null)
        {
            string url = string.Format(defaultUrl, wallCode, userId);
            if (parameters != null)
            {
                for (int i = 0; i < parameters.Count; i++)
                {
                    url += AddKeyValueToUrl(GetPrefix() + "s" + (i + 2), RemoveSpace(parameters[i]));
                }
            }
            return url;
        }

        string GetDefaultDeviceIdentifiers()
        {
            string url = "";
            string keyName = "ios_id";
#if UNITY_IOS
        keyName = "ios_id";
#elif UNITY_ANDROID
            keyName = "android_id";
#endif
            url += AddKeyValueToUrl(keyName, RemoveSpace(SystemInfo.deviceUniqueIdentifier));

            url += AddKeyValueToUrl(GetPrefix() + "model", RemoveSpace(SystemInfo.deviceModel));
#if UNITY_IOS
        url += AddKeyValueToUrl(GetPrefix() + "mfg", "Apple");
#elif UNITY_ANDROID
            url += AddKeyValueToUrl(GetPrefix() + "mfg", "Android");
#endif
            url += AddKeyValueToUrl(GetPrefix() + "osVersion", RemoveSpace(SystemInfo.operatingSystem));

#if UNITY_IOS
        keyName = GetPrefix() +"aid";
#elif UNITY_ANDROID
            keyName = GetPrefix() + "pid";
#endif
            url += AddKeyValueToUrl(keyName, RemoveSpace(Application.identifier));
            url += AddKeyValueToUrl(GetPrefix() + "sdk", "unity");
            return url;
        }

        string RemoveSpace(string str)
        {
            return Regex.Replace(str, @"\s+", "");
        }

        string GetPrefix()
        {
            string pre = "&";
            return pre;
        }

        string AddKeyValueToUrl(string key, string value)
        {
            return key + "=" + value;
        }
    }
}