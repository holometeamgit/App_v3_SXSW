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
            if (Application.platform == RuntimePlatform.Android) {
                Container.Bind<IPermissionGranter>().To<AndroidPermission>().AsSingle();
            } else if (Application.platform == RuntimePlatform.IPhonePlayer) {
                Container.Bind<IPermissionGranter>().To<iOSPermission>().AsSingle();
            } else {
                Container.Bind<IPermissionGranter>().To<EditorPermission>().AsSingle();
            }

            Container.BindInterfacesAndSelfTo<PermissionController>().AsSingle().WithArguments(_windowObject);

        }
    }
}
