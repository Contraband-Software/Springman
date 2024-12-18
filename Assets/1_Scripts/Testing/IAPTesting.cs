using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlatformIntegrations;
using UnityEngine.Purchasing;

/*
 
an example of client usage (product purchase button) of the iap api i provide

This is currently attached to the main menu store button for testing
 
 */

#if UNITY_EDITOR
namespace Development
{
    public class IAPTesting : MonoBehaviour
    {
        IntegrationsManager integrationsManager;

        [SerializeField] string productID = "software.contraband.springman.iap.removeads";

        void Start()
        {
            integrationsManager = IntegrationsManager.Instance;

            ProductCatalog pc = ProductCatalog.LoadDefaultCatalog();
            foreach (ProductCatalogItem item in pc.allProducts)
            {
                
            }

            integrationsManager.iapHandler.RegisterPurchaseProcessor(productID,
                (bool status, PurchaseFailureReason failReason, PurchaseEventArgs args) =>
                {
                    if (status)
                    {
                        //someskin.allowedtouse = true;
                        //give the user the skin
                        Debug.Log("Fufilled User purchase of: " + productID);
                        Debug.Log("RECEIPT: " + args.purchasedProduct.receipt.ToString());

                    }
                    else
                    {
                        Debug.Log("Could not give user item due to: " + failReason.ToString());
                    }

                    return PurchaseProcessingResult.Complete;
                }
            );
        }

        //button function
        public void DoPurchase()
        {
            Debug.Log("Started purchase for: " + productID);
            integrationsManager.iapHandler.StartPurchase(productID);
        }
    }
}
#endif