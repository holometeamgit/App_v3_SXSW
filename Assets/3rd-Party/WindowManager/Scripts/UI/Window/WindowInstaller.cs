using UnityEngine;
using Zenject;

namespace WindowManager.Extenject {
    /// <summary>
    /// Window Instaler for all windows
    /// </summary>
    public class WindowInstaller : MonoInstaller {
        public override void InstallBindings() {
            Container.Bind<Transform>().FromInstance(transform);
            Container.BindInterfacesAndSelfTo<PoolController>().AsSingle();
            Container.BindInterfacesAndSelfTo<WindowController>().AsSingle();
        }
    }
}
