using UnityEngine;
using TMPro;

public class StatusEffectIcon : MonoBehaviour
{
    [SerializeField] private SpriteRenderer iconRenderer;
    [SerializeField] private TextMeshPro stackText;
    [SerializeField] private Transform durationBarTransform;

    private StatusEffect currentEffect;
    private Vector3 initialBarScale;

    private void Awake()
    {
        if (durationBarTransform != null)
            initialBarScale = durationBarTransform.localScale;
    }

    public void Initialize(StatusEffect effect)
    {
        currentEffect = effect;
        stackText.text = $"x{effect.StackCount}";
        UpdateIcon(effect.EffectType);
        if (durationBarTransform != null)
            durationBarTransform.localScale = initialBarScale;
    }

    public void UpdateEffect(StatusEffect effect)
    {
        currentEffect = effect;
        stackText.text = $"x{effect.StackCount}";
    }

    private void Update()
    {
        if (currentEffect != null && currentEffect.Duration > 0f && durationBarTransform != null)
        {
            float ratio = Mathf.Clamp01(currentEffect.Duration / (currentEffect.Duration + 0.01f));
            durationBarTransform.localScale = new Vector3(initialBarScale.x * ratio, initialBarScale.y, initialBarScale.z);
        }
    }

    public void PlayDispelAnimation()
    {
        // Use LeanTween or scale punch for 3D pop
        LeanTween.scale(gameObject, Vector3.one * 1.2f, 0.1f).setEasePunch();
    }

    private void UpdateIcon(StatusEffectType type)
    {
        string iconPath = $"Icons/Effects/{type.ToString().ToLower()}";
        Sprite loadedSprite = Resources.Load<Sprite>(iconPath);
        if (loadedSprite != null)
        {
            iconRenderer.sprite = loadedSprite;
        }
    }
}
