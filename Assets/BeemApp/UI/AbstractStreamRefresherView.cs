using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Beem {

    /// <summary>
    /// Abstract Stream Data Refresher
    /// </summary>
    public abstract class AbstractStreamRefresherView : MonoBehaviour {

        protected CancellationTokenSource cancelTokenSource;

        protected abstract int refreshTimer { get; }

        /// <summary>
        /// Refresh data Video Player 
        /// </summary>
        /// <param name="videoPlayer"></param>
        public abstract void Refresh(string streamID);

        public async void StartCount(string streamID, bool condition = true) {
            cancelTokenSource = new CancellationTokenSource();
            try {
                while (condition) {
                    Refresh(streamID);
                    await Task.Delay(refreshTimer);
                }
            } finally {
                if (cancelTokenSource != null) {
                    cancelTokenSource.Dispose();
                    cancelTokenSource = null;
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
            if (cancelTokenSource != null) {
                cancelTokenSource.Cancel();
                cancelTokenSource = null;
            }
        }
    }
}
