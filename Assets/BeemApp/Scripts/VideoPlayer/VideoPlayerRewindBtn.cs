﻿using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Video;

namespace Beem.Video {

    /// <summary>
    /// RewindBtn
    /// </summary>
    [RequireComponent(typeof(Slider))]
    public class VideoPlayerRewindBtn : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler {

        [SerializeField]
        private UnityEvent onRewindStarted;

        [SerializeField]
        private UnityEvent<float> onRewind;

        [SerializeField]
        private UnityEvent<float> onRewindFinished;

        private Slider progress;

        private void Awake() {
            progress = GetComponent<Slider>();
        }

        public void OnDrag(PointerEventData eventData) {
            onRewind?.Invoke(progress.value);
        }


        public void OnBeginDrag(PointerEventData eventData) {
            onRewindStarted?.Invoke();
        }

        public void OnEndDrag(PointerEventData eventData) {
            onRewindFinished?.Invoke(progress.value);
        }
    }
}
