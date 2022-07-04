using UnityEngine;
using Beem.Content;
using Zenject;

namespace Beem {

    public class CommentsControllersConstructor : MonoBehaviour {
        [SerializeField] VideoUploader _videoUploader;
        [SerializeField] PnlComments _pnlComments;

        private WebRequestHandler _webRequestHandler;
        private CommentsController _commentsController;

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

            StreamCallBacks.onOpenComment += _pnlComments.OpenComments;
            StreamCallBacks.onCloseComments += _pnlComments.CloseComments;
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
            StreamCallBacks.onCloseComments -= _pnlComments.CloseComments;
        }
    }
}