using Beem.SSO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    }
}
