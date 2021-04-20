using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Beem.Pagination;
using System;

namespace Beem.Content {
    public class CommentsController {

        public Action<int,int> onDataFetched;
        public Action onFailDataFetched;
        public Action onPosted;
        public Action onFailPosted;
        public Action onAllDataLoaded;

        PageLoader<CommentJsonData> _commentPageLoader;
        WebRequestHandler _webRequestHandler;
        CommentsContainer _commentsContainer;

        VideoUploader _videoUploaderAPI;
        long _contentId;

        const int PAGE_SIZE = 100;

        public CommentsController(WebRequestHandler webRequestHandler, VideoUploader videoUploader) {
            _webRequestHandler = webRequestHandler;
            _videoUploaderAPI = videoUploader;
        }

        public void Init(int contentId) {
            HelperFunctions.DevLog("CommentsController Init  contentId " + contentId);
            _contentId = contentId;
            _commentsContainer = new CommentsContainer();
            Refresh();
        }

        public void StopLoading() { }

        public void Refresh() {
            HelperFunctions.DevLog("CommentsController Refresh");
            if (_commentPageLoader != null) {
                _commentPageLoader.onDataLoaded -= OnFetchData;
                _commentPageLoader.onFailDataLoaded -= OnFailGetData;
                _commentPageLoader.onAllDataLoaded -= OnAllDataLoad;
                _commentPageLoader.onInit -= Next;
            }

            _commentPageLoader = new PageLoader<CommentJsonData>(GetRequestUrl(), _webRequestHandler, PAGE_SIZE);
            _commentPageLoader.onDataLoaded += OnFetchData;
            _commentPageLoader.onFailDataLoaded += OnFailGetData;
            _commentPageLoader.onAllDataLoaded += OnAllDataLoad;
            _commentPageLoader.onInit += Next;
        }

        public void Next() {
            if (_commentPageLoader == null) {
                Refresh();
                return;
            }
            HelperFunctions.DevLog("CommentsController NextPage");
            _commentPageLoader.NextPage();
        }

        public bool IsAllDataLoad() {
            return _commentPageLoader.IsAllDataLoaded();
        }

        public CommentJsonData GetData(int index) {
            HelperFunctions.DevLog("CommentsController  GetData index " + index);
            return _commentsContainer.GetByIndex(index);
        }

        public int CountLoadedData() {
            return _commentsContainer.Count();
        }

        public void Post(string comment) {
            HelperFunctions.DevLog("Post = " + comment);
            Post(new PostCommentJsonData(comment));
        }

        public void Post(PostCommentJsonData postComment) {
            _webRequestHandler.Post(GetRequestUrl(), postComment, WebRequestHandler.BodyType.JSON,
                (code, body) => OnPost(),
                (code, body) => OnFailPost(code, body));
        }

        #region onLoading 
        private void OnFetchData(List<CommentJsonData> comments) {
            HelperFunctions.DevLog("OnGetData count = " + comments.Count);

            int prevCountItems = _commentsContainer.Count();
            _commentsContainer.Add(comments);

            onDataFetched?.Invoke(_commentsContainer.Count(), _commentsContainer.Count() - prevCountItems);
        }

        private void OnFailGetData() {
            onFailDataFetched.Invoke();
        }

        private void OnAllDataLoad() {
            onAllDataLoaded?.Invoke();
        }
        #endregion

        #region onPosting
        private void OnPost() {
            HelperFunctions.DevLog("OnPost");
            onPosted?.Invoke();
        }

        private void OnFailPost(long code, string body) {
            onFailPosted?.Invoke();
            HelperFunctions.DevLogError("code " + body);
        }
        #endregion

        private string GetRequestUrl() {
            return _webRequestHandler.ServerURLMediaAPI + _videoUploaderAPI.GetComments.Replace("{id}", _contentId.ToString());
        }

        private string PostRequestUrl() {
            return _webRequestHandler.ServerURLMediaAPI + _videoUploaderAPI.PostComments.Replace("{id}", _contentId.ToString());
        }
    }
}