using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Beem.SSO;
using TMPro;

namespace Beem.UI {

    public class UIBtnLikes : MonoBehaviour {
        [SerializeField] Image imageLike;
        [SerializeField] Image imageUnlike;
        [SerializeField] TMP_Text likesCount;

        private bool _isLike;
        private int _count;
        private long _streamId = -1;

        /// <summary>
        /// Set btn state 
        /// </summary>
        public void SetState(bool isLike, int count, long streamId) {
            _isLike = isLike;
            _count = count;
            _streamId = streamId;
            UpdateBtnState();
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
            UpdateBtnState();
            if (_isLike) {
                CallBacks.onClickLike?.Invoke(_streamId);
            } else {
                CallBacks.onClickUnlike?.Invoke(_streamId);
            }
        }

        private void UpdateBtnState() {
            imageLike.enabled = _isLike;
            imageUnlike.enabled = !_isLike;
            likesCount.text = _count.ToString();
        }

    }
}