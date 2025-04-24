using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class QTE_TapTarget : MonoBehaviour, IPointerClickHandler
{
    private float timer;
    private bool tapped = false;
    private Action onSuccess;
    private Action onFail;

    public void Init(Action successCallback, Action failCallback, float timeToTap)
    {
        timer = timeToTap;
        onSuccess = successCallback;
        onFail = failCallback;
    }

    private void Update()
    {
        if (tapped) return;

        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            onFail?.Invoke();
            Destroy(gameObject);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (tapped) return;
        tapped = true;
        onSuccess?.Invoke();
        Destroy(gameObject);
    }
}
