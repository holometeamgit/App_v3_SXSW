using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[SerializeField]
public class BadRequestResetPassword : BadRequestJsonData {
    public List<string> new_password1;
    public List<string> new_password2;
    public List<string> uid;
    public List<string> token;
    public string detail;

    public BadRequestResetPassword() {
        new_password1 = new List<string>();
        new_password2 = new List<string>();
        uid = new List<string>();
        token = new List<string>();
    }
}
