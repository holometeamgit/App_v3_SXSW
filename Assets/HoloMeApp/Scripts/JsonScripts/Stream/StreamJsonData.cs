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
        public string stream_s3_url;
        public string teaser_s3_url;
        public string preview_teaser_s3_url;
        public bool is_bought;
        public bool is_liked;
        public string user;
        public long user_id;
        public ProductType product_type;
        public List<string> content_category;
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
        public string teaser_s3_key;
        public string preview_teaser_s3_key;
        public string title;
        public string description;
        public string paid_type;
        public string privacy_type;
        public string shader_info;
        public long count_of_likes;
        public long count_of_views;

        public const string PIN_ALL = "all";
        public const string LIVE_STR = "live";
        public const string LIVE_ROOM_STR = "live_room";
        public const string STOP_STR = "stop";
        public const string PRERECORDED_STR = "prerecorded";

        private DateTime startDate;
        private DateTime endDate;


        /*public void InvokeDataUpdated() { //TODO better create RX
            OnDataUpdated?.Invoke();
        }*/

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

        public bool IsPublicLiveOrPrerecorded() {
            return (GetStage() == Stage.Live || GetStage() == Stage.Prerecorded) && privacy_type == "public";
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

        public void Update(Data data) {

            if (data.id != id)
                return;

            preview_s3_url = data.preview_s3_url;
            stream_s3_url = data.stream_s3_url;
            teaser_s3_url = data.teaser_s3_url;
            preview_teaser_s3_url = data.preview_teaser_s3_url;
            is_bought = data.is_bought;
            is_liked = data.is_liked;
            user = data.user;
            user_id = data.user_id;
            product_type = data.product_type;
            content_category = data.content_category;
            status = data.status;
            is_pin = data.is_pin;
            agora_sid = data.agora_sid;
            agora_channel = data.agora_channel;
            file_name_prefix = data.file_name_prefix;
            start_date = data.start_date;
            end_date = data.end_date;
            duration = data.duration;
            preview_s3_key = data.preview_s3_key;
            stream_s3_key = data.stream_s3_key;
            teaser_s3_key = data.teaser_s3_key;
            preview_teaser_s3_key = data.preview_teaser_s3_key;
            title = data.title;
            description = data.description;
            paid_type = data.paid_type;
            privacy_type = data.privacy_type;
            shader_info = data.shader_info;
            count_of_likes = data.count_of_likes;
            count_of_views = data.count_of_views;


            startDate = new DateTime();
            endDate = new DateTime();
            OnDataUpdated?.Invoke();
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



