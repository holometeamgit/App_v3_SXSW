using System.ComponentModel;
using Beem.SSO;
using Zenject;

/// <summary>
/// Installer for all currentcontrollers
/// </summary>
public class ControllerInstaller : MonoInstaller {
    public override void InstallBindings() {
        Container.BindInterfacesAndSelfTo<WebRequestHandler>().FromComponentInHierarchy(false).AsSingle();
        Container.BindInterfacesAndSelfTo<AccountManager>().FromComponentInHierarchy(false).AsSingle();
        Container.BindInterfacesAndSelfTo<AuthController>().FromComponentInHierarchy(false).AsSingle();
        Container.BindInterfacesAndSelfTo<UserWebManager>().FromComponentInHierarchy(false).AsSingle();
        Container.BindInterfacesAndSelfTo<HologramHandler>().FromComponentInHierarchy(false).AsSingle();
        Container.BindInterfacesAndSelfTo<AgoraController>().FromComponentInHierarchy(false).AsSingle();
        Container.BindInterfacesAndSelfTo<PurchaseManager>().FromComponentInHierarchy(false).AsSingle();
        Container.BindInterfacesAndSelfTo<AgoraRTMChatController>().FromComponentInHierarchy(false).AsSingle();
        Container.BindInterfacesAndSelfTo<SecondaryServerCalls>().FromComponentInHierarchy(false).AsSingle();
        Container.BindInterfacesAndSelfTo<AgoraRequests>().FromComponentInHierarchy(false).AsSingle();
        Container.BindInterfacesAndSelfTo<IAPController>().FromComponentInHierarchy(false).AsSingle();
        Container.BindInterfacesAndSelfTo<PurchasesSaveManager>().FromComponentInHierarchy(false).AsSingle();
        Container.BindInterfacesAndSelfTo<PnlStreamMLCameraView>().FromComponentInHierarchy(true).AsSingle();
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
