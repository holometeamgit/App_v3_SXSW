using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Beem.Extenject.UI {

    /// <summary>
    /// General pop up
    /// </summary>
    public class GeneralPopUp : MonoBehaviour, IShow {

        [SerializeField]
        private TMP_Text _titleTxt;

        [SerializeField]
        private TMP_Text _descriptionTxt;

        [SerializeField]
        private ButtonData _closeBtnData;

        [SerializeField]
        private ButtonData _funcBtnData;

        public void Show<T>(T parameter) {
            if (parameter is GeneralPopUpData) {
                GeneralPopUpData data = parameter as GeneralPopUpData;
                _titleTxt.text = data.Title;
                _descriptionTxt.text = data.Description;

                _closeBtnData.ButtonObj.SetActive(data.CloseBtnData != null);
                if (data.CloseBtnData != null) {
                    _closeBtnData.TitleTxt.text = data.CloseBtnData.Title;
                    _closeBtnData.ClickBtn.onClick.RemoveAllListeners();
                    _closeBtnData.ClickBtn.onClick.AddListener(() => data.CloseBtnData.OnClick?.Invoke());
                }

                _funcBtnData.ButtonObj.SetActive(data.FuncBtnData != null);
                if (data.FuncBtnData != null) {
                    _funcBtnData.TitleTxt.text = data.FuncBtnData.Title;
                    _funcBtnData.ClickBtn.onClick.RemoveAllListeners();
                    _funcBtnData.ClickBtn.onClick.AddListener(() => data.FuncBtnData.OnClick?.Invoke());
                }
            }
        }

        [Serializable]
        public class ButtonData {
            [SerializeField]
            private GameObject _buttonObj;
            [SerializeField]
            private TMP_Text _titleTxt;
            [SerializeField]
            private Button _clickBtn;

            public GameObject ButtonObj {
                get {
                    return _buttonObj;
                }
            }

            public TMP_Text TitleTxt {
                get {
                    return _titleTxt;
                }
            }

            public Button ClickBtn {
                get {
                    return _clickBtn;
                }
            }
        }
    }
}
