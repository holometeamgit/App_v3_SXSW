using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Beem.Pagination;
using System;

namespace Beem.Content {
    public class CommentsController {

        public Action<int, int> onDataFetched;
        public Action onFailDataFetched;
        public Action onPosted;
        public Action onFailPosted;
        public Action onAllDataLoaded;
        public Action<int> onFetchedTotalCommentsCount;
        public Action onRefresh;

        PageLoader<CommentJsonData> _commentPageLoader;
        private WebRequestHandler _webRequestHandler;
        CommentsContainer _commentsContainer;

        VideoUploader _videoUploaderAPI;
        long _contentId;
        bool _afterRefresh;

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
                _commentPageLoader.onFetchedTotalCommentsCount -= OnFetchTotalCommentsCount;
            }

            _afterRefresh = true;

            _commentPageLoader = new PageLoader<CommentJsonData>(GetRequestUrl(), _webRequestHandler, PAGE_SIZE);
            _commentPageLoader.onDataLoaded += OnFetchData;
            _commentPageLoader.onFailDataLoaded += OnFailGetData;
            _commentPageLoader.onAllDataLoaded += OnAllDataLoad;
            _commentPageLoader.onInit += Next;
            _commentPageLoader.onFetchedTotalCommentsCount += OnFetchTotalCommentsCount;
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

        public void Post(string comment) {
            HelperFunctions.DevLog("Post = " + comment);
            Post(new PostCommentJsonData(comment));
        }

        public void Post(PostCommentJsonData postComment) {
            _webRequestHandler.Post(PostRequestUrl(), postComment, WebRequestBodyType.JSON,
                (code, body) => OnPost(),
                (code, body) => OnFailPost(code, body));
        }

        #region onLoading 
        private void OnFetchData(List<CommentJsonData> comments) {

            int prevCountItems = _commentsContainer.Count();
            _commentsContainer.Add(comments);

            int count = _commentsContainer.Count();
            int newCount = _commentsContainer.Count() - prevCountItems;

            onDataFetched?.Invoke(count, newCount);
            if (_afterRefresh) {
                _afterRefresh = false;
                onRefresh?.Invoke();
            }
        }

        private void OnFetchTotalCommentsCount(int count) {
            onFetchedTotalCommentsCount?.Invoke(count);
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