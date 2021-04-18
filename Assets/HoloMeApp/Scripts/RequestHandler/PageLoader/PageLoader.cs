using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Beem.Pagination {

    public class PageLoader<T> {

        public Action onReady;
        public Action<int> onTotalCountItemsIsTaken;
        public Action onFailInit;
        public Action onAllDataLoaded;
        public Action<List<T>> onDataLoaded;
        public Action onFailDataLoaded;

        private WebRequestHandler _webRequestHandler;
        private string _hostUrl;
        private string _additionalParams;

        private int _currentPage;
        private int _pageSize;
        private int _totalCountItems;

        const int FIRST_PAGE_NUMBER = 1;

        /// <param name="hostUrl">For example https://beem.me/stream/ </param>
        /// <param name="additionalParams">Some filter parametrs</param>
        public PageLoader(string hostUrl, WebRequestHandler webRequestHandler, int pageSize, string additionalParams = "") {
            _hostUrl = hostUrl;
            _webRequestHandler = webRequestHandler;
            _additionalParams = additionalParams;
            _currentPage = FIRST_PAGE_NUMBER;
            _pageSize = pageSize;
            InitRequest();
        }

        public void NextPage() {
            _currentPage--;
            if (IsAllDataLoaded()) {
                onAllDataLoaded?.Invoke();
                return;
            }

            Request(GetRequestUrl(), OnSuccessGetNextRequest, OnFailGetNextRequest);
        }

        public bool IsAllDataLoaded() {
            return _currentPage < FIRST_PAGE_NUMBER;
        }

        private void InitRequest() {
            Request(GetRequestUrl(), OnSuccessInitRequest, OnFailInitRequest);
        }

        private List<T> DeserializeData(string data) {
            List<T> items = DeserializePageData(data)?.results;

            return items;
        }

        private PagedData<T> DeserializePageData(string data) {
            PagedData<T> pageData = null;
            try {
                pageData = JsonUtility.FromJson<PagedData<T>>(data);
            } catch (Exception e) {
                HelperFunctions.DevLogError("Can't deserialize " + data);
                HelperFunctions.DevLogError(e.Message);
            }

            return pageData;
        }

        private void Request(string url, Action<string> onSuccess, Action<string> onFail) {
            _webRequestHandler.Get(url,
                (successCode, data) => {
                    onSuccess?.Invoke(data);
                },
                (errorCode, data) => {
                    onFail?.Invoke("Get " + url + " " + errorCode + " " + data);
                });
        }

        private void OnSuccessInitRequest(string data) {
            PagedData<T> pagedData = DeserializePageData(data);
            if (pagedData == null) {
                OnFailInitRequest(data);
                return;
            }
            onTotalCountItemsIsTaken?.Invoke(pagedData.count);
            _totalCountItems = pagedData.count;
            _currentPage = Mathf.Max(Mathf.CeilToInt((float)_totalCountItems / _pageSize), 1) + 1;
            onReady?.Invoke();
        }

        private void OnFailInitRequest(string msg) {
            HelperFunctions.DevLogError(msg);
            onFailInit?.Invoke();
        }

        private void OnSuccessGetNextRequest(string data) {
            onDataLoaded?.Invoke(DeserializeData(data));
        }

        private void OnFailGetNextRequest(string msg) {
            onFailDataLoaded?.Invoke();
        }

        private string GetRequestUrl() {
            string resultUrl = _hostUrl +
                PageParameters.Page + "=" + _currentPage + "&" +
                PageParameters.PageSize + "=" + _pageSize;

            if (!string.IsNullOrWhiteSpace(_additionalParams))
                resultUrl += ("&" + _additionalParams);

            return resultUrl;
        }
    }
}