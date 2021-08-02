using Zenject;

namespace Beem.Extenject {
    /// <summary>
    /// Bind Manager for Signals
    /// </summary>
    public static class BindManager {
        public static void BindAllSignals<T>(DiContainer container) where T : BeemSignal {
            BindSignal<T>(container);
            BindSignal<SuccessSignal<T>>(container);
            BindSignal<FailSignal<T>>(container);
        }

        public static void BindSignal<T>(DiContainer container) where T : BeemSignal {
            container.DeclareSignal<T>();
        }
    }
}