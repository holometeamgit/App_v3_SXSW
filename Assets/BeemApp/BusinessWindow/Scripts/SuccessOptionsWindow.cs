using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Success Options Window
/// </summary>
public class SuccessOptionsWindow : MonoBehaviour {

    private const int DELAY_FOR_SUCCESS = 3000;

    /// <summary>
    /// Show Window
    /// </summary>
    public async void Show() {
        gameObject.SetActive(true);

        await Task.Delay(DELAY_FOR_SUCCESS);
        BusinessOptionsConstructor.OnShowLast?.Invoke();
        SuccessOptionsConstructor.OnHide?.Invoke();
    }

    /// <summary>
    /// Hide Window
    /// </summary>
    public void Hide() {
        gameObject.SetActive(false);
    }

}
