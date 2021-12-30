using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Beem.Extenject.Hologram {
    /// <summary>
    /// AR Hint for Target
    /// </summary>
    public class ARHintTarget : AbstractARHint {

        [SerializeField]
        private Animator _hintAnimator;

        protected override void Refresh() {
            _hintAnimator.SetBool("Hologram", _arObjectWasCreated);
            _hintAnimator.SetBool("Active", _arActive && !_arObjectWasPinched && _arPlanesDetected);
        }

    }
}
