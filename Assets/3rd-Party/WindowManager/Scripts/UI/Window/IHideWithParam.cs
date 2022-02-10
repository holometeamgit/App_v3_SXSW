namespace WindowManager.Extenject {
    /// <summary>
    /// Interface for Hide Window for all Window with param
    /// </summary>
    public interface IHideWithParam {
        /// <summary>
        /// Hide Window
        /// </summary>
        void Hide<T>(T parameter);
    }
}
