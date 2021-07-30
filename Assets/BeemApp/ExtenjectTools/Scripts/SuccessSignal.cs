namespace Beem.Extenject {
    /// <summary>
    /// Request Success
    /// </summary>
    public class SuccessSignal<T> : BeemSignal {
        private T _requester;

        public T Requester {
            get {
                return _requester;
            }
        }

        public SuccessSignal(T requester) {
            _requester = requester;
        }

        public SuccessSignal() {
        }
    }

    /// <summary>
    /// Request Success
    /// </summary>
    public class SuccessSignal<T, K> : BeemSignal {
        private T _requester;

        public T Requester {
            get {
                return _requester;
            }
        }

        private K _result;

        public K Result {
            get {
                return _result;
            }
        }

        public SuccessSignal(T requester) {
            _requester = requester;
        }

        public SuccessSignal(K result) {
            _result = result;
        }
    }
}