using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class StreamJsonData
{
    public int count;
    public int next;
    public int previous;
    public List<Data> results;

    public StreamJsonData() {
        results = new List<Data>();
    }

    [Serializable]
    public class Data {

        public int id;
        public string preview_s3_url;
        public string stream_s3_url;
        public string status;
        public int agora_id;
        public string agora_chanel;
        public string start_date;
        public string end_date;
        public int duration;
        public string preview_s3_key;
        public string stream_s3_key;
        public string name;
        public string description;
        public int user;
    }
}



