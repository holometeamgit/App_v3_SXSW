using UnityEngine;
using UnityEngine.UI;
using Beem.SSO;
using TMPro;

namespace Beem.UI {


    /// <summary>
    /// Need for demonstration count of likes
    /// can invoke like and unlike event
    /// </summary>
    public class UIBtnLikes : MonoBehaviour {
        [SerializeField] Image imageLike;
        [SerializeField] Image imageUnlike;
        [SerializeField] TMP_Text likesCount;

        private bool _isLike;
        private long _count;
        private long _streamId = -1;

        /// <summary>
        /// Set btn state 
        /// </summary>
        public void SetStreamId(long streamId) {
            CallBacks.onGetLikeStateCallBack -= UpdateState;

            _streamId = streamId;
            CallBacks.onGetLikeStateCallBack += UpdateState;

            CallBacks.onGetLikeState?.Invoke(streamId);
        }

        /// <summary>
        /// invoke when user click on btn
        /// </summary>
        public void ClickLike() {
            HelperFunctions.DevLog("ClickLike streamId " + _streamId);
            if (_streamId < 0)
                return;
            Handheld.Vibrate();
            _isLike = !_isLike;
            _count = _isLike ? ++_count : --_count;
            UpdateBtnUIState();
            if (_isLike) {
                CallBacks.onClickLike?.Invoke(_streamId);
            } else {
                CallBacks.onClickUnlike?.Invoke(_streamId);
            }
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

        private void OnDisable() {
            _streamId = -1;
        }
    }
}