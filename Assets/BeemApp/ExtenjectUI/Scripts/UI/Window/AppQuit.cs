using UnityEngine;

namespace Beem.Extenject.UI {

    /// <summary>
    /// Application Quit
    /// </summary>
    public class AppQuit : MonoBehaviour, IEscape {

        public void Escape() {
            Application.Quit();
        }
    }
}
