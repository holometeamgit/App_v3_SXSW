using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Beem.SSO;
using TMPro;


public class PnlEmailVerificationFirebase : MonoBehaviour
{
    [SerializeField]
    AuthController AuthController;
    [SerializeField]
    TMP_Text txtEmail;

    private void OnEnable() {
        txtEmail.text = AuthController.GetEmail();
    }
}
