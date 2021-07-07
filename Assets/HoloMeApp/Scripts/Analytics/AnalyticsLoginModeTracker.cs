using UnityEngine;

public class AnalyticsLoginModeTracker : MonoBehaviour {
    public static AnalyticsLoginModeTracker Instance { get; private set; }

    public enum LoginMethod { Email, Google, Apple }

    public LoginMethod LoginMethodUsed { get; private set; }

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(Instance);
        } else {
            Debug.LogError($"{nameof(AnalyticsLoginModeTracker)} Instance Already Exists!");
            Destroy(Instance);
        }
    }

    public void SetAsEmail() {
        LoginMethodUsed = LoginMethod.Email;
        SendSignUpTappedEvent();
    }

    public void SetAsGoogle() {
        LoginMethodUsed = LoginMethod.Google;
        SendSignUpTappedEvent();
    }

    public void SetAsApple() {
        LoginMethodUsed = LoginMethod.Apple;
        SendSignUpTappedEvent();
    }

    void SendSignUpTappedEvent() {
        AnalyticsController.Instance.SendCustomEvent(AnalyticKeys.KeySignUpTapped, AnalyticParameters.ParamSignUpMethod, LoginMethodUsed.ToString());
    }
}
