using System;
using System.Collections.Generic;

/// <summary>
/// ARMsgJSON. Serializable class for communication with server 
/// </summary>
[Serializable]
public class ARMsgJSON {
    public int count;
    public string next;
    public string previous;
    public List<Data> results;

    /// <summary>
    /// subclass ARMsgJSON
    /// </summary>
    [Serializable]
    public class Data : IData {
        public string id;
        public string share_link;
        public string ar_message_s3_link;
        public string processing_status;
        public string created_at;
        public string processed_at;
        public string user;

        public static readonly string PROCESSING_STATUS = "PROCESSING";
        public static readonly string COMPETED_STATUS = "COMPLETED";
        public static readonly string FAILED_STATUS = "FAILED";
        public static readonly string IN_QUEUE_STATUS = "IN_QUEUE";

        private DateTime _created_at;
        private DateTime _processed_at;

        public DateTime CreatedAt {
            get {
                if (_created_at != new DateTime())
                    return _created_at;
                if (!DateTime.TryParse(created_at, out _created_at))
                    _created_at = new DateTime();
                return _created_at;
            }
        }

        public DateTime ProcessedAt {
            get {
                if (_processed_at != new DateTime())
                    return _processed_at;
                if (!DateTime.TryParse(processed_at, out _processed_at))
                    _processed_at = new DateTime();
                return _processed_at;
            }
        }

        public string Id => id;

        public string ShareLink => share_link;

        public string Username => user;

        public string Status => processing_status;
    }
}
