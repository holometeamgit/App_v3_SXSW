using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Beem.UI {
    /// <summary>
    /// Interface for BusinessProfileManager
    /// </summary>

    public interface IBusinessProfileManagerView {
        /// <summary>
        /// Init BusinessProfileManager
        /// </summary>
        /// <param name="businessProfileManager"></param>
        void Init(BusinessProfileManager businessProfileManager);
    }
}
