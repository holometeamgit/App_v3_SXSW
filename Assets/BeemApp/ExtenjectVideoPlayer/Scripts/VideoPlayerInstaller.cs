using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Beem.Extenject.Video {

    public class VideoPlayerInstaller : MonoInstaller {
        public override void InstallBindings() {
            Container.DeclareSignal<InitSignal>();
            Container.DeclareSignal<PlaySignal>();
            Container.DeclareSignal<PauseSignal>();
            Container.DeclareSignal<StopSignal>();
            Container.DeclareSignal<RewindSignal>();
            Container.DeclareSignal<StartRewindSignal>();
            Container.DeclareSignal<FinishRewindSignal>();
            Container.BindInterfacesAndSelfTo<VideoPlayerController>().AsSingle();
        }
    }
}
