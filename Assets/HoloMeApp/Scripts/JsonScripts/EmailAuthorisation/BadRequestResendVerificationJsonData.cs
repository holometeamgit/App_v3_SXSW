using System.Collections.Generic;
using System;

[Serializable]
public class BadRequestResendVerificationJsonData : BadRequestJsonData {
    public List<string> email;
    public string detail;

    public BadRequestResendVerificationJsonData() {
        email = new List<string>();
        detail = "";
    }
}
