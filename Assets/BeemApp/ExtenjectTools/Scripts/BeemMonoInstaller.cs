using UnityEngine;
using Zenject;

namespace Beem.Extenject
{

    ///Main Beam App Installer
    public class BeemMonoInstaller : MonoInstaller
    {

        /// <summary>
        /// Here we bind the objects that will need to be added to the container. They will be initialized before Mono scripts start running.
        /// </summary>

        public override void InstallBindings()
        {
            SignalBusInstaller.Install(Container);
        }
    }
}