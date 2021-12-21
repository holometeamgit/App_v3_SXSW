using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

/// <summary>
/// Counter. Spawn numbers before something and then request event
/// </summary>
public class Counter : MonoBehaviour {
    [SerializeField]
    private Transform _counterSpawn;
    [SerializeField]
    private GameObject _txtCounter;
    [SerializeField]
    private TMP_Text _startTimerValue;
    [SerializeField]
    private int _timerSize = 5;

    [SerializeField] private float delayTimer = 2;

    public UnityEvent OnOneSecondBeforeFinish;
    public UnityEvent OnFinish;
    private Coroutine coroutine;

    /// <summary>
    /// set time for counting 
    /// </summary>
    /// <param name="timerSize"></param>
    public void SetCounterTime(int timerSize) {
        _timerSize = timerSize;
    }

    private void OnEnable() {
        coroutine = StartCoroutine(StartCounting());
        if (_startTimerValue != null) {
            _startTimerValue.text = _timerSize.ToString();
        }
    }

    private void OnDisable() {
        if (coroutine != null) {
            StopCoroutine(coroutine);
            coroutine = null;
        }
        if (_startTimerValue != null) {
            _startTimerValue.text = "";
            _startTimerValue.gameObject.SetActive(true);
        }
    }

    private IEnumerator StartCounting() {
        int time = _timerSize;

        if (_startTimerValue != null) {
            yield return new WaitForSeconds(delayTimer);
            _startTimerValue.gameObject.SetActive(false);
        }

        while (time > 0) {
            GameObject txtCounterGO = Instantiate(_txtCounter, _counterSpawn);
            TextCounter textCounter = txtCounterGO.GetComponent<TextCounter>();
            textCounter.Text = time.ToString();
            time--;
            if (time == 0) {
                OnOneSecondBeforeFinish?.Invoke();
            }
            yield return new WaitForSeconds(1);
        }

        OnFinish?.Invoke();
    }
}
