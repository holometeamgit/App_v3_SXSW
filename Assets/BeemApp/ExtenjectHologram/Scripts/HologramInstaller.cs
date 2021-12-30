using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Beem.Extenject.Hologram {

    public class HologramInstaller : MonoInstaller {
        public override void InstallBindings() {
            Container.DeclareSignal<TargetPlacementSignal>();
            Container.DeclareSignal<ARSessionActivateSignal>();
            Container.DeclareSignal<ARPlanesDetectedSignal>();
            Container.DeclareSignal<HologramPlacementSignal>();
            Container.DeclareSignal<ARPinchSignal>();
            Container.DeclareSignal<SelectHologramSignal>();
            Container.BindInterfacesAndSelfTo<HologramConstructor>().AsSingle();
        }
    }
}
