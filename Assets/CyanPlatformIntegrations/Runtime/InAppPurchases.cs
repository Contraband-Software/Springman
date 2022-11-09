using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlatformIntegrations
{
    using System;
    using UnityEngine.Events;
    using UnityEngine.Purchasing;
    using Unity.Services.Core;
    using Unity.Services.Core.Environments;

    public class InAppPurchases : MonoBehaviour, IStoreListener
    {
        const string logDecorator = "IAP: ";

#region EVENTS

        public class OnInitialize : UnityEvent<bool> { }
        [HideInInspector] public static OnInitialize OnInitializeEvent;
        //public class OnPurchaseSuccess : UnityEvent<PurchaseEventArgs> { }
        //[HideInInspector] private static OnPurchaseSuccess OnPurchaseSuccessEvent;
        //public class OnPurchaseFail : UnityEvent<Product, PurchaseFailureReason> { }
        //[HideInInspector] private static OnPurchaseFail OnPurchaseFailEvent;

#endregion

        //store
        private IStoreController controller;
        private IExtensionProvider extensions;

        private static Dictionary<string, Func<bool, PurchaseFailureReason, PurchaseEventArgs, PurchaseProcessingResult>> purchaseProcessingCallbacks;

        private static string environment = "production";

        private bool available = false;

        public static bool FirstInit { get; private set; } = false;

        private void Awake()
        {
            Debug.Log(FirstInit);
            if (!FirstInit)
            {
                InitState();
            }

            //UNITY GAMING SERVICES
            InitializeServices();
        }

        private async void InitializeServices()
        {
            try
            {
                var options = new InitializationOptions()
                    .SetEnvironmentName(environment);

                await UnityServices.InitializeAsync(options);

                Debug.Log(logDecorator + "INITALIZED UNITY GAMING SERVICES");

                InitializeIAP();
            }
            catch (Exception exception)
            {
                Debug.Log(logDecorator + "FAILED TO INITIALIZE UNITY GAMING SERVICES, IAP ABORTED: " + exception.ToString());
            }
        }
        private void InitializeIAP()
        {
            ConfigurationBuilder builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

            //Use the codeless IAP catalogue
            ProductCatalog pc = ProductCatalog.LoadDefaultCatalog();
            foreach (ProductCatalogItem item in pc.allProducts)
            {
                builder.AddProduct(item.id, item.type);
            }

            UnityPurchasing.Initialize(this, builder);
        }

#region ISTORELISTENER_IMPLEMENTATION

        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            this.controller = controller;
            this.extensions = extensions;

            available = true;

            OnInitializeEvent.Invoke(true);
        }

        public void OnInitializeFailed(InitializationFailureReason error)
        {
            OnInitializeEvent.Invoke(false);
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
        {
            //OnPurchaseSuccessEvent.Invoke(purchaseEvent);
            Debug.Log(logDecorator + "[" + purchaseProcessingCallbacks.Count.ToString() + "] Purchase processing: " + purchaseEvent.purchasedProduct.definition.id);
            return purchaseProcessingCallbacks[purchaseEvent.purchasedProduct.definition.id](true, PurchaseFailureReason.Unknown, purchaseEvent);//PurchaseProcessingResult.Complete;
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            //OnPurchaseFailEvent.Invoke(product, failureReason);
            Debug.Log(logDecorator + "Purchase could not be processed: " + product.definition.id);
            purchaseProcessingCallbacks[product.definition.id](false, failureReason, null);
        }

#endregion

#region PUBLIC_INTERFACE

        /// <summary>
        /// Initializes the public interface, independant of the IAP internals
        /// </summary>
        public void InitState()
        {
            purchaseProcessingCallbacks = new Dictionary<string, Func<bool, PurchaseFailureReason, PurchaseEventArgs, PurchaseProcessingResult>>();

            OnInitializeEvent = new OnInitialize();
            OnInitializeEvent.AddListener((bool status) =>
            {
                Debug.Log(logDecorator + "Initialized: " + status.ToString());
            });
            //OnPurchaseSuccessEvent = new OnPurchaseSuccess();
            //OnPurchaseSuccessEvent.AddListener((PurchaseEventArgs args) =>
            //{
            //    Debug.Log(logDecorator + "Successfully purchased: " + args.purchasedProduct.definition.id);
            //});
            //OnPurchaseFailEvent = new OnPurchaseFail();
            //OnPurchaseFailEvent.AddListener((Product product, PurchaseFailureReason reason) =>
            //{
            //    Debug.Log(logDecorator + "Failed to purchase: " + product.definition.id + ": " + reason.ToString());
            //});

            Debug.Log(logDecorator + "Initialized Internal State");
            FirstInit = true;
            Debug.Log(FirstInit);
        }

        /// <summary>
        /// Returns if Unity gaming services and Purchasing has initialized
        /// </summary>
        /// <returns></returns>
        public bool IsAvailible()
        {
            return available;
        }

        /// <summary>
        /// Registers a function to handle the purchase for a specific product ID, call this on any script handling a purchase for a product in Start().
        /// </summary>
        /// <param name="productID"></param>
        /// <param name="callback">This function will handle both successful purchases and failiures: bool success?, PurchaseFailureReason (Unknown if successful), PurchaseEventArgs AllTheDataYouNeedAboutTheProduct</param>
        /// <exception cref="Exception">If the function fails to add to the dictionary for any reason</exception>
        public void RegisterPurchaseProcessor(string productID, Func<bool, PurchaseFailureReason, PurchaseEventArgs, PurchaseProcessingResult> callback)
        {
            if (!purchaseProcessingCallbacks.ContainsKey(productID))
            {
                if (!purchaseProcessingCallbacks.TryAdd(productID, callback))
                {
                    throw new Exception(logDecorator + "PURCHASE PROCESSING CALLBACK FAILED TO REGISTER");
                } else
                {
                    Debug.Log(logDecorator + "[" + purchaseProcessingCallbacks.Count.ToString() + "] Registered callback: " + purchaseProcessingCallbacks.ToString());
                }
            } else
            {
                //Debug.Log(logDecorator + "PURCHASE PROCESSING CALLBACK ALREADY REGISTERED");
            }
        }

        /// <summary>
        /// YOU MUST REGISTER A PURCHASE PROCESSOR CALLBACK FOR ANY PRODUCT ID USED WITH THIS METHOD.
        /// </summary>
        /// <param name="productID">The product ID defined in the IAP catalogue</param>
        /// <returns>If the purchase could be initiated</returns>
        public bool InitiatePurchase(string productID)
        {
            if (available)
            {
                controller.InitiatePurchase(productID);
                return true;
            } else
            {
                return false;
            }
        }

#endregion
    }
}