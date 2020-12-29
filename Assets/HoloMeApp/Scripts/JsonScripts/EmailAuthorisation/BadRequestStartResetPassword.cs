using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class BadRequestStartResetPassword : BadRequestJsonData {
    public List<string> email;
    public string detail;

    public BadRequestStartResetPassword() {
        email = new List<string>();
        detail = "";
    }
}
