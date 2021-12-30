using Beem.Extenject.Hologram;
using Beem.Extenject.Video;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using Zenject;
namespace Beem.Extenject.Hologram {
    /// <summary>
    /// Constructor for hologram creation
    /// </summary>
    public class HologramConstructor : IInitializable, ILateDisposable {
        private SignalBus _signalBus;
        private VideoPlayerController _videoPlayerController;

        [Inject]
        public void Construct(SignalBus signalBus, VideoPlayerController videoPlayerController) {
            _videoPlayerController = videoPlayerController;
            _signalBus = signalBus;
        }

        private void Construct(HologramPlacementSignal signal) {
            _videoPlayerController?.SetVideoPlayer(signal.Hologram.GetComponentInChildren<VideoPlayer>());
        }

        public void Initialize() {
            _signalBus.Subscribe<HologramPlacementSignal>(Construct);
        }

        public void LateDispose() {
            _signalBus.Unsubscribe<HologramPlacementSignal>(Construct);
        }
    }
}
