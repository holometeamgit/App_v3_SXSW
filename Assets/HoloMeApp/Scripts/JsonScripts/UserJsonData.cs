using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class UserJsonData
{
    public int pk;
    public string username;
    public string email;
    public string first_name;
    public string last_name;
    public bool is_creator;
    public ProfileJsonData profile;

    public UserJsonData() {
        profile = new ProfileJsonData();
    }
}
