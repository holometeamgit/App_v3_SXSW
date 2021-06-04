using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ChatBtn : MonoBehaviour, IPointerDownHandler {

    [SerializeField]
    private bool isOpened;

    public static Action<bool> onOpen = delegate { };
    public void OnPointerDown(PointerEventData eventData) {
        onOpen?.Invoke(isOpened);
    }
}
