using UnityEngine;

/// <summary>
/// Used to determine which sign up method was used and to track sign up button presses
/// </summary>
public class AnalyticsSignUpModeTracker : MonoBehaviour {
    public static AnalyticsSignUpModeTracker Instance { get; private set; }

    public enum SignUpMethod { Email, Google, Apple }

    public SignUpMethod SignUpMethodUsed { get; private set; }

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(Instance);
        } else {
            Debug.LogError($"{nameof(AnalyticsSignUpModeTracker)} Instance Already Exists!");
            Destroy(Instance);
        }
    }

    /// <summary>
    /// Set login method used to email
    /// </summary>
    public void SetAsEmail() {
        SignUpMethodUsed = SignUpMethod.Email;
        SendSignUpTappedEvent();
    }

    /// <summary>
    /// Set login method used to Google
    /// </summary>
    public void SetAsGoogle() {
        SignUpMethodUsed = SignUpMethod.Google;
        SendSignUpTappedEvent();
    }

    /// <summary>
    /// Set login method used to Apple
    /// </summary>
    public void SetAsApple() {
        SignUpMethodUsed = SignUpMethod.Apple;
        SendSignUpTappedEvent();
    }

    private void SendSignUpTappedEvent() {
        AnalyticsController.Instance.SendCustomEvent(AnalyticKeys.KeySignUpTapped, AnalyticParameters.ParamSignUpMethod, SignUpMethodUsed.ToString());
    }
}
