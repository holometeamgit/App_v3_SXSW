using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

public class StyleController : MonoBehaviour {
    public Action OnStyleChanged;

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
        if(currentBeemMenuStyle.ColorStyles != null)
            foreach(var colorsStyle in currentBeemMenuStyle.ColorStyles)
                mainDataContainer.Container.Remove(colorsStyle.Colors);

        if (currentBeemMenuStyle.ColorStyles != null)
            foreach (var spritesStyle in currentBeemMenuStyle.Sprites)
                mainDataContainer.Container.Remove(spritesStyle.Sprites);

        index = (index + 1) % beemMenuStyles.Count;
        currentBeemMenuStyle = beemMenuStyles[index];

        if (currentBeemMenuStyle.ColorStyles != null)
            foreach (var colorsStyle in currentBeemMenuStyle.ColorStyles)
                mainDataContainer.Container.Add(colorsStyle.Colors);

        if (currentBeemMenuStyle.ColorStyles != null)
            foreach (var spritesStyle in currentBeemMenuStyle.Sprites)
                mainDataContainer.Container.Add(spritesStyle.Sprites);

//        Debug.Log("Style Changed");
        OnStyleChanged?.Invoke();
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {

            UpdateStyle();
        }
    }
}
