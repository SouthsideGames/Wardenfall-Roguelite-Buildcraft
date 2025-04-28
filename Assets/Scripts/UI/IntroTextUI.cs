using UnityEngine;

public class IntroTextUI : MonoBehaviour
{
    [SerializeField] private float scrollSpeed = 20f;
    [SerializeField] private float fastScrollMultiplier = 3f;
    [SerializeField] private RectTransform textTransform;
    [SerializeField] private float endYPosition = 1000f; 

    private bool isSkipping = false;

    void Update()
    {
        if (isSkipping) return;

        float speed = scrollSpeed;
        
        if (Input.GetMouseButton(0) || Input.touchCount > 0)
            speed *= fastScrollMultiplier;

        textTransform.anchoredPosition += Vector2.up * speed * Time.deltaTime;

        if (textTransform.anchoredPosition.y >= endYPosition)
            FinishIntro();
    }

    public void SkipIntro()
    {
        isSkipping = true;
        FinishIntro();
    }

    private void FinishIntro() => gameObject.SetActive(false);
}
