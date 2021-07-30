using System;
using System.Threading.Tasks;

namespace Beem.Extenject {

    /// <summary>
    /// Task manager
    /// </summary>
    public static class BeemTaskManager {

        /// <summary>
        /// Check Task for Firebase
        /// </summary>
        /// <param name="task"></param>
        /// <param name="onSuccess"></param>
        /// <param name="onFail"></param>
        public static void Check<T>(T task, Action onSuccess, Action<Exception> onFail) where T : Task {
            if (task.IsCanceled) {
                onFail?.Invoke(new Exception("Cancelled"));
                return;
            }
            if (task.IsFaulted) {
                onFail?.Invoke(task.Exception);
                return;
            }
            onSuccess?.Invoke();
        }

    }
}
