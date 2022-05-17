using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using System;

/// <summary>
/// sets all the necessary scriptable object DATA to the container
/// </summary>
[CreateAssetMenu(fileName = "BeemScriptableObjectInstaller", menuName = "Installers/BeemScriptableObjectInstaller")]
public class BeemScriptableObjectInstaller : ScriptableObjectInstaller<BeemScriptableObjectInstaller> {

    [SerializeField]
    private ARMsgAPIScriptableObject ARMsgAPIScriptableObject;
    [SerializeField]
    private AuthorizationAPIScriptableObject AuthorizationAPIScriptableObject;
    [SerializeField]
    private ExternalLinksScriptableObject ExternalLinksScriptableObject;
    [SerializeField]
    private GeneralAppAPIScriptableObject GeneralAppAPIScriptableObject;
    [SerializeField]
    private PurchaseAPIScriptableObject PurchaseAPIScriptableObject;
    [SerializeField]
    private ServerURLAPIScriptableObject ServerURLAPIScriptableObject;
    [SerializeField]
    private VideoUploader VideoUploader;

    [SerializeField]
    private QRCodeAdditionalInformationScriptableObject QRCodeAdditionalInformationScriptableObject;

    public override void InstallBindings() {
        Container.Bind<ARMsgAPIScriptableObject>().AsSingle().NonLazy();
        Container.Bind<AuthorizationAPIScriptableObject>().AsSingle().NonLazy();
        Container.Bind<ExternalLinksScriptableObject>().AsSingle().NonLazy();
        Container.Bind<GeneralAppAPIScriptableObject>().AsSingle().NonLazy();
        Container.Bind<PurchaseAPIScriptableObject>().AsSingle().NonLazy();
        Container.Bind<ServerURLAPIScriptableObject>().AsSingle().NonLazy();
        Container.Bind<VideoUploader>().AsSingle().NonLazy();

        Container.Bind<QRCodeAdditionalInformationScriptableObject>().AsSingle().NonLazy();
    }
}