using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TextCounter : MonoBehaviour {
    [SerializeField]
    private TMP_Text _textCount;
    [SerializeField]
    private float _destroyTimer = 1.2f;

    public string Text {
        set { _textCount.text = value; }
        get { return _textCount.text; }
    }

    private void OnEnable() {
        Destroy(gameObject, _destroyTimer);
    }

    private void OnDisable() {
        gameObject.SetActive(false);
    }
}
