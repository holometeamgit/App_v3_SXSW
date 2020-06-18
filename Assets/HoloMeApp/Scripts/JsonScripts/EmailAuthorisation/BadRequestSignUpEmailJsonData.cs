using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class BadRequestSignUpEmailJsonData
{
    public List<string> username;
    public List<string> email;
    public List<string> password1;
    public List<string> non_field_errors;

    public BadRequestSignUpEmailJsonData() {
        username = new List<string>();
        email = new List<string>();
        password1 = new List<string>();
        non_field_errors = new List<string>();
    }
}
