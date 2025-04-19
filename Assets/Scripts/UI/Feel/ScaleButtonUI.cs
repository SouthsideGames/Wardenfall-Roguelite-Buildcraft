using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Button))]
public class ScaleButtonUI : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    private Vector3 initialScale;

    private void Awake() => initialScale = transform.localScale;

    public void OnSelect(BaseEventData eventData)
    {
        LeanTween.cancel(gameObject);
        LeanTween.scale(gameObject, initialScale * 1.05f, .25f).setLoopPingPong().setIgnoreTimeScale(true);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        LeanTween.cancel(gameObject);
        transform.localScale = initialScale;
    }


}
