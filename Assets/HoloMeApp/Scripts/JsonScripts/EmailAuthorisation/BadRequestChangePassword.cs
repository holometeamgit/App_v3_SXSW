using System.Collections.Generic;
using System;

[Serializable]
public class BadRequestChangePassword : BadRequestJsonData {
    public List<string> new_password1;
    public List<string> new_password2;
    public string detail;

    public BadRequestChangePassword() {
        new_password1 = new List<string>();
        new_password2 = new List<string>();
    }
}
