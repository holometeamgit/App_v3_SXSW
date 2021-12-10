using UnityEngine;
using UnityEditor;
namespace Beem.Extenject.UI {

    /// <summary>
    /// Editor for Window Object
    /// </summary>
    [CustomEditor(typeof(WindowObject))]

    public class WindowObjectEditor : Editor {

        public override void OnInspectorGUI() {
            WindowObject windowObject = (WindowObject)target;
            windowObject.Type = (WindowObject.WindowObjectType)EditorGUILayout.EnumPopup("Item Type:", windowObject.Type);

            if (windowObject.Type == WindowObject.WindowObjectType.Prefab) {
                windowObject.Prefab = (GameObject)EditorGUILayout.ObjectField("Item Prefab:", windowObject.Prefab, typeof(GameObject));
            } else {
                windowObject.Folder = EditorGUILayout.TextField("Item Folder:", windowObject.Folder);
            }
        }

    }
}
