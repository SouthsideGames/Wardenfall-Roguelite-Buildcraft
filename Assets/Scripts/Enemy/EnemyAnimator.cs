using UnityEngine;

public class EnemyAnimator : MonoBehaviour
{
    [Header("VISUAL TARGET:")]
    [SerializeField] private Transform visualRoot;

    private LTDescr moveTween;
    private bool isMoving = false;
    public Transform GetVisual() => visualRoot;

    public void PlayMoveAnimation()
    {
        if (isMoving) return;

        isMoving = true;
        moveTween = LeanTween.scale(visualRoot.gameObject, new Vector3(1.05f, 0.95f, 1f), 0.25f)
            .setEaseInOutSine()
            .setLoopPingPong()
            .setOnComplete(() => isMoving = false);
    }

    public void StopMoveAnimation()
    {
        if (moveTween != null) LeanTween.cancel(visualRoot.gameObject);
        visualRoot.localScale = Vector3.one;
        isMoving = false;
    }

    public void PlayAttackAnimation()
    {
        LeanTween.cancel(visualRoot.gameObject);
        visualRoot.localScale = Vector3.one;

        LeanTween.scale(visualRoot.gameObject, new Vector3(1.2f, 0.8f, 1f), 0.1f).setEaseOutQuad().setOnComplete(() =>
        {
            LeanTween.scale(visualRoot.gameObject, Vector3.one, 0.1f).setEaseInQuad();
        });
    }

    public void PlayGroggyMove()
    {
        if (isMoving) return;

        isMoving = true;
        moveTween = LeanTween.scale(visualRoot.gameObject, new Vector3(0.95f, 1.05f, 1f), 0.3f)
            .setEaseInOutSine()
            .setLoopPingPong()
            .setOnComplete(() => isMoving = false);
    }

    public void PlayPanicGrow(float intensity)
    {
        LeanTween.cancel(visualRoot.gameObject);
        visualRoot.localScale = Vector3.one * (1f + 0.3f * intensity);
    }

    public void ResetVisual()
    {
        LeanTween.cancel(visualRoot.gameObject);
        visualRoot.localScale = Vector3.one;
    }

}
