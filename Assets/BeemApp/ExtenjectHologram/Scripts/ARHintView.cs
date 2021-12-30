using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Beem.Extenject.Hologram {

    /// <summary>
    /// AR Hint View
    /// </summary>
    public class ARHintView : AbstractARHint {

        [SerializeField]
        private GameObject _scan;

        [SerializeField]
        private GameObject _hologramPlacement;

        [SerializeField]
        private GameObject _pinch;

        protected override void Refresh() {
            _scan?.SetActive(_arActive && !_arObjectWasPinched && !_arPlanesDetected && !_arObjectWasCreated);
            _hologramPlacement?.SetActive(_arActive && !_arObjectWasPinched && _arPlanesDetected && !_arObjectWasCreated);
            _pinch?.SetActive(_arActive && !_arObjectWasPinched && _arPlanesDetected && _arObjectWasCreated);
        }


    }
}
