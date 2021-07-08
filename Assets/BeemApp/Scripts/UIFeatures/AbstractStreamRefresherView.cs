using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Beem.UI {

    /// <summary>
    /// Abstract Stream Data Refresher
    /// </summary>
    public abstract class AbstractStreamRefresherView : MonoBehaviour {

        protected CancellationTokenSource _cancelTokenSource;

        protected abstract int delay { get; }


        /// <summary>
        /// Refresh data Video Player 
        /// </summary>
        /// <param name="streamID"></param>
        public abstract void Refresh(string streamID);

        /// <summary>
        /// StartCount Data
        /// </summary>
        /// <param name="streamID">Id for streams</param>
        /// <param name="condition">Conditions for repeating</param>
        public async void StartCountAsync(string streamID, bool condition = true) {
            _cancelTokenSource = new CancellationTokenSource();
            try {
                while (condition) {
                    Refresh(streamID);
                    await Task.Delay(delay);
                }
            } finally {
                if (_cancelTokenSource != null) {
                    _cancelTokenSource.Dispose();
                    _cancelTokenSource = null;
                }
            }
        }

        protected void OnDestroy() {
            Cancel();
        }

        /// <summary>
        /// Clear Info
        /// </summary>
        public void Cancel() {
            if (_cancelTokenSource != null) {
                _cancelTokenSource.Cancel();
                _cancelTokenSource = null;
            }
        }
    }
}
