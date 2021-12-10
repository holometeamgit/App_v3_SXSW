using UnityEngine;

namespace Beem.Extenject.UI {
    /// <summary>
    /// ScriptableObject for application windows
    /// </summary>
    [CreateAssetMenu(fileName = "WindowObject", menuName = "Beem/UI/New WindowObject")]
    public class WindowObject : ScriptableObject {

        public enum WindowObjectType {
            Prefab,
            Resource
        }

        [Header("Window Object Type")]

        public WindowObjectType Type;


        public string Id {
            get {
                return WindowPrefab.name;
            }
        }

        [Header("Window Prefab")]
        public GameObject Prefab = default;

        public GameObject WindowPrefab {
            get {
                if (Type == WindowObjectType.Prefab) {
                    return Prefab;
                } else {
                    return (GameObject)Resources.Load(Folder, typeof(GameObject));
                }
            }
        }

        [Header("Window Resource Folder")]
        public string Folder = default;
    }
}
