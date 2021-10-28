using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using Beem;
using UnityEngine.UI;

namespace Beem.UI {

    public class UICommentElement : MonoBehaviour {

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

        /// <summary>
        /// element id rom model
        /// </summary>
        private int _contentId;

        float _maxWidth = 932;

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
            _commentText.text = comments;
            _titleText.text = title;
            int textCommentsLineCount = Mathf.CeilToInt(_commentText.preferredWidth / _maxWidth);
            int spaceCommensLineCount = Mathf.Max(0, textCommentsLineCount - 1);

            int textTitleLineCount = Mathf.CeilToInt(_titleText.preferredWidth / _maxWidth);
            int spaceTitleLineCount = Mathf.Max(0, textTitleLineCount - 1);


            float height =
                textTitleLineCount * _commentText.fontSize +
                spaceTitleLineCount * _commentText.lineSpacing +
                ADDITIONAL_SPACING +
                textCommentsLineCount * _commentText.fontSize +
                spaceCommensLineCount * _commentText.lineSpacing;

            return height;
        }
    }
}
