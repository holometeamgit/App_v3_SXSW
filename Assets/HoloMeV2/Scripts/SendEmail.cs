using UnityEngine;
using UnityEngine.Networking;

public class SendEmail : MonoBehaviour
{
    [SerializeField]
    string email = "support@holo.me";

    [SerializeField]
    string subject = "HoloMe App Support";

    public void SendOffEmail()
    {
        string escapedSubject = MyEscapeURL(subject);
        string body = MyEscapeURL("");
        Application.OpenURL("mailto:" + email + "?subject=" + escapedSubject + "&body=" + body);
    }
    static string MyEscapeURL(string URL)
    {
        return UnityWebRequest.EscapeURL(URL).Replace("+", "%20");
    }
}
