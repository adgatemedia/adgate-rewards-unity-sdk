using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AdGate;

namespace AdGateExample
{
    public class OfferWallExample : MonoBehaviour
    {
        bool offerWallLoaded;
        public Text offerWallStatus;
        public Text conversionDetails;
        public Button conversionBtn;
        public Button loadOfferBtn;
        public Button showOfferBtn;
        public string wallCode;
        public string userId;
        public List<string> subIds = new List<string>();

        void Start()
        {
            LoadOfferWall();
            showOfferBtn.interactable = loadOfferBtn.interactable = false;
        }

        public void ShowOfferWall()
        {
            conversionDetails.text = "";
            offerWallStatus.text = "Showing Offer";
            if (offerWallLoaded)
            {
                AdGateManager.ShowOfferWall(onOfferWallClosed: () =>
                 {
                     showOfferBtn.interactable = false;
                     offerWallStatus.text = "No more offers, Please load some more";
                 });
            }
            loadOfferBtn.interactable = true;
        }

        public void LoadOfferWall()
        {
            loadOfferBtn.interactable = false;
            offerWallLoaded = false;
            conversionDetails.text = "";
            offerWallStatus.text = "Loading";
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
        }

        public void GetConversionDetails()
        {
            conversionBtn.interactable = false;
            conversionDetails.text = "Loading Details";
            AdGateManager.GetConversionDetails(wallCode, userId,
            null, (response) =>
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
        }
    }
}