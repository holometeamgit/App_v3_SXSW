using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Success Options Window
/// </summary>
public class SuccessOptionsWindow : MonoBehaviour, IBlindView {

    [SerializeField]
    private TMP_Text _titleText;

    [SerializeField]
    private TMP_Text _descriptionText;

    [SerializeField]
    private BtnController _backBtn;

    private SuccessOptionsData _successOptionsData;

    /// <summary>
    /// Show Window
    /// </summary>
    public void Show(params object[] objects) {

        if (objects != null && objects.Length > 0) {
            foreach (var item in objects) {
                if (item is SuccessOptionsData) {
                    _successOptionsData = item as SuccessOptionsData;
                }
            }
        }


        gameObject.SetActive(true);

        if (_successOptionsData != null) {
            _titleText.text = _successOptionsData.Title;
            _descriptionText.text = _successOptionsData.Description;
            _backBtn.OnPress.AddListener(Back);
        }

    }

    private void Back() {
        _successOptionsData.BackEvent?.Invoke();
        _backBtn.OnPress.RemoveListener(Back);
    }

    /// <summary>
    /// Hide Window
    /// </summary>

    public void Hide() {
        gameObject.SetActive(false);
    }
}
