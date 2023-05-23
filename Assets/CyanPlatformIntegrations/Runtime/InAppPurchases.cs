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

    public class InAppPurchases : IStoreListener
    {
        #region EVENTS
        public class OnInitialize : UnityEvent<bool> { }
        public OnInitialize OnInitializeEvent { get; private set; }
        #endregion

        #region CONFIG
        const string logDecorator = Config.globalLogDecorator + "IAP System: ";
        public static string environment { get; internal set; } = "production";
        #endregion

        #region STATE
        private bool available = false;

        //store api data
        private IStoreController controller;
        private IExtensionProvider extensions;

        //individual purchase handling functions
        private Dictionary<string, Func<bool, PurchaseFailureReason, PurchaseEventArgs, PurchaseProcessingResult>> purchaseProcessingCallbacks;

        public HashSet<string> purchasedProducts { private set; get; } = new();
        #endregion

        public InAppPurchases()
        {
            InitState();

            //UNITY GAMING SERVICES
            InitializeServices();
        }

        #region PUBLIC_INTERFACE
        /// <summary>
        /// Initializes all callbacks.
        /// </summary>
        public void InitState()
        {
            purchaseProcessingCallbacks = new Dictionary<string, Func<bool, PurchaseFailureReason, PurchaseEventArgs, PurchaseProcessingResult>>();

            OnInitializeEvent = new OnInitialize();
            OnInitializeEvent.AddListener((bool status) =>
            {
                Debug.Log(logDecorator + "[STATUS] Initialized Store API: " + status.ToString());
            });

            Debug.Log(logDecorator + "[STATUS] Registered Callbacks");
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
                    throw new Exception(logDecorator + "[ERROR] PURCHASE PROCESSING CALLBACK FAILED TO REGISTER");
                }
                else
                {
                    Debug.Log(logDecorator + "[STATUS] [" + purchaseProcessingCallbacks.Count.ToString() + "] Registered callback: " + purchaseProcessingCallbacks.ToString());
                }
            }
            else
            {
                Debug.Log(logDecorator + "[WARN] PURCHASE PROCESSING CALLBACK ALREADY REGISTERED");
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
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region PRIVATE_INTERFACE
        private async void InitializeServices()
        {
            try
            {
                var options = new InitializationOptions()
                    .SetEnvironmentName(environment);

                await UnityServices.InitializeAsync(options);

                Debug.Log(logDecorator + "[STATUS] INITALIZED UNITY GAMING SERVICES");

                InitializeIAP();
            }
            catch (Exception exception)
            {
                Debug.Log(logDecorator + "[ERROR] FAILED TO INITIALIZE UNITY GAMING SERVICES, IAP ABORTED: " + exception.ToString());
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
        #endregion

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
            available = false;

            OnInitializeEvent.Invoke(false);
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
        {
            // Successfull on the API side.

            Debug.Log(logDecorator + "[STATUS] [" + purchaseProcessingCallbacks.Count.ToString() + "] Purchase processing: " + purchaseEvent.purchasedProduct.definition.id);

            PurchaseProcessingResult res = purchaseProcessingCallbacks[purchaseEvent.purchasedProduct.definition.id]
                (true, PurchaseFailureReason.Unknown, purchaseEvent);

            if (res == PurchaseProcessingResult.Complete)
            {
                purchasedProducts.Add(purchaseEvent.purchasedProduct.definition.id);
            }

            return res;
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            Debug.Log(logDecorator + "[ERROR] Purchase could not be processed: " + product.definition.id);

            purchaseProcessingCallbacks[product.definition.id](false, failureReason, null);
        }
        #endregion
    }
}