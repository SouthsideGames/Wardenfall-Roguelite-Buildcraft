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

    private ParticleSystem effectParticles;
    private Color effectColor;

    private void Start()
    {
        effectParticles = GetComponentInChildren<ParticleSystem>();
        UpdateEffectColor();
    }

    public void PlayDispelAnimation()
    {
        // Scale animation
        LeanTween.scale(gameObject, Vector3.one * 1.2f, 0.1f).setEasePunch();
        
        // Particle burst
        if (effectParticles != null)
        {
            var main = effectParticles.main;
            main.startColor = effectColor;
            effectParticles.Play();
        }
    }

    private void UpdateEffectColor()
    {
        switch (currentEffect.EffectType)
        {
            case StatusEffectType.Burn:
                effectColor = new Color(1f, 0.5f, 0f, 1f);
                break;
            case StatusEffectType.Poison:
                effectColor = new Color(0f, 1f, 0f, 1f);
                break;
            case StatusEffectType.Freeze:
                effectColor = new Color(0f, 1f, 1f, 1f);
                break;
            default:
                effectColor = Color.white;
                break;
        }
    }

    public void UpdateEffect(StatusEffect effect)
    {
        currentEffect = effect;
        stackText.text = $"x{effect.StackCount}";
        UpdateEffectColor();
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
