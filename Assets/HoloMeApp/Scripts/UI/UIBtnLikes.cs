using UnityEngine;
using UnityEngine.UI;
using Beem.SSO;
using TMPro;
using System.Collections;

namespace Beem.UI {


    /// <summary>
    /// Need for demonstration count of likes
    /// can invoke like and unlike event
    /// </summary>
    public class UIBtnLikes : MonoBehaviour, IStreamDataView {
        [SerializeField] Image imageLike;
        [SerializeField] Image imageUnlike;
        [SerializeField] TMP_Text likesCount;

        private bool _isLike;
        private long _count;
        private long _streamId = -1;


        /// <summary>
        /// Init btn state. Take data from local data container
        /// </summary>
        public void Init(StreamJsonData.Data streamData) {
            Init(streamData.id);
        }

        public void Init(long streamId) {
#if UNITY_EDITOR
            this.gameObject.name = "BtnLikes " + streamId;
#endif
            CallBacks.onGetLikeStateCallBack -= UpdateState;

            _streamId = streamId;
            CallBacks.onGetLikeStateCallBack += UpdateState;
            GetLikesState();
            UpdateBtnUIState();
        }

        /// <summary>
        /// invoke when user click on btn
        /// </summary>
        public void ClickLike() {
            HelperFunctions.DevLog("ClickLike streamId " + _streamId);
            if (_streamId < 0)
                return;
            _isLike = !_isLike;
            _count = _isLike ? ++_count : --_count;
            UpdateBtnUIState();
            if (_isLike) {
                CallBacks.onClickLike?.Invoke(_streamId);
            } else {
                CallBacks.onClickUnlike?.Invoke(_streamId);
            }
        }

        private void Start() {
            UpdateBtnUIState();
        }

        private void RefreshLocalLikes(long streamId) {
            if (_streamId != streamId)
                return;

            GetLikesState();
        }

        private void UpdateState(long streamId, bool isLike, long count) {
            if (streamId != _streamId)
                return;

            if (_isLike != isLike || _count != count) {
                _isLike = isLike;
                _count = count;
                UpdateBtnUIState();
            }
        }

        private void UpdateBtnUIState() {
            imageLike.enabled = _isLike;
            imageUnlike.enabled = !_isLike;
            likesCount.text = _count.ToString();
        }

        private void GetLikesState() {
            if (!isActiveAndEnabled)
                return;
            CallBacks.onGetLikeState?.Invoke(_streamId);
        }

        private void OnEnable() {
            CallBacks.onStreamByIdInContainerUpdated += RefreshLocalLikes;
        }

        private void OnDisable() {
            _streamId = -1;
            CallBacks.onStreamByIdInContainerUpdated -= RefreshLocalLikes;
            StopAllCoroutines();
        }
    }
}