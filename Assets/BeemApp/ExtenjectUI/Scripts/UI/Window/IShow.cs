﻿namespace Beem.Extenject.UI {
    /// <summary>
    /// Interface for Show Window for all Window with param
    /// </summary>
    public interface IShow {
        /// <summary>
        /// Show Window
        /// </summary>
        void Show<T>(T parameter);
    }
}
