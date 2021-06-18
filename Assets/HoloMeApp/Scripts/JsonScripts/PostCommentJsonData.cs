using System;

namespace Beem.Content {

    [Serializable]
    public class PostCommentJsonData {
        public string body;
        public int timeline;

        public PostCommentJsonData() { }

        public PostCommentJsonData(string body) {
            this.body = body;
            timeline = 0;
        }
    }
}
