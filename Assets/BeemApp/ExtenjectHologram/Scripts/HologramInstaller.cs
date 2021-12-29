using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Beem.Extenject.Hologram {

    public class HologramInstaller : MonoInstaller {
        public override void InstallBindings() {
            Container.DeclareSignal<TargetPlacementSignal>();
            Container.DeclareSignal<CreateHologramSignal>();
            Container.DeclareSignal<ARSessionActivateSignal>();
            Container.DeclareSignal<ARPlanesDetectedSignal>();
            Container.DeclareSignal<HologramPlacementSignal>();
            Container.DeclareSignal<ARPinchSignal>();
            Container.DeclareSignal<SelectHologramSignal>();
            /*Container.Bind<ARHint>().AsSingle();
            Container.BindSignal<ARSessionActivateSignal>().ToMethod<ARHint>((x) => x.ActivateAR).FromResolve();
            Container.BindSignal<ARSessionActivateSignal>().ToMethod<ARHint>((x) => x.ActivateAR).FromResolve();
            Container.BindSignal<ARPlanesDetectedSignal>().ToMethod<ARHint>(x => x.ActivateARPlanesDetected).FromResolve();
            Container.BindSignal<ARPinchSignal>().ToMethod<ARHint>(x => x.ActivateARPinch).FromResolve();
            Container.BindSignal<HologramPlacementSignal>().ToMethod<ARHint>(x => x.ActivateHologramPlacement).FromResolve();*/
        }
    }
}
