using UnityEngine;

namespace WindowManager.Extenject {

    /// <summary>
    /// Application Quit
    /// </summary>
    public class AppQuit : MonoBehaviour, IEscape {
        public void Escape() {
            Application.Quit();
        }
    }
}
