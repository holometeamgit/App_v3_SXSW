using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StyleController : MonoBehaviour {
    public UnityEvent OnStyleChanged;

    [SerializeField]
    MainDataContainer mainDataContainer;

    //rewrite with tags
    [SerializeField]
    List<BeemMenuStyleScriptableObject> beemMenuStyles;
    BeemMenuStyleScriptableObject currentBeemMenuStyle;
    int index = 0;

    private void Awake() {
        currentBeemMenuStyle = beemMenuStyles[0];
    }

    private void Start() {
        UpdateStyle();
    }

    public void UpdateStyle() {
        mainDataContainer.Container.Remove(currentBeemMenuStyle.colorStyles);
        mainDataContainer.Container.Remove(currentBeemMenuStyle.sprites);

        index = (index + 1) % beemMenuStyles.Count;
        currentBeemMenuStyle = beemMenuStyles[index];

        mainDataContainer.Container.Add(currentBeemMenuStyle.colorStyles);
        mainDataContainer.Container.Add(currentBeemMenuStyle.sprites);

        Debug.Log("Style Changed");
        OnStyleChanged.Invoke();
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {

            UpdateStyle();
        }
    }
}
