﻿using System;
using System.Collections.Generic;
using UnityEngine;
using VoxelBusters.CoreLibrary;
using VoxelBusters.CoreLibrary.NativePlugins;

namespace VoxelBusters.EssentialKit
{
    public class EssentialKitSettings : SettingsObject
    {
        #region Static fields

        private     static      EssentialKitSettings    s_sharedInstance;

        private     static      UnityPackageDefinition  s_package;

        #endregion

        #region Fields

        [SerializeField]
        private     ApplicationSettings                 m_applicationSettings           = new ApplicationSettings();

        [SerializeField]
        private     AddressBookUnitySettings            m_addressBookSettings           = new AddressBookUnitySettings();

        [SerializeField]
        private     NativeUIUnitySettings               m_nativeUISettings              = new NativeUIUnitySettings();

        [SerializeField]
        private     SharingServicesUnitySettings        m_sharingServicesSettings       = new SharingServicesUnitySettings();

        [SerializeField]
        private     CloudServicesUnitySettings          m_cloudServicesSettings         = new CloudServicesUnitySettings();

        [SerializeField]
        private     GameServicesUnitySettings           m_gameServicesSettings          = new GameServicesUnitySettings();

        [SerializeField]
        private     BillingServicesUnitySettings        m_billingServicesSettings       = new BillingServicesUnitySettings();

        [SerializeField]
        private     NetworkServicesUnitySettings        m_networkServicesSettings       = new NetworkServicesUnitySettings();

        [SerializeField]
        private     WebViewUnitySettings                m_webViewSettings               = new WebViewUnitySettings();

        [SerializeField]
        private     NotificationServicesUnitySettings   m_notificationServicesSettings  = new NotificationServicesUnitySettings();

        [SerializeField]
        private     MediaServicesUnitySettings          m_mediaServicesSettings         = new MediaServicesUnitySettings();

        [SerializeField]
        private     DeepLinkServicesUnitySettings       m_deepLinkServicesSettings      = new DeepLinkServicesUnitySettings();

        [SerializeField]
        private     UtilityUnitySettings                m_utilitySettings               = new UtilityUnitySettings();

        #endregion

        #region Static properties

        internal static UnityPackageDefinition Package
        {
            get
            {
                if (s_package == null)
                {
                    s_package   = new UnityPackageDefinition(
                        name: "com.voxelbusters.essentialkit",
                        displayName: "Essential Kit",
                        version: "2.5.1",
                        defaultInstallPath: $"Assets/Plugins/VoxelBusters/EssentialKit",
                        dependencies: CoreLibrarySettings.Package);
                }
                return s_package;
            }
        }

        public static string PackageName => Package.Name;

        public static string DisplayName => Package.DisplayName;

        public static string Version => Package.Version;

        public static string DefaultSettingsAssetName => "EssentialKitSettings";

        public static string DefaultSettingsAssetPath => $"{Package.GetMutableResourcesPath()}/{DefaultSettingsAssetName}.asset";

        public static string PersistentDataPath => Package.PersistentDataPath;

        public static EssentialKitSettings Instance
        {
            get { return GetSharedInstanceInternal(); }
        }

        #endregion

        #region Properties

        public ApplicationSettings ApplicationSettings
        {
            get
            {
                return m_applicationSettings;
            }
            set
            {
                m_applicationSettings   = value;
            }
        }

        public AddressBookUnitySettings AddressBookSettings
        {
            get
            {
                return m_addressBookSettings;
            }
            set
            {
                m_addressBookSettings   = value;
            }
        }

        public NativeUIUnitySettings NativeUISettings
        {
            get
            {
                return m_nativeUISettings;
            }
            set
            {
                m_nativeUISettings    = value;
            }
        }

        public SharingServicesUnitySettings SharingServicesSettings
        {
            get
            {
                return m_sharingServicesSettings;
            }
            set
            {
                m_sharingServicesSettings   = value;
            }
        }

        public CloudServicesUnitySettings CloudServicesSettings
        {
            get
            {
                return m_cloudServicesSettings;
            }
            set
            {
                m_cloudServicesSettings = value;
            }
        }

        public GameServicesUnitySettings GameServicesSettings
        {
            get
            {
                return m_gameServicesSettings;
            }
            set
            {
                m_gameServicesSettings  = value;
            }
        }

        public BillingServicesUnitySettings BillingServicesSettings
        {
            get
            {
                return m_billingServicesSettings;
            }
            set
            {
                m_billingServicesSettings   = value;
            }
        }

        public NetworkServicesUnitySettings NetworkServicesSettings
        {
            get
            {
                return m_networkServicesSettings;
            }
            set
            {
                m_networkServicesSettings   = value;
            }
        }

        public WebViewUnitySettings WebViewSettings
        {
            get
            {
                return m_webViewSettings;
            }
            set
            {
                m_webViewSettings   = value;
            }
        }

        public NotificationServicesUnitySettings NotificationServicesSettings
        {
            get
            {
                return m_notificationServicesSettings;
            }
            set
            {
                m_notificationServicesSettings  = value;
            }
        }

        public MediaServicesUnitySettings MediaServicesSettings
        {
            get
            {
                return m_mediaServicesSettings;
            }
            set
            {
                m_mediaServicesSettings     = value;
            }
        }

        public DeepLinkServicesUnitySettings DeepLinkServicesSettings
        {
            get
            {
                return m_deepLinkServicesSettings;
            }
            set
            {
                m_deepLinkServicesSettings  = value;
            }
        }

        public UtilityUnitySettings UtilitySettings
        {
            get
            {
                return m_utilitySettings;
            }
            set
            {
                m_utilitySettings = value;
            }
        }

        #endregion

        #region Static methods

        public static void SetSettings(EssentialKitSettings settings)
        {
            Assert.IsArgNotNull(settings, nameof(settings));

            // set properties
            s_sharedInstance    = settings;
        }

        private static EssentialKitSettings GetSharedInstanceInternal(bool throwError = true)
        {
            if (null == s_sharedInstance)
            {
                // check whether we are accessing in edit or play mode
                var     assetPath   = DefaultSettingsAssetName;
                var     settings    = Resources.Load<EssentialKitSettings>(assetPath);
                if (throwError && (null == settings))
                {
                    throw Diagnostics.PluginNotConfiguredException();
                }

                // store reference
                s_sharedInstance = settings;
            }

            return s_sharedInstance;
        }

        #endregion

        #region Feature methods

        public string[] GetAvailableFeatureNames()
        {
            return new string[]
            {
                NativeFeatureType.kAddressBook,
                NativeFeatureType.kBillingServices,
                NativeFeatureType.kCloudServices,
                NativeFeatureType.kGameServices,
                NativeFeatureType.kMediaServices,
                NativeFeatureType.kNativeUI,
                NativeFeatureType.kNetworkServices,
                NativeFeatureType.kNotificationServices,
                NativeFeatureType.KSharingServices,
                NativeFeatureType.kWebView,
                NativeFeatureType.kDeepLinkServices,
                NativeFeatureType.kExtras
            };
        }

        public string[] GetUsedFeatureNames()
        {
            var     usedFeatures    = new List<string>();
            if (m_addressBookSettings.IsEnabled)
            {
                usedFeatures.Add(NativeFeatureType.kAddressBook);
            }
            if (m_billingServicesSettings.IsEnabled)
            {
                usedFeatures.Add(NativeFeatureType.kBillingServices);
            }
            if (m_cloudServicesSettings.IsEnabled)
            {
                usedFeatures.Add(NativeFeatureType.kCloudServices);
            }
            if (m_gameServicesSettings.IsEnabled)
            {
                usedFeatures.Add(NativeFeatureType.kGameServices);
            }
            if (m_mediaServicesSettings.IsEnabled)
            {
                usedFeatures.Add(NativeFeatureType.kMediaServices);
            }
            if (m_nativeUISettings.IsEnabled)
            {
                usedFeatures.Add(NativeFeatureType.kNativeUI);
            }
            if (m_networkServicesSettings.IsEnabled)
            {
                usedFeatures.Add(NativeFeatureType.kNetworkServices);
            }
            if (m_notificationServicesSettings.IsEnabled)
            {
                usedFeatures.Add(NativeFeatureType.kNotificationServices);
            }
            if (m_sharingServicesSettings.IsEnabled)
            {
                usedFeatures.Add(NativeFeatureType.KSharingServices);
            }
            if (m_webViewSettings.IsEnabled)
            {
                usedFeatures.Add(NativeFeatureType.kWebView);
            }
            if (m_deepLinkServicesSettings.IsEnabled)
            {
                usedFeatures.Add(NativeFeatureType.kDeepLinkServices);
            }
            if ((usedFeatures.Count > 0) || (m_applicationSettings.RateMyAppSettings.IsEnabled))
            {
                usedFeatures.Add(NativeFeatureType.kNativeUI);//Required for showing confirmation dialog
            }
            if (m_utilitySettings.IsEnabled)
            {
                usedFeatures.Add(NativeFeatureType.kExtras);
            }

            return usedFeatures.ToArray();
        }

        public bool IsFeatureUsed(string name)
        {
            return Array.Exists(GetUsedFeatureNames(), (item) => string.Equals(item, name));
        }

        #endregion

        #region Others

        protected override void OnValidate()
        {
            base.OnValidate();
            DebugLogger.SetLogLevel(ApplicationSettings.LogLevel);
        }

        #endregion
    }
}