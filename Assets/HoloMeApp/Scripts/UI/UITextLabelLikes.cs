using UnityEngine;
using Beem.SSO;
using TMPro;
using UnityEngine.UI;

namespace Beem.UI {

    public class UITextLabelLikes : MonoBehaviour, IStreamDataView {
        [SerializeField] TMP_Text likesCount;
        [SerializeField] private bool emptyTextIfZero;
        [SerializeField] Image imageToDisableIfZero;

        private long _count;
        private long _streamId = -1;


        /// <summary>
        /// Init state. Take data from local data container
        /// </summary>
        public void Init(StreamJsonData.Data streamData) {
            Init(streamData.id);
        }

        public void Init(long streamId) {

            CallBacks.onGetLikeStateCallBack -= UpdateState;

            _streamId = streamId;
            CallBacks.onGetLikeStateCallBack += UpdateState;
            GetLikesState();
            UpdateUIState();
        }

        private void Start() {
            UpdateUIState();
        }

        private void RefreshLocalLikes(long streamId) {
            if (_streamId != streamId)
                return;

            GetLikesState();
        }

        private void UpdateState(long streamId, bool isLike, long count) {
            if (streamId != _streamId)
                return;

            if (_count != count) {
                _count = count;
                UpdateUIState();
            }
        }

        private void UpdateUIState() {
            likesCount.text = emptyTextIfZero && _count == 0 ? "" : _count.ToString();
            
            if (imageToDisableIfZero) {
                imageToDisableIfZero.enabled = _count > 0;
            }
        }

        private void GetLikesState() {
            if (!isActiveAndEnabled && _streamId >= 0)
                return;
            CallBacks.onGetLikeState?.Invoke(_streamId);
        }

        private void OnEnable() {
            CallBacks.onStreamByIdInContainerUpdated += RefreshLocalLikes;
            GetLikesState();
        }

        private void OnDisable() {
            _streamId = -1;
            CallBacks.onStreamByIdInContainerUpdated -= RefreshLocalLikes;
            StopAllCoroutines();
        }
    }
}