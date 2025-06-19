using UnityEngine;

public class EnemyAnimator : MonoBehaviour
{
    [Header("VISUAL TARGET:")]
    [SerializeField] private Transform visualRoot;

    private LTDescr moveTween;
    private bool isMoving = false;
    public Transform GetVisual() => visualRoot;

    private bool isIdlePulsing = false;
    private bool isEnraged = false;


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

    public void PlayExplosionPulse(float pulseScale = 1.2f, float duration = 0.2f)
    {
        LeanTween.cancel(visualRoot.gameObject);
        LeanTween.scale(visualRoot.gameObject, Vector3.one * pulseScale, duration)
            .setEaseOutQuad()
            .setLoopPingPong(2);
    }

    public void ResetVisual()
    {
        LeanTween.cancel(visualRoot.gameObject);
        visualRoot.localScale = Vector3.one;
        isIdlePulsing = false;
        isEnraged = false;
    }

    public void PlayIdlePulse()
    {
        if (isIdlePulsing) return;

        isIdlePulsing = true;
        moveTween = LeanTween.scale(visualRoot.gameObject, new Vector3(1.05f, 0.95f, 1f), 0.5f)
            .setEaseInOutSine()
            .setLoopPingPong()
            .setOnComplete(() => isIdlePulsing = false);
    }

    public void PlayEnrageBurst()
    {
        if (visualRoot == null) return;

        LeanTween.cancel(visualRoot.gameObject);
        visualRoot.localScale = Vector3.one;

        // Flash red and grow briefly
        LeanTween.color(visualRoot.gameObject, Color.red, 0.1f).setLoopPingPong(2);
        LeanTween.scale(visualRoot.gameObject, Vector3.one * 1.2f, 0.2f)
            .setEaseOutBack()
            .setOnComplete(() => visualRoot.localScale = Vector3.one);

        isEnraged = true;
    }

    public void PlayEnragePulse()
    {
        if (!isEnraged) return;

        // Persistent red pulse aura effect
        float pulse = Mathf.PingPong(Time.time * 3f, 1f) * 0.25f;
        SpriteRenderer renderer = visualRoot.GetComponent<SpriteRenderer>();
        if (renderer != null)
            renderer.color = Color.Lerp(Color.white, Color.red, pulse);
    }

    public void PlayAttackBurst()
    {
        if (visualRoot == null) return;

        LeanTween.cancel(visualRoot.gameObject);
        LeanTween.scale(visualRoot.gameObject, new Vector3(1.2f, 0.8f, 1f), 0.08f).setEaseOutSine().setOnComplete(() =>
        {
            LeanTween.scale(visualRoot.gameObject, Vector3.one, 0.1f).setEaseInSine();
        });
    }

    public void PlayCorruptionPulseAnimation()
    {
        LeanTween.scale(visualRoot.gameObject, new Vector3(1.25f, 1.25f, 1f), 0.2f)
            .setEaseOutBack()
            .setLoopPingPong(1)
            .setOnComplete(() => visualRoot.localScale = Vector3.one);
    }
    
    public void PlayPrePulseShake()
    {
        Vector3 originalPos = visualRoot.localPosition;
        LeanTween.moveLocalX(visualRoot.gameObject, originalPos.x + 0.1f, 0.05f).setLoopPingPong(4).setOnComplete(() =>
        {
            visualRoot.localPosition = originalPos;
        });
    }
    
    public void PlayIdlePulseAnimation()
    {
        if (isIdlePulsing) return;
        isIdlePulsing = true;
    
        LeanTween.scale(visualRoot.gameObject, new Vector3(1.05f, 0.95f, 1f), 1f)
            .setEaseInOutSine()
            .setLoopPingPong()
            .setOnComplete(() => isIdlePulsing = false);
    }
}
