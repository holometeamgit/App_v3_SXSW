using System.Collections;
using UnityEngine;
using SuperBlur;
using System;

[RequireComponent(typeof(SuperBlurFast))]
public class BlurController : MonoBehaviour
{
    SuperBlurFast superBlurFast;

    public Action OnBlurRemoved;
    public Action OnBlurAdded;

    [SerializeField]
    float speed = .125f;

    bool blurIsActive = true;

    void Start()
    {
        superBlurFast = GetComponent<SuperBlurFast>();
    }

    public void RemoveBlur()
    {
        if (!blurIsActive)
            return;

        StopAllCoroutines();
        StartCoroutine(RemoveBlurRoutine());
        blurIsActive = false;
    }

    public void AddBlur()
    {
        if (blurIsActive)
            return;
        StopAllCoroutines();
        StartCoroutine(AddBlurRoutine());
        blurIsActive = true;
    }

    IEnumerator RemoveBlurRoutine()
    {
        while (superBlurFast.downsample != 0)
        {
            yield return new WaitForSeconds(speed);
            superBlurFast.downsample -= 1;
        }
        superBlurFast.enabled = false;
        OnBlurRemoved?.Invoke();
    }

    IEnumerator AddBlurRoutine()
    {
        superBlurFast.enabled = true;
        while (superBlurFast.downsample < 4)
        {
            yield return new WaitForSeconds(speed);
            superBlurFast.downsample += 1;
        }
        OnBlurAdded?.Invoke();
    }
}
