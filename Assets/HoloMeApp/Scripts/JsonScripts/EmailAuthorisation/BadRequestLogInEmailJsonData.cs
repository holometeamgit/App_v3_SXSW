using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class BadRequestLogInEmailJsonData
{
    public List<string> username;
    public List<string> password;
    public string detail;

    public BadRequestLogInEmailJsonData() {
        username = new List<string>();
        password = new List<string>();
        detail = "";
    }
}
