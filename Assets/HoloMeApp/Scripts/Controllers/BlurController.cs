using System.Collections;
using UnityEngine;
using SuperBlur;
using System;

[RequireComponent(typeof(SuperBlurFast))]
public class BlurController : MonoBehaviour
{
    SuperBlurFast superBlurFast;
    SuperBlurFast SuperBlurFast
    {
        get
        {
            if (superBlurFast == null)
            {
                superBlurFast = GetComponent<SuperBlurFast>();
            }
            return superBlurFast;
        }
    }

    public Action OnBlurRemoved;
    public Action OnBlurAdded;

    [SerializeField]
    float speed = .125f;

    bool blurIsActive = true;

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
        while (SuperBlurFast.downsample != 0)
        {
            yield return new WaitForSeconds(speed);
            SuperBlurFast.downsample -= 1;
        }
        SuperBlurFast.enabled = false;
        OnBlurRemoved?.Invoke();
    }

    IEnumerator AddBlurRoutine()
    {
        SuperBlurFast.enabled = true;
        while (SuperBlurFast.downsample < 4)
        {
            yield return new WaitForSeconds(speed);
            SuperBlurFast.downsample += 1;
        }
        OnBlurAdded?.Invoke();
    }
}
