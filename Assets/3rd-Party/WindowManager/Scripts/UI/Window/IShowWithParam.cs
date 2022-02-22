namespace WindowManager.Extenject {
    /// <summary>
    /// Interface for Show Window for all Window with param
    /// </summary>
    public interface IShowWithParam {
        /// <summary>
        /// Show Window
        /// </summary>
        void Show<T>(T parameter);
    }
}
