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
            Live,
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
        public bool is_pin;
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

        public const string PIN_ALL = "all";
        public const string LIVE_STR = "live";
        public const string LIVE_ROOM_STR = "live_room";
        public const string PRERECORDED_STR = "prerecorded";

        private DateTime startDate;
        private DateTime endDate;


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

        public Stage GetStage() {
            return GetStage(status);
        }

        public static Stage GetStage(string status) {
            switch (status) {
                case LIVE_STR:
                    return Stage.Live;
                case PRERECORDED_STR:
                    return Stage.Prerecorded;
                default:
                    return Stage.All;
            }
        }

        public bool IsPin() {
            return is_pin;
        }

        public static string GetStatusValue(Stage stage) {
            switch (stage) {
                case Stage.Live:
                    return LIVE_STR;
                case Stage.Prerecorded:
                    return PRERECORDED_STR;
                default:
                    return PIN_ALL;
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



