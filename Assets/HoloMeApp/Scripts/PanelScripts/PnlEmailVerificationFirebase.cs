using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Beem.SSO;
using TMPro;
using System.Threading.Tasks;

public class PnlEmailVerificationFirebase : MonoBehaviour
{
    [SerializeField]
    AuthController AuthController;
    [SerializeField]
    TMP_Text txtEmail;
    [SerializeField]
    GameObject GoToLogInBtn;

    private const int DELAY_TIME = 5000;

    /// <summary>
    /// The method do actions after pressing the ResendVerification button
    /// </summary>
    public void ResendVerificationBtnClick() {
        CallBacks.onEmailVerification?.Invoke();
    }

    private void OnEnable() {
        txtEmail.text = AuthController.GetEmail();

        TaskScheduler taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
        Task.Delay(DELAY_TIME).ContinueWith((_) => GoToLogInBtn.SetActive(true), taskScheduler);
    }

    private void OnDisable() {
        GoToLogInBtn.SetActive(false);
    }

    private void OnApplicationFocus(bool focus) {
        GoToLogInBtn.SetActive(true);
    }
}
