namespace WindowManager.Extenject {

    /// <summary>
    /// Signal for Show/Hide UI
    /// </summary>
    public class ViewSignal {
        public bool Status {
            get {
                return _status;
            }
        }

        private bool _status;

        public ViewSignal(bool status) {
            _status = status;
        }
    }
}
