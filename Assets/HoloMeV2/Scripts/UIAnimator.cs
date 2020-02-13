using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UnityEngine.UI;

public class UIAnimator : MonoBehaviour
{
    [SerializeField]
    Sprite[] frames;

    [SerializeField]
    Image imgToAnimate;

    [SerializeField]
    string spriteResourceFolderDir;

    [SerializeField]
    float speed = 0.1f;

    int frameIndex = 0;

    void Awake()
    {
        frames = new List<Sprite>(Resources.LoadAll<Sprite>(@spriteResourceFolderDir)).OrderBy(x => Convert.ToInt32(x.name)).ToArray();
    }

    private void OnEnable()
    {
        frameIndex = 0;
        StartCoroutine(Animate());
    }

    IEnumerator Animate()
    {
        while (gameObject.activeSelf)
        {
            yield return new WaitForSeconds(speed);
            imgToAnimate.sprite = frames[frameIndex++];

            if (frameIndex == frames.Length - 1)
            {
                frameIndex = 0;
            }
        }
    }

}
