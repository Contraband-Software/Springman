using System.Collections;
using System.Collections.Generic;
using PlatformIntegrations;
using UnityEngine;
using UnityEngine.Purchasing;

public class BuyPremiumButton : MonoBehaviour
{
    public string productID { set; get; } = "";
    IntegrationsManager integrationsManager;

    private void Start()
    {
        integrationsManager = IntegrationsManager.Instance;

        ProductCatalog pc = ProductCatalog.LoadDefaultCatalog();
        foreach (ProductCatalogItem item in pc.allProducts)
        {
            integrationsManager.iapHandler.RegisterPurchaseProcessor(item.id,
            (bool status, PurchaseFailureReason failReason, PurchaseEventArgs args) =>
            {
                if (status)
                {
                    //someskin.allowedtouse = true;
                    //give the user the skin
                    Debug.Log("Fufilled User purchase of: " + item.id);
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
    }

    public void DoPurchase()
    {
        Debug.Log("Started purchase for: " + productID);
        integrationsManager.iapHandler.InitiatePurchase(productID);
    }
}
