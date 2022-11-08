using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

namespace PlatformIntegrations
{
    public class InAppPurchases : MonoBehaviour
    {
        private void Start()
        {
            //ProductCatalog.LoadDefaultCatalog().allProducts
            Debug.Log("IAP INIT: ");
            Debug.Log(CodelessIAPStoreListener.initializationComplete);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}