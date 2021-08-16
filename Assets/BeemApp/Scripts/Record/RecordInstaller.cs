using Beem.Extenject.UI;
using UnityEngine;
using Zenject;

namespace Beem.Extenject.Record {

    public class RecordInstaller : MonoInstaller {

        [SerializeField]
        private WindowSignal _snapShotWindow;

        [SerializeField]
        private WindowSignal _videoRecordWindow;

        public override void InstallBindings() {
            Camera[] cameras = FindObjectsOfType<Camera>();
            Container.DeclareSignal<RecordStartSignal>();
            Container.DeclareSignal<RecordStopSignal>();
            Container.DeclareSignal<RecordProgressSignal>();
            Container.DeclareSignal<RecordEndSignal>();
            Container.BindInterfacesAndSelfTo<RecordController>().AsSingle();
            Container.BindInterfacesAndSelfTo<VideoRecordController>().AsSingle().WithArguments(_videoRecordWindow, cameras);
            Container.BindInterfacesAndSelfTo<SnapShotController>().AsSingle().WithArguments(_snapShotWindow, cameras);
        }
    }
}
