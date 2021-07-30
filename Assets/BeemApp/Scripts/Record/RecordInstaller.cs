using Beem.Extenject.Record;
using Beem.Extenject.Record.SnapShot;
using Beem.Extenject.UI;
using Beem.Extenject.Video;
using UnityEngine;
using Zenject;

public class RecordInstaller : MonoInstaller {

    [SerializeField]
    private WindowSignal _snapShotWindow;

    [SerializeField]
    private WindowSignal _videoRecordWindow;

    public override void InstallBindings() {
        Debug.Log("RecordInstaller");
        Camera[] cameras = FindObjectsOfType<Camera>();
        Container.DeclareSignal<RecordSignal>();
        Container.BindInterfacesAndSelfTo<VideoRecordController>().AsSingle().WithArguments(_videoRecordWindow, cameras);
        Container.BindInterfacesAndSelfTo<SnapShotController>().AsSingle().WithArguments(_snapShotWindow);
    }
}
