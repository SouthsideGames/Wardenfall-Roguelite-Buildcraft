using UnityEngine;

public class PulseWard : MonoBehaviour
{
    [SerializeField] private float lifetime = 0.5f;
    [SerializeField] private AnimationCurve scaleCurve;
    [SerializeField] private float maxScale = 2f;

    private float timer = 0f;
    private Vector3 initialScale;

    private void Start()
    {
        initialScale = transform.localScale;
    }

    private void Update()
    {
        timer += Time.deltaTime;
        float progress = timer / lifetime;

        float scale = scaleCurve.Evaluate(progress) * maxScale;
        transform.localScale = initialScale * scale;

        if (timer >= lifetime)
            Destroy(gameObject);
    }
}
