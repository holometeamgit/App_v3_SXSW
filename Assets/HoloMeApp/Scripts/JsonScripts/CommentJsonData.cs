using System;

namespace Beem.Content {

    [Serializable]
    public class CommentJsonData {
        public int id;
        public string body;
        public string created_at;
        public int timeline;
        public string user;

        private DateTime createdat;

        public DateTime CreatedAt {
            get {
                if (createdat != new DateTime())
                    return createdat;
                if (!DateTime.TryParse(created_at, out createdat))
                    createdat = new DateTime();
                return createdat;
            }
        }
    }
}