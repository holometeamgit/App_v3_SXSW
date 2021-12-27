using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// AR Hint View
/// </summary>
public class ARHintView : MonoBehaviour {

    [SerializeField]
    private List<HintObjects> _hintObjects = new List<HintObjects>();

    private ARHint _arHint = new ARHint();

    private void OnEnable() {
        _arHint.onChangedState += ChangeState;
    }

    private void OnDisable() {
        _arHint.onChangedState -= ChangeState;
    }

    private void ChangeState() {
        _hintObjects.ForEach(x => x.HintStateObjects.SetActive(x.HintState == _arHint.CurrentState));
    }

    /// <summary>
    /// Hint Objects
    /// </summary>
    [Serializable]
    public class HintObjects {
        [SerializeField]
        private ARHint.State _hintState;

        public ARHint.State HintState {
            get {
                return _hintState;
            }
        }

        [SerializeField]
        private GameObject _hintStateObjects;

        public GameObject HintStateObjects {
            get {
                return _hintStateObjects;
            }
        }
    }

}
