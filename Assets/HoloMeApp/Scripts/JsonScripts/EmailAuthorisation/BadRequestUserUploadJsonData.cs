using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BadRequestUserUploadJsonData : MonoBehaviour
{
    public List<string> username;
    public List<string> first_name;
    public List<string> last_name;
    public string detail;

    public BadRequestUserUploadJsonData() {
        username = new List<string>();
        first_name = new List<string>();
        last_name = new List<string>();
        detail = "";
    }
}
