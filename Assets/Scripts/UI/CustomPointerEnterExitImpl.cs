using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.EventSystems;


public class CustomPointerEnterExitImpl : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    private Action<GameObject> onPointerEnterCallback;
    private Action<GameObject> onPointerExitCallback;

    private GameObject onPointerEnterCallbackTarget;
    private GameObject onPointerExitCallbackTarget;

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData) {
        onPointerEnterCallback?.Invoke(onPointerEnterCallbackTarget);
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData) {
        onPointerExitCallback?.Invoke(onPointerExitCallbackTarget);
    }

    public void AddOnPointerEnterCallback(Action<GameObject> callback, GameObject callbackTarget) {
        onPointerEnterCallback = callback;
        onPointerEnterCallbackTarget = callbackTarget;
    }

    public void AddOnPointerExitCallback(Action<GameObject> callback, GameObject callbackTarget) {
        onPointerExitCallback = callback;
        onPointerExitCallbackTarget = callbackTarget;
    }
}
