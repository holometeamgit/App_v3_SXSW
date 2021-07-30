using UnityEngine;
using Zenject;

namespace Beem.Extenject.UI {
    /// <summary>
    /// Window Instaler for all windows
    /// </summary>
    public class WindowInstaller : MonoInstaller {
        public override void InstallBindings() {
            Container.DeclareSignal<WindowSignal>();

            Container.Bind<Transform>().FromInstance(transform);

            Container.BindInterfacesAndSelfTo<WindowController>().AsSingle();

            Container.BindInterfacesAndSelfTo<PoolController>().AsSingle();
        }
    }
}
