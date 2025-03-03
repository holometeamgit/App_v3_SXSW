﻿#if UNITY_ANDROID
using System.Xml;
using VoxelBusters.CoreLibrary;
using VoxelBusters.CoreLibrary.Editor.NativePlugins.Build.Android;

namespace VoxelBusters.EssentialKit.Editor.Android
{
    public class AndroidManifestGenerator
    {
#region Static fields

        private static string s_androidLibraryRootPackageName = "com.voxelbusters.android.essentialkit";

#endregion

#region Public methods

        public static void GenerateManifest(EssentialKitSettings settings, bool addQueries = false, string savePath = null)
        {
            Manifest manifest = new Manifest();
            manifest.AddAttribute("xmlns:android", "http://schemas.android.com/apk/res/android");
            manifest.AddAttribute("xmlns:tools", "http://schemas.android.com/tools");
            manifest.AddAttribute("package", s_androidLibraryRootPackageName + "plugin");
            manifest.AddAttribute("android:versionCode", "1");
            manifest.AddAttribute("android:versionName", "1.0");

            if (addQueries) //Required from Android 11
            {
                AddQueries(manifest, settings);
            }

            /*SDK sdk = new SDK();
            sdk.AddAttribute("android:minSdkVersion", "14");
            sdk.AddAttribute("android:targetSdkVersion", "30");

            // Add sdk
            manifest.Add(sdk);*/

            Application application = new Application();

            AddActivities(application, settings);
            AddProviders(application, settings);
            AddServices(application, settings);
            AddReceivers(application, settings);
            AddMetaData(application, settings);

            manifest.Add(application);

            AddPermissions(manifest, settings, addQueries);
            AddFeatures(manifest, settings);


            XmlDocument xmlDocument = new XmlDocument();
            XmlNode xmlNode = xmlDocument.CreateXmlDeclaration("1.0", "UTF-8", null);

            // Append xml node
            xmlDocument.AppendChild(xmlNode);

            // Get xml hierarchy
            XmlElement element = manifest.GenerateXml(xmlDocument);
            xmlDocument.AppendChild(element);

            // Save to androidmanifest.xml
            xmlDocument.Save(savePath == null ? IOServices.CombinePath(EssentialKitPackageLayout.AndroidProjectPath, "AndroidManifest.xml") : savePath);
        }

#endregion

#region Private methods

        private static void AddQueries(Manifest manifest, EssentialKitSettings settings)
        {
            Queries queries = new Queries();

            if (settings.MediaServicesSettings.IsEnabled || (settings.WebViewSettings.IsEnabled && settings.WebViewSettings.AndroidProperties.UsesCamera))
            {
                Intent intent   = new Intent();
                Action action = new Action();
                action.AddAttribute("android:name", "android.media.action.IMAGE_CAPTURE");
                intent.Add(action);
                queries.Add(intent);
            }

            if (settings.GameServicesSettings.IsEnabled)
            {
                Intent intent = new Intent();
                Action action = new Action();
                action.AddAttribute("android:name", "com.google.android.gms.games.VIEW_LEADERBOARDS");
                intent.Add(action);
                queries.Add(intent);

                intent = new Intent();
                action = new Action();
                action.AddAttribute("android:name", "com.google.android.gms.games.VIEW_ACHIEVEMENTS");
                intent.Add(action);
                queries.Add(intent);
            }

            if (settings.SharingServicesSettings.IsEnabled)
            {
                Intent intent = new Intent();
                Action action = new Action();
                action.AddAttribute("android:name", "android.intent.action.SEND");
                intent.Add(action);
                queries.Add(intent);

                intent = new Intent();
                action = new Action();
                action.AddAttribute("android:name", "android.intent.action.SENDTO");
                intent.Add(action);
                queries.Add(intent);

                intent = new Intent();
                action = new Action();
                action.AddAttribute("android:name", "android.intent.action.SEND_MULTIPLE");
                intent.Add(action);
                queries.Add(intent);
            }

            if (settings.WebViewSettings.IsEnabled || (settings.MediaServicesSettings.IsEnabled && settings.MediaServicesSettings.UsesGallery))
            {
                Intent intent = new Intent();
                Action action = new Action();
                action.AddAttribute("android:name", "android.intent.action.GET_CONTENT");
                intent.Add(action);
                queries.Add(intent);

                intent = new Intent();
                action = new Action();
                action.AddAttribute("android:name", "android.intent.action.OPEN_DOCUMENT");
                Category category = new Category();
                category.AddAttribute("android:name", "android.intent.category.OPENABLE");
                intent.Add(action);
                intent.Add(category);
                queries.Add(intent);
            }

            if (settings.SharingServicesSettings.IsEnabled || settings.WebViewSettings.IsEnabled)
            {
                Package package = new Package();
                package.AddAttribute("android:name", "com.facebook.katana");
                queries.Add(package);

                package = new Package();
                package.AddAttribute("android:name", "com.twitter.android");
                queries.Add(package);

                package = new Package();
                package.AddAttribute("android:name", "com.instagram.android");
                queries.Add(package);

                package = new Package();
                package.AddAttribute("android:name", "com.whatsapp");
                queries.Add(package);

                Intent intent = new Intent();
                Action action = new Action();
                action.AddAttribute("android:name", "android.intent.action.VIEW");
                intent.Add(action);
                queries.Add(intent);
            }

            manifest.Add(queries);
        }

        private static void AddActivities(Application application, EssentialKitSettings settings)
        {
            Activity activity;
            if (settings.WebViewSettings.IsEnabled)
            {
                activity = new Activity();
                activity.AddAttribute("android:name", s_androidLibraryRootPackageName + ".features.webview.WebViewActivity");
                activity.AddAttribute("android:theme", "@style/Theme.Transparent");
                activity.AddAttribute("android:hardwareAccelerated", "true");
                application.Add(activity);
            }

            if (settings.DeepLinkServicesSettings.IsEnabled)
            {
                var universalLinks  = settings.DeepLinkServicesSettings.AndroidProperties.UniversalLinks;
                var urlSchemes      = settings.DeepLinkServicesSettings.AndroidProperties.CustomSchemeUrls;

                if(universalLinks.Length > 0 || urlSchemes.Length > 0)
                {
                    activity = new Activity();
                    activity.AddAttribute("android:name", s_androidLibraryRootPackageName + ".features.deeplinkservices.DeepLinkRedirectActivity");
                    activity.AddAttribute("android:label", UnityEditor.PlayerSettings.productName);
                    activity.AddAttribute("android:exported", "true");

                    foreach (var each in universalLinks)
                    {
                        IntentFilter intentFilter = CreateIntentFilterForDeepLink(true, each.Identifier, each.Scheme, each.Host, each.Path);
                        activity.Add(intentFilter);
                    }

                    foreach (var each in urlSchemes)
                    {
                        IntentFilter intentFilter = CreateIntentFilterForDeepLink(false, each.Identifier, each.Scheme, each.Host, each.Path);
                        activity.Add(intentFilter);
                    }
                    application.Add(activity);
                }
            }

            if(settings.NotificationServicesSettings.IsEnabled)
            {
                activity = new Activity(); 
                activity.AddAttribute("android:name", s_androidLibraryRootPackageName + ".features.notificationservices.NotificationLauncher");
                activity.AddAttribute("android:theme", "@style/Theme.Transparent");
                activity.AddAttribute("android:exported", "true");
                application.Add(activity);
            }
        }

        private static void AddProviders(Application application, EssentialKitSettings settings)
        {
            Provider provider = null;

            provider = new Provider();
            provider.AddAttribute("android:name", "com.voxelbusters.android.essentialkit.common.FileProviderExtended");
            provider.AddAttribute("android:authorities", string.Format("{0}.essentialkit.fileprovider", UnityEngine.Application.identifier));
            provider.AddAttribute("android:exported", "false");
            provider.AddAttribute("android:grantUriPermissions", "true");

            MetaData metaData = new MetaData();
            metaData.AddAttribute("android:name", "android.support.FILE_PROVIDER_PATHS");
            metaData.AddAttribute("android:resource", "@xml/essential_kit_file_paths");

            provider.Add(metaData);
            application.Add(provider);
        }

        private static void AddServices(Application application, EssentialKitSettings settings)
        {
            if(settings.NotificationServicesSettings.IsEnabled && settings.NotificationServicesSettings.PushNotificationServiceType == PushNotificationServiceType.Custom)
            {
                //Firebase Cloud Messaging Service
                Service service             = new Service();
                IntentFilter intentFilter   = new IntentFilter();
                Action action               = new Action();

                service.AddAttribute("android:name", s_androidLibraryRootPackageName + ".features.notificationservices.provider.fcm.FCMMessagingService");
                service.AddAttribute("android:exported", "false");
                action.AddAttribute("android:name", "com.google.firebase.MESSAGING_EVENT");

                intentFilter.Add(action);
                service.Add(intentFilter);

                application.Add(service);
            }
        }

        private static void AddReceivers(Application application, EssentialKitSettings settings)
        {
            Receiver receiver;

            if(settings.NotificationServicesSettings.IsEnabled)
            {
                receiver = new Receiver();
                receiver.AddAttribute("android:name", s_androidLibraryRootPackageName + ".features.notificationservices.AlarmBroadcastReceiver");
                application.Add(receiver);

                receiver = new Receiver();
                receiver.AddAttribute("android:name", s_androidLibraryRootPackageName + ".features.notificationservices.BootCompleteBroadcastReceiver");
                receiver.AddAttribute("android:exported", "true");

                IntentFilter intentFilter = new IntentFilter();

                Category category = new Category();
                category.AddAttribute("android:name", "android.intent.category.DEFAULT");
                intentFilter.Add(category);

                Action action = new Action();
                action.AddAttribute("android:name", "android.intent.action.BOOT_COMPLETED");
                intentFilter.Add(action);

                action = new Action();
                action.AddAttribute("android:name", "android.intent.action.QUICKBOOT_POWERON");
                intentFilter.Add(action);

                action = new Action();
                action.AddAttribute("android:name", "com.htc.intent.action.QUICKBOOT_POWERON");
                intentFilter.Add(action);

                receiver.Add(intentFilter);
                application.Add(receiver);
            }
            
        }

        private static void AddMetaData(Application application, EssentialKitSettings settings)
        {
            if(settings.GameServicesSettings.IsEnabled || settings.CloudServicesSettings.IsEnabled)
            {
                MetaData metaData = new MetaData();
                metaData.AddAttribute("android:name", "com.google.android.gms.games.APP_ID");
                if(settings.GameServicesSettings.AndroidProperties.PlayServicesApplicationId != null)
                    metaData.AddAttribute("android:value", string.Format("\\u003{0}", settings.GameServicesSettings.AndroidProperties.PlayServicesApplicationId.Trim()));
                application.Add(metaData);
            }
        }

        private static void AddFeatures(Manifest manifest, EssentialKitSettings settings)
        {
            if ((settings.MediaServicesSettings.IsEnabled && settings.MediaServicesSettings.UsesCamera) || (settings.WebViewSettings.IsEnabled && settings.WebViewSettings.AndroidProperties.UsesCamera))
            {
                Feature feature = new Feature();
                feature.AddAttribute("android:name", "android.hardware.camera");
                manifest.Add(feature);

                feature = new Feature();
                feature.AddAttribute("android:name", "android.hardware.camera.autofocus");
                manifest.Add(feature);
            }
        }

        private static void AddPermissions(Manifest manifest, EssentialKitSettings settings, bool addSupportForApi30 = false)
        {
            Permission permission;

            if (settings.AddressBookSettings.IsEnabled)
            {
                permission = new Permission();
                permission.AddAttribute("android:name", "android.permission.READ_CONTACTS");
                manifest.Add(permission);
            }

            if (settings.NetworkServicesSettings.IsEnabled)
            {
                permission = new Permission();
                permission.AddAttribute("android:name", "android.permission.ACCESS_NETWORK_STATE");
                manifest.Add(permission);

                permission = new Permission();
                permission.AddAttribute("android:name", "android.permission.INTERNET");
                manifest.Add(permission);
            }

            if (settings.NotificationServicesSettings.IsEnabled)
            {
                permission = new Permission();
                permission.AddAttribute("android:name", "android.permission.RECEIVE_BOOT_COMPLETED");
                manifest.Add(permission);

                permission = new Permission();
                permission.AddAttribute("android:name", "android.permission.VIBRATE");

                if(!settings.NotificationServicesSettings.AndroidProperties.AllowVibration) //Don't limit maxSdk if vibration is set explicitly. Vibrate permission is required on some devices (4.0, 4.1) for notifications. So enabling for those by default
                    permission.AddAttribute("android:maxSdkVersion", "18");

                manifest.Add(permission);
            }

            if (settings.MediaServicesSettings.IsEnabled && settings.MediaServicesSettings.SavesFilesToGallery)
            {
                permission = new Permission();
                permission.AddAttribute("android:name", "android.permission.WRITE_EXTERNAL_STORAGE");
                permission.AddAttribute("tools:remove", "android:maxSdkVersion");
                manifest.Add(permission);
            }

            if (settings.MediaServicesSettings.IsEnabled && settings.MediaServicesSettings.UsesGallery)
            {
                permission = new Permission();
                permission.AddAttribute("android:name", "android.permission.READ_EXTERNAL_STORAGE");
                permission.AddAttribute("tools:remove", "android:maxSdkVersion");
                manifest.Add(permission);

                permission = new Permission();
                permission.AddAttribute("android:name", "com.google.android.apps.photos.permission.GOOGLE_PHOTOS");
                manifest.Add(permission);

                permission = new Permission();
                permission.AddAttribute("android:name", "android.permission.MANAGE_DOCUMENTS");
                manifest.Add(permission);
            }

            if ((settings.MediaServicesSettings.IsEnabled && settings.MediaServicesSettings.UsesCamera) || (settings.WebViewSettings.IsEnabled && settings.WebViewSettings.AndroidProperties.UsesCamera))
            {
                permission = new Permission();
                permission.AddAttribute("android:name", "android.permission.CAMERA");
                manifest.Add(permission);
            }

            if(settings.WebViewSettings.IsEnabled && settings.WebViewSettings.AndroidProperties.UsesMicrophone)
            {
                permission = new Permission();
                permission.AddAttribute("android:name", "android.permission.RECORD_AUDIO");
                manifest.Add(permission);

                permission = new Permission();
                permission.AddAttribute("android:name", "android.permission.MODIFY_AUDIO_SETTINGS");
                manifest.Add(permission);
            }

            /*if(addSupportForApi30)
            {
                permission = new Permission();
                permission.AddAttribute("android:name", "android.permission.QUERY_ALL_PACKAGES");
                manifest.Add(permission);
            }*/
        }

        private static IntentFilter CreateIntentFilterForDeepLink(bool isUniversalLinkFilter, string label, string scheme, string host, string pathPrefix = null)
        {
            IntentFilter intentFilter = new IntentFilter();
            intentFilter.AddAttribute("android:label", label);

            if(isUniversalLinkFilter)
                intentFilter.AddAttribute("android:autoVerify", "true");

            Action action = new Action();
            action.AddAttribute("android:name", "android.intent.action.VIEW");
            intentFilter.Add(action);

            Category category = new Category();
            category.AddAttribute("android:name", "android.intent.category.DEFAULT");
            intentFilter.Add(category);

            category = new Category();
            category.AddAttribute("android:name", "android.intent.category.BROWSABLE");
            intentFilter.Add(category);

            Data data = new Data();
            data.AddAttribute("android:scheme", scheme);
            if (!string.IsNullOrEmpty(host))
            {
                data.AddAttribute("android:host", host);
            }

            if (!string.IsNullOrEmpty(pathPrefix))
            {
                data.AddAttribute("android:pathPrefix", pathPrefix.StartsWith("/") ? pathPrefix :  "/" + pathPrefix);
            }

            intentFilter.Add(data);

            return intentFilter;
        }

#endregion
    }
}
#endif