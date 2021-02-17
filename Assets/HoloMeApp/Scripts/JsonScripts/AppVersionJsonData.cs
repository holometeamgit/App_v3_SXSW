using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class AppVersionJsonData 
{
    public List<Version> versions;

    public AppVersionJsonData() {
        versions = new List<Version>();
    }

    [Serializable]
    public class Version {
        public string min_support_version;
        public bool forced_update;
    }
}


