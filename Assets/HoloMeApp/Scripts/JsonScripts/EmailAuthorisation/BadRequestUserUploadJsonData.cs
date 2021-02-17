using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class BadRequestUserUploadJsonData : BadRequestJsonData {
    public string username;
    public List<string> first_name;
    public List<string> last_name;
    public string detail;

    public BadRequestUserUploadJsonData() {
        first_name = new List<string>();
        last_name = new List<string>();
        detail = "";
    }
}
