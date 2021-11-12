using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;
using UnityEngine.Events;
using System;
using UnityEngine.UI;

public class PnlSignUpLive : MonoBehaviour {
    [SerializeField]
    InputFieldController inputFieldControllerEmail;

    [SerializeField]
    UnityEvent OnSignUpComplete;

    [SerializeField]
    Button btnClose;

    public const string MatchEmailPattern =
        @"^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@"
        + @"((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\."
        + @"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|"
        + @"([a-zA-Z]+[\w-]+\.)+[a-zA-Z]{2,4})$";

    public static bool IsEmail(string email) {
        if (!string.IsNullOrEmpty(email)) return Regex.IsMatch(email, MatchEmailPattern);
        else return false;
    }

    public void Send() {
        if (!IsEmail(inputFieldControllerEmail.text)) {
            inputFieldControllerEmail.ShowWarning("Please enter a valid email");
            return;
        }

        try {
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress("customer-success@holo.me");
            mail.To.Add("customer-success@holo.me");
            mail.Subject = "Holo Live Interest from " + inputFieldControllerEmail.text;
            mail.Body = "User Email: " + inputFieldControllerEmail.text;
            SmtpClient smtpServer = new SmtpClient("smtp.gmail.com");
            smtpServer.Port = 587;
            smtpServer.Credentials = new System.Net.NetworkCredential("customer-success@holo.me", "kdudvzgzzpfuortr") as ICredentialsByHost;
            smtpServer.EnableSsl = true;
            ServicePointManager.ServerCertificateValidationCallback =
            delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
            smtpServer.Send(mail);
        } catch (Exception exception) {
            Debug.LogError(exception);
            GenericConstructor.ActivateSingleButton("Error", "An error occurred please try again later", onBackPress: () => btnClose.onClick?.Invoke());
            return;
        }

        OnSignUpComplete?.Invoke();
    }

}
