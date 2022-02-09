using UnityEngine;
using Beem.Content;
using Zenject;
using System;

namespace Beem {

    public class CommentsControllersConstructor : MonoBehaviour {
        [SerializeField] VideoUploader _videoUploader;
        [SerializeField] PnlComments _pnlComments;

        private CommentsController _commentsController;
        private WebRequestHandler _webRequestHandler;

        public static Action<StreamJsonData.Data> OnShow = delegate { };
        public static Action OnHide = delegate { };

        [Inject]
        public void Construct(WebRequestHandler webRequestHandler) {
            _webRequestHandler = webRequestHandler;
        }

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

            OnShow += _pnlComments.Show;
            OnHide += _pnlComments.Hide;
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

            OnShow -= _pnlComments.Show;
            OnHide -= _pnlComments.Hide;
        }
    }
}