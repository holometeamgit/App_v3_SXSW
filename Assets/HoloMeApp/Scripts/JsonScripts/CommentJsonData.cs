using System;

namespace Beem.Content {

    [Serializable]
    public class CommentJsonData {
        public int id;
        public string body;
        public DateTime created_at;
        public int timeline;
        public string user;
    }
}