using UnityEngine;

namespace Beem.Extenject.UI {
    /// <summary>
    /// ScriptableObject for application windows
    /// </summary>
    [CreateAssetMenu(fileName = "WindowObject", menuName = "Beem/UI/New WindowObject")]
    public class WindowObject : ScriptableObject {
        [Header("Window Id")]
        [SerializeField]
        private string _id = default;

        public string Id {
            get {
                return _id;
            }
        }

        [Header("Window Prefab")]
        [SerializeField]
        private GameObject _prefab = default;

        public GameObject Prefab {
            get {
                return _prefab;
            }
        }
    }
}
