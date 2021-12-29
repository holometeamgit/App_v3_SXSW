using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Beem.Extenject.Hologram {

    public class HologramInstaller : MonoInstaller {
        public override void InstallBindings() {
            Container.DeclareSignal<TargetPlacementSignal>();
            Container.DeclareSignal<HologramPlacementSignal>();
            Container.DeclareSignal<ARSignal>();
            Container.DeclareSignal<ARPlanesDetectedSignal>();
            Container.DeclareSignal<ARPinchSignal>();
            Container.DeclareSignal<SelectHologramSignal>();
        }
    }
}
