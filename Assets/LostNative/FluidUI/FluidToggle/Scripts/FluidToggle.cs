using JetBrains.Annotations;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.UI;

namespace LostNative.Toolkit.FluidUI
{
    public class FluidToggle : MonoBehaviour
    {
        public delegate void ToggleDelegate(bool optionASelected);
        public ToggleDelegate OnToggle;

        public bool IsOn {

            set {
                if (!isInit) {
                    Init(value);
                } else {
                    toggleSequenceAlive = false;
                    targetIsOnValue = value;

                    StartToggle(targetIsOnValue);
                    toggleSequence.Complete();
                }
            }

            get { return isOn; }

        }

        [SerializeField] protected RectTransform toggleRectTransform;
        [SerializeField] protected Image toggleContainerImage;
        [SerializeField] protected Image toggleLeftImage;
        [SerializeField] protected Image toggleCenterImage;
        [SerializeField] protected Image toggleRightImage;

        [Header("Toggle Colors")]

        [SerializeField] protected Color toggleContainerColor;
        [SerializeField] protected Color toggleColor;
        [SerializeField] protected Color optionALabelColor;
        [SerializeField] protected Color optionBLabelColor;

        [Header("Toggle Label")]


        [SerializeField] protected TextMeshProUGUI toggleLabel;
        [SerializeField] protected List<MaskableGraphic> elementColors;
        [SerializeField] protected string optionAText;
        [SerializeField] protected string optionBText;

        [Header("Animation Variables")]

        [SerializeField] protected float stretchTime = 0.2f;
        [SerializeField] protected float movementTime = 0.25f;
        [SerializeField] protected float compressedTime = 0.2f;
        [SerializeField] protected float stretchedSize = 117f;
        [SerializeField] protected float compressedSize = 35f;

        private float targetXPositionA;
        private float targetXPositionB;

        private float pivotResetPositionA;
        private float pivotResetPositionB;

        private const float RightPivot = 0f;
        private const float LeftPivot = 1f;

        private Sequence toggleSequence;

        private bool toggleSequenceAlive;
        private bool targetIsOnValue;
        private bool isOn;
        private bool isInit;
        public Toggle.ToggleEvent onValueChanged = new Toggle.ToggleEvent();

        private void Awake()
        {
            if(!isInit)
                Init(false);
        }

        public void Init(bool initialValue)
        {
            toggleSequenceAlive = false;
            targetIsOnValue = initialValue;

            HandleSizing();

            HandleColors();

            StartToggle(targetIsOnValue);

            toggleSequence.Complete();

            isInit = true;
        }


        private void HandleSizing()
        {
            pivotResetPositionA = toggleRectTransform.anchoredPosition.x;
            targetXPositionB = toggleRectTransform.anchoredPosition.x * -1f;

            toggleRectTransform.sizeDelta = new Vector2(compressedSize, toggleRectTransform.sizeDelta.y);

            targetXPositionA = toggleRectTransform.anchoredPosition.x - toggleRectTransform.sizeDelta.x;

            pivotResetPositionB = targetXPositionB - toggleRectTransform.sizeDelta.x;
        }

        private void HandleColors()
        {
            toggleContainerImage.color = toggleContainerColor;

            toggleLeftImage.color = toggleColor;
            toggleCenterImage.color = toggleColor;
            toggleRightImage.color = toggleColor;
        }

        protected void StartToggle(bool targetOptionA)
        {
            var targetXPosition = targetOptionA ? targetXPositionA : targetXPositionB;

            UpdateToggleLabel();

            StartToggleSequence(targetXPosition);
        }

        private void UpdateToggleLabel()
        {
            toggleLabel.text = isOn ? optionAText : optionBText;

            var color = isOn ? optionALabelColor : optionBLabelColor;
            foreach (var element in elementColors) {
                element.color = color;
            }
        }

        protected virtual void StartToggleSequence(float targetXPosition)
        {
            toggleSequence = DOTween.Sequence();

            var positionTween = toggleRectTransform.DOAnchorPosX(targetXPosition, movementTime);
            var stretchTween = toggleRectTransform.DOSizeDelta(new Vector2(stretchedSize, toggleRectTransform.sizeDelta.y), stretchTime);
            var compressTween = toggleRectTransform.DOSizeDelta(new Vector2(compressedSize, toggleRectTransform.sizeDelta.y), compressedTime);

            toggleSequence.Insert(0f, positionTween);
            toggleSequence.Insert(0f, stretchTween);
            toggleSequence.Insert(stretchTime / 2, compressTween);

            toggleSequence.OnComplete(OnToggleSequenceComplete);

            toggleSequence.Play();

            toggleSequenceAlive = true;
        }

        private void OnToggleSequenceComplete()
        {
            ResetPositionAtPivot();
            isOn = targetIsOnValue;
            toggleSequenceAlive = false;
            UpdateToggleLabel();
            onValueChanged.Invoke(isOn);
            Debug.Log(isOn);

            /*ResetPositionAtPivot();

            optionASelected = !optionASelected;

            toggleSequenceAlive = false;

            UpdateToggleLabel();

            onValueChanged.Invoke(optionASelected);
            Debug.Log(optionASelected); */
        }

        private void ResetPositionAtPivot()
        {
            if (targetIsOnValue)
            {
                toggleRectTransform.pivot = new Vector2(LeftPivot, toggleRectTransform.pivot.y);
                toggleRectTransform.anchoredPosition = new Vector2(pivotResetPositionA, toggleRectTransform.anchoredPosition.y);
            }
            else
            {
                toggleRectTransform.pivot = new Vector2(RightPivot, toggleRectTransform.pivot.y);
                toggleRectTransform.anchoredPosition = new Vector2(pivotResetPositionB, toggleRectTransform.anchoredPosition.y);
            }
        }

        [UsedImplicitly]
        public virtual void OnToggleClick()
        {
            FinishCurrentToggle();

            targetIsOnValue = !isOn;

            StartToggle(targetIsOnValue);

            OnToggle?.Invoke(targetIsOnValue);
        }

        protected void FinishCurrentToggle()
        {
            //Debug.Log("FinishCurrentToggle");
            if (toggleSequenceAlive)
                toggleSequence.Complete();
        }
    }
}