using UnityEngine;
using Zenject;

namespace Beem.Extenject {
    /// <summary>
    /// Fail View
    /// </summary>
    public abstract class AbstractFailView<T> : MonoBehaviour {

        protected SignalBus _signalBus;

        [Inject]
        public void Construct(SignalBus signalBus) {
            _signalBus = signalBus;
        }

        protected void OnEnable() {
            _signalBus.Subscribe<FailSignal<T>>(Fail);
        }

        protected void OnDisable() {
            _signalBus.Unsubscribe<FailSignal<T>>(Fail);
        }

        protected abstract void Fail(FailSignal<T> requestFail);
    }
}