using CleverTap;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NotificationServices = UnityEngine.iOS.NotificationServices;
using NotificationType = UnityEngine.iOS.NotificationType;

namespace Beem.CleverTap
{

    public class PushNotificationRegister : MonoBehaviour
    {
        private void Start()
        {
            NotificationServices.RegisterForNotifications(NotificationType.Alert | NotificationType.Badge | NotificationType.Sound);
            CleverTapBinding.RegisterPush();
        }
    }
}
