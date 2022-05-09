using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using System;

[CreateAssetMenu(fileName = "BeemScriptableObjectInstaller", menuName = "Installers/BeemScriptableObjectInstaller")]
public class BeemScriptableObjectInstaller : ScriptableObjectInstaller<BeemScriptableObjectInstaller> {

    public ARMsgAPIScriptableObject ARMsgAPIScriptableObject;
    public AuthorizationAPIScriptableObject AuthorizationAPIScriptableObject;
    public ExternalLinksScriptableObject ExternalLinksScriptableObject;
    public GeneralAppAPIScriptableObject GeneralAppAPIScriptableObject;
    public PurchaseAPIScriptableObject PurchaseAPIScriptableObject;
    public ServerURLAPIScriptableObject ServerURLAPIScriptableObject;
    public VideoUploader VideoUploader;

    public override void InstallBindings() {
        Container.Bind<ARMsgAPIScriptableObject>().AsSingle().NonLazy();
        Container.Bind<AuthorizationAPIScriptableObject>().AsSingle().NonLazy();
        Container.Bind<ExternalLinksScriptableObject>().AsSingle().NonLazy();
        Container.Bind<GeneralAppAPIScriptableObject>().AsSingle().NonLazy();
        Container.Bind<PurchaseAPIScriptableObject>().AsSingle().NonLazy();
        Container.Bind<ServerURLAPIScriptableObject>().AsSingle().NonLazy();
        Container.Bind<VideoUploader>().AsSingle().NonLazy();
    }
}