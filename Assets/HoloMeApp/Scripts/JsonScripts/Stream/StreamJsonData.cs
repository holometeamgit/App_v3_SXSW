using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class StreamJsonData {

    public int count;
    public string next;
    public string previous;
    public List<Data> results;

    public StreamJsonData() {
        results = new List<Data>();
    }

    [Serializable]
    public class Data {
        public Action OnDataUpdated;

        public enum Stage {
            All,
            Pin,
            Live,
            PastLivestream,
            Prerecorded
        }

        public long id;
        public string preview_s3_url;
        public string preview_teaser_s3_url;
        public string stream_s3_url;
        public string teaser_s3_url;
        public string user;
        public string paid_type;
        public bool is_bought;
        public ProductType product_type;
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

        public void InvokeDataUpdated() { //TODO better create RX
            OnDataUpdated?.Invoke();
        }

        public Data() {
            product_type = new ProductType();
        }

        public bool IsStarted {
            get { return DateTime.Now >= StartDate; }
        }

        public bool HasTeaser {
            get { return !string.IsNullOrWhiteSpace(teaser_s3_url); }
        }

        public bool HasTeaserPreview {
            get { return !string.IsNullOrWhiteSpace(preview_teaser_s3_url); }
        }

        public bool HasStreamUrl {
            get { return !string.IsNullOrWhiteSpace(stream_s3_url); }
        }

        public bool HasAgoraChannel {
            get { return !string.IsNullOrWhiteSpace(agora_channel); }
        }

        public bool HasProduct {
            get { return !string.IsNullOrWhiteSpace(product_type.product_id); }
        }

        public bool HasEndTime {
            get { return !string.IsNullOrWhiteSpace(end_date); }
        }

        public DateTime StartDate {
            get {
                if (startDate != new DateTime())
                    return startDate;
                //Debug.Log("GET start_date " + start_date);
                if (!DateTime.TryParse(start_date, out startDate))
                    startDate = new DateTime();
                return startDate;
            }
        }

        public DateTime EndDate {
            get {
                if (endDate != new DateTime())
                    return endDate;
                //Debug.Log("GET start_date " + start_date);
                if (!DateTime.TryParse(end_date, out endDate))
                    endDate = new DateTime();
                return endDate;
            }
        }

        private DateTime startDate;
        private DateTime endDate;
        private const string pinAll = "all";
        private const string pinStr = "pin";
        private const string lifeStr = "live";
        private const string pastLivestreamStr = "past_live";
        private const string prerecordedStr = "prerecorded";

        public Stage GetStatus() {
            switch (status) {
                case pinStr:
                    return Stage.Pin;
                case lifeStr:
                    return Stage.Live;
                case pastLivestreamStr:
                    return Stage.PastLivestream;
                case prerecordedStr:
                    return Stage.Prerecorded;
                default:
                    return Stage.All;
            }
        }

        public static string GetStatusValue(Stage stage) {
            switch (stage) {
                case StreamJsonData.Data.Stage.Pin:
                    return pinStr;
                case StreamJsonData.Data.Stage.Live:
                    return lifeStr;
                case StreamJsonData.Data.Stage.PastLivestream:
                    return pastLivestreamStr;
                case StreamJsonData.Data.Stage.Prerecorded:
                    return prerecordedStr;
                default:
                    return pinAll;
            }
        }
    }

    [Serializable]
    public class ProductType {
        public string name;
        public string product_id;
        public float price;
        public string condition_start_date;
        public string condition_end_date;

        public DateTime StartDate {
            get {
                if (startDate != new DateTime())
                    return startDate;
                if (!DateTime.TryParse(condition_start_date, out startDate))
                    startDate = new DateTime();
                return startDate;
            }
        }

        private DateTime startDate;
    }

}



