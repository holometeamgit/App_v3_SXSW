using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Beem.Extenject.Hologram {

    public class HologramInstaller : MonoInstaller {
        public override void InstallBindings() {
            Container.DeclareSignal<CreateHologramSignal>();
            Container.DeclareSignal<SelectHologramSignal>();
        }
    }
}
