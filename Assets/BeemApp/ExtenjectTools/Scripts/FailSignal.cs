using System;

namespace Beem.Extenject {

    /// <summary>
    /// Request Fail
    /// </summary>
    public class FailSignal<T> : BeemSignal {

        private T _requester;

        public T Requester {
            get {
                return _requester;
            }
        }

        private Exception _error;

        public Exception Error {
            get {
                return _error;
            }
        }

        public FailSignal(T requester, Exception error) {
            _requester = requester;
            _error = error;
        }

        public FailSignal(Exception error) {
            _error = error;
        }

        public FailSignal(T requester) {
            _requester = requester;
        }
    }

}