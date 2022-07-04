using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Beem.UI {
    /// <summary>
    /// Interface for UserWebManager
    /// </summary>

    public interface IUserWebManagerView {
        /// <summary>
        /// Init UserWebManager
        /// </summary>
        /// <param name="userWebManager"></param>
        void Init(UserWebManager userWebManager);
    }
}
