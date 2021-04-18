using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Beem.Pagination;
using System;

namespace Beem.Content {
    public class CommentsController {

        public Action onDataLoaded;
        public Action onFailDataLoaded;
        public Action onPosted;
        public Action onFailPosted;
        public Action onAllDataLoaded;
        public Action onPostData;

        PageLoader<CommentJsonData> _commentPageLoader;
        WebRequestHandler _webRequestHandler;
        CommentsContainer _commentsContainer;

        VideoUploader _videoUploaderAPI;
        ServerURLAPIScriptableObject _serverURLAPI;
        long _contentId;
        bool isBusy;

        const int PAGE_SIZE = 100;

        public CommentsController(WebRequestHandler webRequestHandler, VideoUploader videoUploader, ServerURLAPIScriptableObject serverURLAPI, int contentId) {
            _webRequestHandler = webRequestHandler;
            _videoUploaderAPI = videoUploader;
            _serverURLAPI = serverURLAPI;
            _contentId = contentId;
            _commentsContainer = new CommentsContainer();
        }

        public void Refresh() {
            if(_commentPageLoader != null) {
                _commentPageLoader.onDataLoaded -= OnGetData;
                _commentPageLoader.onFailDataLoaded -= OnFailGetData;
                _commentPageLoader.onAllDataLoaded -= OnAllDataLoad;
            }

            _commentPageLoader = new PageLoader<CommentJsonData>(GetRequestUrl(), _webRequestHandler, PAGE_SIZE);
            _commentPageLoader.onDataLoaded += OnGetData;
            _commentPageLoader.onFailDataLoaded += OnFailGetData;
            _commentPageLoader.onAllDataLoaded += OnAllDataLoad;
            Next();
        }

        public void Next() {
            if (_commentPageLoader == null) {
                Refresh();
                return;
            }

            _commentPageLoader.NextPage();
        }

        public bool IsAllDataLoad() {
            return _commentPageLoader.IsAllDataLoaded();
        }

        public CommentJsonData GetData(int index) {
            return _commentsContainer.GetByIndex(index);
        }

        public int CountLoadedData() {
            return _commentsContainer.Count();
        }

        public void Post(PostCommentJsonData postComment) {
            _webRequestHandler.Post(GetRequestUrl(), postComment, WebRequestHandler.BodyType.JSON,
                (code, body) => OnPost(),
                (code, body) => OnFailPost(code, body));
        }

        #region onLoading 
        private void OnGetData(List<CommentJsonData> comments) {
            _commentsContainer.Add(comments);
            onDataLoaded.Invoke();
        }

        private void OnFailGetData() {
            onFailDataLoaded.Invoke();
        }

        private void OnAllDataLoad() {
            onAllDataLoaded?.Invoke();
        }
        #endregion

        #region onPosting
        private void OnPost() {
            onPosted?.Invoke();
        }

        private void OnFailPost(long code, string body) {
            onFailPosted?.Invoke();
            HelperFunctions.DevLogError("code " + body);
        }
        #endregion

        private string GetRequestUrl() {
            return _serverURLAPI.DevServerURLMedia + _videoUploaderAPI.GetComments.Replace("{id}", _contentId.ToString());
        }

        private string PostRequestUrl() {
            return _serverURLAPI.DevServerURLMedia + _videoUploaderAPI.PostComments.Replace("{id}", _contentId.ToString());
        }
    }
}