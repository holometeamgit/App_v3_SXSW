using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class PasswordWarningChangeJsonData
{
    public List<string> old_password;
    public List<string> new_password1;
    public List<string> new_password2;

    public PasswordWarningChangeJsonData() {
        old_password = new List<string>();
        new_password1 = new List<string>();
        new_password2 = new List<string>();
    }
}