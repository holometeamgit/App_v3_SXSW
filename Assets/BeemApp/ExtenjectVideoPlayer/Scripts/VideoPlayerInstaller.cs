using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Beem.Extenject.Video {

    public class VideoPlayerInstaller : MonoInstaller {
        public override void InstallBindings() {
            Container.BindInterfacesAndSelfTo<VideoPlayerController>().AsSingle();
        }
    }
}
