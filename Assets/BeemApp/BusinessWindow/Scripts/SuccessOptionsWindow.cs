using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Success Options Window
/// </summary>
public class SuccessOptionsWindow : MonoBehaviour {

    [SerializeField]
    private Mover _mover;


    private const int DELAY_FOR_SUCCESS = 3000;

    /// <summary>
    /// Show Window
    /// </summary>
    public async void Show() {
        gameObject.SetActive(true);
        _mover.ChangeState(true);
        _mover.onEndMoving += OnClose;

        await Task.Delay(DELAY_FOR_SUCCESS);
        BusinessOptionsConstructor.OnShowLast?.Invoke();
        SuccessOptionsConstructor.OnHide?.Invoke();
    }

    /// <summary>
    /// Hide Window
    /// </summary>
    public void Hide() {
        _mover.ChangeState(false);
    }

    private void OnClose(bool status) {
        if (!status) {
            _mover.onEndMoving -= OnClose;
            gameObject.SetActive(false);
        }
    }

}
