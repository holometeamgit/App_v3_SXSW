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

public class PnlSignUpLive : MonoBehaviour
{
    [SerializeField]
    TMP_InputField inputEmail;

    [SerializeField]
    IncorrectInputAnimationToggle incorrectInputAnimationToggle;

    [SerializeField]
    UnityEvent OnSignUpComplete;

    [SerializeField]
    PnlGenericError pnlGenericError;

    [SerializeField]
    Button btnClose;

    public const string MatchEmailPattern =
        @"^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@"
        + @"((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\."
        + @"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|"
        + @"([a-zA-Z]+[\w-]+\.)+[a-zA-Z]{2,4})$";

    public static bool IsEmail(string email)
    {
        if (!string.IsNullOrEmpty(email)) return Regex.IsMatch(email, MatchEmailPattern);
        else return false;
    }

    public void Send()
    {
        if (!IsEmail(inputEmail.text))
        {
            incorrectInputAnimationToggle.StartIncorrectAnimation();
            return;
        }

        try
        {
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress("zuheb.javed@holo.me");
            mail.To.Add("zuheb.javed@holo.me");
            mail.Subject = "Holo Live Interest";
            mail.Body = "User Email: " + inputEmail.text;
            SmtpClient smtpServer = new SmtpClient("smtp.gmail.com");
            smtpServer.Port = 587;
            smtpServer.Credentials = new System.Net.NetworkCredential("zuheb.javed@holo.me", "acddyakcoyhdujxi") as ICredentialsByHost;
            smtpServer.EnableSsl = true;
            ServicePointManager.ServerCertificateValidationCallback =
            delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
            { return true; };
            smtpServer.Send(mail);
        }
        catch (Exception exception)
        {
            Debug.LogError(exception);
            pnlGenericError.ActivateSingleButton("Error", "An error occurred please try again later", onBackPress: () => btnClose.onClick?.Invoke());
            return;
        }

        OnSignUpComplete?.Invoke();
    }

}
