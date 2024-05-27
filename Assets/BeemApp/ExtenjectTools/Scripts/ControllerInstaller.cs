using System.ComponentModel;
using Beem.SSO;
using Zenject;

/// <summary>
/// Installer for all currentcontrollers
/// </summary>
public class ControllerInstaller : MonoInstaller {
    public override void InstallBindings() {
        Container.BindInterfacesAndSelfTo<WebRequestHandler>().FromComponentInHierarchy(false).AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<AccountManager>().FromComponentInHierarchy(false).AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<AuthController>().FromComponentInHierarchy(false).AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<UserWebManager>().FromComponentInHierarchy(false).AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<HologramHandler>().FromComponentInHierarchy(false).AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<AgoraController>().FromComponentInHierarchy(false).AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<PurchaseManager>().FromComponentInHierarchy(false).AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<AgoraRTMChatController>().FromComponentInHierarchy(false).AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<SecondaryServerCalls>().FromComponentInHierarchy(false).AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<AgoraRequests>().FromComponentInHierarchy(false).AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<IAPController>().FromComponentInHierarchy(false).AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<PurchasesSaveManager>().FromComponentInHierarchy(false).AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<PnlStreamMLCameraView>().FromComponentInHierarchy(true).AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<PnlViewingExperience>().FromComponentInHierarchy(true).AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<SharingOnViewCntroller>().FromComponentInHierarchy(true).AsSingle().NonLazy();



        Container.BindInterfacesAndSelfTo<BusinessLogoController>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<BusinessProfileManager>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<QRCodeGenerator>().AsSingle().NonLazy();

        Container.BindInterfacesAndSelfTo<DeleteARMsgController>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<GetAllARMsgController>().AsSingle().NonLazy();

        InstallSignals();
    }

    private void InstallSignals() {
        SignalBusInstaller.Install(Container);

        Container.DeclareSignal<DeleteARMsgSignal>();
        Container.DeclareSignal<GetAllArMessagesSignal>();
        Container.DeclareSignal<GetAllArMessagesSuccesSignal>();

        Container.DeclareSignal<PnlViewingExperience.BeemVideoPlayStartedSignal>();
        Container.DeclareSignal<PnlViewingExperience.BeemVideoPlayStoped>();
        Container.DeclareSignal<PnlViewingExperience.ARBeemShareLinkReceived>();
    }
}
