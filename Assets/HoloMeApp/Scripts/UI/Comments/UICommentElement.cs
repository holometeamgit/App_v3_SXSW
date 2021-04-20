using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using Beem;
using UnityEngine.UI;

namespace Beem.UI {

    public class UICommentElement : MonoBehaviour {

        [SerializeField] float _maxWidth = 932;

        public Action<int> onRemoveByContentId;

        [Header("Comments")]
        [SerializeField] TMP_Text _titleText;
        [SerializeField] TMP_Text _commentText;
        [SerializeField] Color _commentPostedColor;

        [Header("Likes")]
        [SerializeField] TMP_Text _likesCountText;
        [SerializeField] Color _likesColor;
        [SerializeField] Color _unlikesColor;
        [SerializeField] Sprite _unlikeTexture;
        [SerializeField] Sprite _likeTexture;
        [SerializeField] Image _imgLike;

        private const int ADDITIONAL_CHARACTERS_LENGS = 6;
        //ADDITIONAL SPACING BTW TITLE AND COMMENTS
        private const int ADDITIONAL_SPACING = 18;

        //private void Start() {
        //    UpdateData("WWWWWWWWWWWWWWWWWWW", "This is so cool, how did they even do this? Would love to find out mor", DateTime.Now.ToUniversalTime().AddDays(-1), 2, 2);
        //}

        /// <summary>
        /// element id rom model
        /// </summary>
        private int _contentId;

        public void UpdateData(string title, string commentText, DateTime dateTime, int likesCount, int contentId) {
            _titleText.text = title;
            _commentText.text = string.Format("{0} <color=#{1}>{2}</color> ",
                commentText, ColorUtility.ToHtmlStringRGB(_commentPostedColor), TimeSpanString.GetTimeSince(dateTime));
            _contentId = contentId;
            _likesCountText.text = likesCount.ToString();

            _imgLike.sprite = likesCount > 0 ? _likeTexture : _unlikeTexture;
            _imgLike.color = likesCount > 0 ? _likesColor : _unlikesColor;
        }

        public void RemoveItem() {
            onRemoveByContentId?.Invoke(_contentId);
        }

        public float GetRequiredHeight(string title, string comments) {
            int countTitleStr = Mathf.CeilToInt(((float)(_titleText.fontSize * title.Length)) / _maxWidth);
            int countCommentStr = Mathf.CeilToInt(((float)(_commentText.fontSize * comments.Length + ADDITIONAL_CHARACTERS_LENGS)) / _maxWidth);

            float height = _titleText.fontSize * countTitleStr + _titleText.lineSpacing * (countTitleStr - 1) + ADDITIONAL_SPACING +
                _commentText.fontSize * countCommentStr + _commentText.lineSpacing * (countCommentStr - 1);

            return height;
        }
    }
}
