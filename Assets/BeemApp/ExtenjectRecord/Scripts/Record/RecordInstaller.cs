using Beem.Extenject.UI;
using UnityEngine;
using Zenject;

namespace Beem.Extenject.Record {

    public class RecordInstaller : MonoInstaller {

        public override void InstallBindings() {
            Camera[] cameras = FindObjectsOfType<Camera>();

            Container.DeclareSignal<VideoRecordStartSignal>();
            Container.DeclareSignal<VideoRecordStopSignal>();
            Container.DeclareSignal<VideoRecordProgressSignal>();
            Container.DeclareSignal<VideoRecordEndSignal>();
            Container.DeclareSignal<VideoRecordFinishSignal>();
            Container.BindInterfacesAndSelfTo<VideoRecordController>().AsSingle();

            Container.DeclareSignal<SnapShotStartSignal>();
            Container.DeclareSignal<SnapShotEndSignal>();
            Container.DeclareSignal<SnapShotFinishSignal>();
            Container.BindInterfacesAndSelfTo<SnapShotController>().AsSingle();

            Container.BindInterfacesAndSelfTo<RecordSystem>().AsSingle().WithArguments(cameras);
        }
    }
}
