using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Beem.Content;

namespace Beem {

    public class CommentsControllersConstructor : MonoBehaviour {
        [SerializeField] WebRequestHandler _webRequestHandler;
        [SerializeField] VideoUploader _videoUploader;
        [SerializeField] PnlComments _pnlComments;

        CommentsController _commentsController;
        private void Awake() {
            _commentsController = new CommentsController(_webRequestHandler, _videoUploader);
            _pnlComments.onOpen += _commentsController.Init;
            _pnlComments.onClose += _commentsController.StopLoading;
            _pnlComments.onGetItemByOrdinalIndex += _commentsController.GetData;
            _pnlComments.onLoadNext += _commentsController.Next;
            _pnlComments.onRefresh += _commentsController.Refresh;
            _pnlComments.onPost += _commentsController.Post;

            _commentsController.onDataFetched += _pnlComments.OnApplyDataTo;
            _commentsController.onAllDataLoaded += _pnlComments.OnAllDataLoaded;
            _commentsController.onPosted += _pnlComments.OnPost;
            _commentsController.onFailPosted += _pnlComments.OnFailPost;
            _commentsController.onFetchedTotalCommentsCount += _pnlComments.OnRefreshUpdateCommentsCount;
            _commentsController.onRefresh += _pnlComments.DoOnModelRefreshed;

            StreamCallBacks.onOpenComment += _pnlComments.OpenComments;
        }

        private void OnDestroy() {
            _pnlComments.onOpen -= _commentsController.Init;
            _pnlComments.onClose -= _commentsController.StopLoading;
            _pnlComments.onGetItemByOrdinalIndex -= _commentsController.GetData;
            _pnlComments.onLoadNext -= _commentsController.Next;
            _pnlComments.onRefresh -= _commentsController.Refresh;
            _pnlComments.onPost -= _commentsController.Post;

            _commentsController.onDataFetched -= _pnlComments.OnApplyDataTo;
            _commentsController.onAllDataLoaded -= _pnlComments.OnAllDataLoaded;
            _commentsController.onPosted -= _pnlComments.OnPost;
            _commentsController.onFailPosted -= _pnlComments.OnFailPost;
            _commentsController.onFetchedTotalCommentsCount -= _pnlComments.OnRefreshUpdateCommentsCount;

            StreamCallBacks.onOpenComment -= _pnlComments.OpenComments;
        }
    }
}