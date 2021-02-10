using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AdGateRequest;

namespace AdGate
{
    public class AdGateManager : MonoBehaviour
    {
        static AdGateManager Instance;
        static GameObject webViewPrefab;
        static UniWebView uniWebViewComponent;
        static Action onOfferWallClosed;
        static string wallCodeCache;
        static string userIdCache;
        static List<string> subIdsCache;
        static bool initialized;

        static AdGateManager _Instance
        {
            get
            {
                AdGateManager manager = null;
                if (Instance == null)
                {
                    GameObject managerGO = new GameObject("AdGateManager");
                    manager = managerGO.AddComponent<AdGateManager>();
                    Instance = manager;
                    DontDestroyOnLoad(Instance.gameObject);
                }
                LoadConfig();
                return Instance;
            }
        }

        static bool LoadConfig()
        {
            if (AdGateConfig.current == null)
                AdGateConfig.current = Resources.Load("Config/AdGateConfig") as AdGateConfig;
            initialized = true;
            UniWebView.SetWebContentsDebuggingEnabled(AdGateConfig.current.debugMode);
            return initialized;
        }

        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                ReloadOfferWall();
            }
        }

        /// <summary>
        /// Call first to load the offerwall in the background, it also has callbacks to let you know once it was successfull and if it failed
        /// so you can try again.
        /// </summary>
        /// <param name="wallCode"></param>
        /// <param name="userId"></param>
        /// <param name="subIds"></param>
        /// <param name="onOfferWallLoadedSuccess"></param>
        /// <param name="onOfferWallLoadingFailed"></param>
        public static void LoadOfferWall(string wallCode, string userId, List<string> subIds = null, Action onOfferWallLoadedSuccess = null, Action<int> onOfferWallLoadingFailed = null)
        {
            CheckIntialisedState();
            wallCodeCache = wallCode;
            userIdCache = userId;
            subIdsCache = subIds;
            webViewPrefab = webViewPrefab == null ? Resources.Load<GameObject>("AdGameOfferView") : webViewPrefab;
            uniWebViewComponent = Instantiate(webViewPrefab).GetComponent<UniWebView>();
            OfferWallRequest.LoadOfferWall(uniWebViewComponent, wallCode, userId, onOfferWallLoadedSuccess, onOfferWallLoadingFailed, subIds);
        }

        static void CheckIntialisedState()
        {
            if (!initialized)
                LoadConfig();
        }

        static void ReloadOfferWall()
        {
            CheckIntialisedState();
            if (uniWebViewComponent != null)
                uniWebViewComponent.Load(AdGateConfig.current.GetOfferWallUrlWithParams(wallCodeCache, userIdCache, subIdsCache));
        }
        /// <summary>
        /// Once you have loaded and confirmed an offer is available, you can call this to display the offerwall to user. You also get callbacks
        /// for when showing offer was successfull or if it failed
        /// </summary>
        /// <param name="onOfferWallShown"></param>
        /// <param name="onOfferWallClosed"></param>
        public static void ShowOfferWall(Action onOfferWallShown = null, Action onOfferWallClosed = null)
        {
            CheckIntialisedState();
            AdGateManager.onOfferWallClosed = onOfferWallClosed;
            if (uniWebViewComponent != null)
                OfferWallRequest.ShowOfferWall(uniWebViewComponent);
            else
                onOfferWallShown?.Invoke();
        }

        public static void CloseOfferWall()
        {
            if (uniWebViewComponent != null)
            {
                Destroy(uniWebViewComponent.gameObject);
                uniWebViewComponent = null;
                onOfferWallClosed?.Invoke();
            }
        }

        /// <summary>
        /// This is used to get the conversion details for a user and code
        /// </summary>
        /// <param name="vcCode"></param>
        /// <param name="userId"></param>
        /// <param name="onConversionDetailsAvailable"></param>
        /// <param name="onConversionDetailsFailedToLoad"></param>
        /// <param name="subIds"></param>
        public static void GetConversionDetails(string vcCode, string userId, List<string> subIds = null, Action<AdGateConversionResponse> onConversionDetailsAvailable = null, Action<string, int> onConversionDetailsFailedToLoad = null)
        {
            CheckIntialisedState();
            OfferWallRequest.GetConversionDetails(_Instance, vcCode, userId, onConversionDetailsAvailable, onConversionDetailsFailedToLoad, subIds);
        }

        public static void DebugMessage(string message, bool isError = false)
        {
            if (AdGateConfig.current.debugMode)
            {
                if (isError)
                    Debug.LogError(message);
                else
                    Debug.Log(message);
            }
        }
    }
}