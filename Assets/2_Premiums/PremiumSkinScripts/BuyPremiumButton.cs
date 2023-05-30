using System.Collections;
using System.Collections.Generic;
using PlatformIntegrations;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.Purchasing;
using Architecture.Managers;

public class BuyPremiumButton : MonoBehaviour
{
    public string productID { set; get; } = "";
    IntegrationsManager integrationsManager;
    [SerializeField] SkinSelector_Premium skinSelectorPremium;
    [SerializeField] SkinsController skinsController;
    [SerializeField] CosmeticsMenuController cosMenuCon;

    [Header("Closing Buy Panel On Purchase")]
    public UnityEvent onPurchaseCloseTab = new UnityEvent();

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

                    //refresh the page
                    print("Refreshing Premium Page...");
                    UserGameData.Instance.currentSkinPremium = true;
                    print("new active premium will be: " + InAppPurchases.ProductIDToTitle(item.id));
                    UserGameData.Instance.activePremiumSkinName = InAppPurchases.ProductIDToTitle(item.id);
                    print("active premium set to: " + UserGameData.Instance.activePremiumSkinName);
                    UserGameData.Instance.currentSkin = item.id;
                    skinsController.currentSkinID = item.id;
                    skinSelectorPremium.RemoveLockIconOnOwnedSkins();
                    skinsController.OpenNewTab("premium");
                    onPurchaseCloseTab.Invoke();
                    cosMenuCon.UpdateDemo();

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
        integrationsManager.iapHandler.StartPurchase(productID);
    }
}
