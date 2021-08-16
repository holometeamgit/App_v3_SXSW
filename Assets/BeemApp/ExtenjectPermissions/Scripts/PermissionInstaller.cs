using Beem.Extenject.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Beem.Extenject.Permissions {

    /// <summary>
    /// Permission Installer
    /// </summary>
    public class PermissionInstaller : MonoInstaller {

        [SerializeField]
        private WindowObject _windowObject;

        public override void InstallBindings() {
#if UNITY_IOS && !UNITY_EDITOR
            Container.Bind<ICameraPermission>().To<iOSCameraPermission>().AsSingle();
            Container.Bind<IMicrophonePermission>().To<iOSMicPermission>().AsSingle();
            Container.Bind<ISettingsPermission>().To<iOSPermission>().AsSingle();
#elif UNITY_ANDROID && !UNITY_EDITOR
            Container.Bind<ICameraPermission>().To<AndroidCameraPermission>().AsSingle();
            Container.Bind<IMicrophonePermission>().To<AndroidMicPermission>().AsSingle();
            Container.Bind<ISettingsPermission>().To<AndroidPermission>().AsSingle();
#else
            Container.Bind<ICameraPermission>().To<EditorCameraPermission>().AsSingle();
            Container.Bind<IMicrophonePermission>().To<EditorMicPermission>().AsSingle();
            Container.Bind<ISettingsPermission>().To<EditorPermission>().AsSingle();
#endif
            Container.BindInterfacesAndSelfTo<PermissionController>().AsSingle().WithArguments(_windowObject);

        }
    }
}
