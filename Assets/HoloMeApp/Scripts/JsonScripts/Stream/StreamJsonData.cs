using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class StreamJsonData
{
    public int count;
    public string next;
    public string previous;
    public List<Data> results;

    public StreamJsonData() {
        results = new List<Data>();
    }

    [Serializable]
    public class Data {

        public enum Stage {
            All,
            Live,
            Announced,
            Finished
        }

        public long id;
        public string preview_s3_url;
        public string stream_s3_url;
        public string user;
        public string status;
        public string agora_sid;
        public string agora_channel;
        public string file_name_prefix;
        public string start_date;
        public string end_date;
        public long duration;
        public string preview_s3_key;
        public string stream_s3_key;
        public string title;
        public string description;

        public DateTime StartDate {
            get {
                if (startDate != new DateTime())
                    return startDate;

                if (!DateTime.TryParse(start_date, out startDate))
                    startDate = new DateTime();
                return startDate;
            }
        }

        public DateTime EndDate {
            get {
                if (endDate != new DateTime())
                    return endDate;

                if (!DateTime.TryParse(end_date, out endDate))
                    endDate = new DateTime();
                return endDate;
            }
        }

        private DateTime startDate;
        private DateTime endDate;

        private const string announcedStr = "announced";
        private const string finishedStr = "finished";
        private const string lifeStr = "live";

        public Stage GetStatus() {
            switch (status) {
            case announcedStr:
                return Stage.Announced;
            case finishedStr:
                return Stage.Finished;
            case lifeStr:
                return Stage.Live;
            default:
                return Stage.All;
            }
        }

        public static string GetStatusValue(Stage stage) {
            switch (stage) {
            case StreamJsonData.Data.Stage.Announced:
                return announcedStr;
            case StreamJsonData.Data.Stage.Finished:
                return finishedStr;
            case StreamJsonData.Data.Stage.Live:
                return lifeStr;
            case StreamJsonData.Data.Stage.All:
            default:
                return "All";
            }
        }
    }


}



