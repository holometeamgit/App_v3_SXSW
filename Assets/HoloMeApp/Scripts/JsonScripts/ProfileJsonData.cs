using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class ProfileJsonData {
    public string bio;
    public bool go_live_feature;
    public bool room_feature;
    public string profile_picture_s3_url;
    public List<string> capabilities;
}
