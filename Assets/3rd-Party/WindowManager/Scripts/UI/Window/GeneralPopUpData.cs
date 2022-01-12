using System;

namespace WindowManager.Extenject {

    /// <summary>
    /// General data for all popups
    /// </summary>
    public class GeneralPopUpData {

        private string _title = default;
        private string _description = default;
        private ButtonData _cancelBtnData = default;
        private ButtonData _funcBtnData = default;

        public string Title {
            get {
                return _title;
            }
        }

        public string Description {
            get {
                return _description;
            }
        }

        public ButtonData CloseBtnData {
            get {
                return _cancelBtnData;
            }
        }

        public ButtonData FuncBtnData {
            get {
                return _funcBtnData;
            }
        }

        public GeneralPopUpData(string title, string description, ButtonData cancelBtnData) {
            _title = title;
            _description = description;
            _cancelBtnData = cancelBtnData;
        }

        public GeneralPopUpData(string title, string description, ButtonData cancelBtnData, ButtonData funcBtnData) {
            _title = title;
            _description = description;
            _cancelBtnData = cancelBtnData;
            _funcBtnData = funcBtnData;
        }

        public class ButtonData {
            private string _title = default;
            private Action _onClick = delegate { };

            public string Title {
                get {
                    return _title;
                }
            }

            public Action OnClick {
                get {
                    return _onClick;
                }
            }

            public ButtonData(string title, Action onClick) {
                _title = title;
                _onClick = onClick;
            }
        }

    }
}
