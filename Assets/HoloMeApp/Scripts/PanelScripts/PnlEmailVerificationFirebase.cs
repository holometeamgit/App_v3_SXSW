using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Beem.SSO;
using TMPro;
using System.Threading.Tasks;
using System;

public class PnlEmailVerificationFirebase : MonoBehaviour
{
    [SerializeField]
    AuthController _authController;
    [SerializeField]
    TMP_Text _txtEmail;
    [SerializeField]
    GameObject _goToLogInBtn;
    [SerializeField] TMP_Text _resendMsgMin;
    [SerializeField] TMP_Text _resendMsgSec;
    private const int DELAY_TIME = 5000;

    /// <summary>
    /// The method do actions after pressing the ResendVerification button
    /// </summary>
    public void ResendVerificationBtnClick() {
        CallBacks.onEmailVerification?.Invoke();

        StartCoroutine(UpdateResendText());
    }

    private void OnEnable() {
        _txtEmail.text = _authController.GetEmail();

        StartCoroutine(UpdateResendText());
        TaskScheduler taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
        Task.Delay(DELAY_TIME).ContinueWith((_) => _goToLogInBtn.SetActive(true), taskScheduler);
    }



    private void OnDisable() {
        _goToLogInBtn.SetActive(false);
        StopAllCoroutines();
        EmailVerificationTimer.Cancel();
        _resendMsgMin.text = "1";
        _resendMsgSec.text = "00";
    }

    private void OnApplicationFocus(bool focus) {
        _goToLogInBtn.SetActive(true);
    }

    private IEnumerator UpdateResendText() {
        yield return new WaitForSeconds(1);
        TimeSpan timeSpan = EmailVerificationTimer.GetTimeLeft();
        while (timeSpan.TotalSeconds > 1) {
            _resendMsgMin.text = timeSpan.Minutes.ToString();
            _resendMsgSec.text = (timeSpan.Seconds < 10 ? "0" : "") + timeSpan.Seconds.ToString();
            timeSpan = EmailVerificationTimer.GetTimeLeft();
            yield return new WaitForSeconds(1);
        }
    }
}
