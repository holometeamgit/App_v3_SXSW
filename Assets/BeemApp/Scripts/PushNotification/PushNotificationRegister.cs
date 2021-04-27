using CleverTap;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_IOS && !UNITY_EDITOR
using NotificationServices = UnityEngine.iOS.NotificationServices;
using NotificationType = UnityEngine.iOS.NotificationType;
#endif

namespace Beem.CleverTap
{

    public class PushNotificationRegister : MonoBehaviour
    {
#if UNITY_IOS && !UNITY_EDITOR
        private void Start()
        {
            NotificationServices.RegisterForNotifications(NotificationType.Alert | NotificationType.Badge | NotificationType.Sound);
        }
#endif
    }
}
