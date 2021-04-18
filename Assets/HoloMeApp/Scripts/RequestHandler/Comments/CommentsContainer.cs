using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Beem.Content {

    public class CommentsContainer {
        private SortedHashSet<int, CommentJsonData> _comments;

        public CommentsContainer() {
            _comments = new SortedHashSet<int, CommentJsonData>(new CommentsByDateTimeComparer());
        }

        public void Add(List<CommentJsonData> commentsList) {
            foreach (var comment in commentsList) {
                _comments.Add(comment.id, comment);
            }
        }

        public CommentJsonData GetByIndex(int index) {
            return _comments.GetByIndex(index);
        }

        public int Count() {
            return _comments.Count();
        }

        public void Clear() {
            _comments.Clear();
        }

        public void Remove(int id) {
            _comments.Remove(id);
        }
    }
}