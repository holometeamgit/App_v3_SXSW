using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class BadRequestLogInEmailJsonData
{
    public List<string> username;
    public List<string> password;
    public List<string> non_field_errors;
    public string detail;

    public BadRequestLogInEmailJsonData() {
        username = new List<string>();
        password = new List<string>();
        non_field_errors = new List<string>();
        detail = "";
    }
}
