namespace Beem.Permissions {
    /// <summary>
    /// Microphone permission
    /// </summary>
    public interface IMicrophonePermission {
        bool HasMicAccess { get; }

        void RequestMicAccess();
    }
}
