using UnityEngine;
using TMPro;

public class StatusEffectIcon : MonoBehaviour
{
    [SerializeField] private SpriteRenderer iconRenderer;
    [SerializeField] private TextMeshPro stackText;
    [SerializeField] private Transform durationBarTransform;

    private StatusEffect currentEffect;
    private Vector3 initialBarScale;

    private ParticleSystem effectParticles;
    private Color effectColor;

    private void Awake()
    {
        if (durationBarTransform != null)
            initialBarScale = durationBarTransform.localScale;
    }

    private void Start()
    {
        effectParticles = GetComponentInChildren<ParticleSystem>();
        if (currentEffect != null)
        {
            UpdateEffectColor(currentEffect.EffectType);
        }
    }

    private void Update()
    {
        if (currentEffect != null && currentEffect.Duration > 0f && durationBarTransform != null)
        {
            float ratio = Mathf.Clamp01(currentEffect.Duration / (currentEffect.Duration + 0.01f));
            durationBarTransform.localScale = new Vector3(initialBarScale.x * ratio, initialBarScale.y, initialBarScale.z);
        }
    }

    public void Initialize(StatusEffect effect)
    {
        currentEffect = effect;
        stackText.text = $"x{effect.StackCount}";
        UpdateIcon(effect.EffectType);
        UpdateEffectColor(effect.EffectType);
        if (durationBarTransform != null)
            durationBarTransform.localScale = initialBarScale;
    }

    public void UpdateEffect(StatusEffect effect)
    {
        currentEffect = effect;
        stackText.text = $"x{effect.StackCount}";
        UpdateEffectColor(effect.EffectType);
    }

    public void PlayDispelAnimation()
    {
        // Scale pop
        LeanTween.scale(gameObject, Vector3.one * 1.2f, 0.1f).setEasePunch();

        // Particle burst
        if (effectParticles != null)
        {
            var main = effectParticles.main;
            main.startColor = effectColor;
            effectParticles.Play();
        }
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

    private void UpdateEffectColor(StatusEffectType type)
    {
        switch (type)
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
            case StatusEffectType.Drain:
                effectColor = new Color(0.6f, 0f, 0.9f, 1f);
                break;
            default:
                effectColor = Color.white;
                break;
        }

        if (effectParticles != null)
        {
            var main = effectParticles.main;
            main.startColor = effectColor;
        }

        if (iconRenderer != null)
            iconRenderer.color = effectColor;
    }

    /// <summary>
    /// Sets up the icon as a temporary visual (e.g. from a corruption pulse), floating upward and fading out.
    /// </summary>
    public void SetupTemporary(StatusEffectType type)
    {
        stackText.gameObject.SetActive(false);
        if (durationBarTransform != null)
            durationBarTransform.gameObject.SetActive(false);

        UpdateIcon(type);
        UpdateEffectColor(type);

        LeanTween.moveY(gameObject, transform.position.y + 1.2f, 1f).setEaseOutQuad();
        LeanTween.alpha(gameObject, 0f, 1f).setOnComplete(() => Destroy(gameObject));
        LeanTween.scale(gameObject, Vector3.one * 1.3f, 0.2f).setEaseOutBack();
    }
}