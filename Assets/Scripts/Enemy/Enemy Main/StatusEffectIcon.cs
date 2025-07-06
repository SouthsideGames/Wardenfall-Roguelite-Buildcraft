using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class StatusEffectIcon : MonoBehaviour
{
    [SerializeField] private SpriteRenderer iconRenderer;
    [SerializeField] private TextMeshPro stackText;
    [SerializeField] private Transform durationBarTransform;
    [SerializeField]
    private List<StatusEffectSprite> statusEffectSprites = new List<StatusEffectSprite>();

    private StatusEffect currentEffect;
    private Vector3 initialBarScale;

    private void Awake()
    {
        if (durationBarTransform != null)
            initialBarScale = durationBarTransform.localScale;
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
        if (durationBarTransform != null)
            durationBarTransform.localScale = initialBarScale;
    }

    public void UpdateEffect(StatusEffect effect)
    {
        currentEffect = effect;
        stackText.text = $"x{effect.StackCount}";
    }

    public void PlayDispelAnimation()
    {
        // Scale pop
        LeanTween.scale(gameObject, Vector3.one * 1.2f, 0.1f).setEasePunch();

    }

    private void UpdateIcon(StatusEffectType type)
    {
        foreach (var entry in statusEffectSprites)
        {
            if (entry.effectType == type && entry.icon != null)
            {
                iconRenderer.sprite = entry.icon;
                return;
            }
        }

        Debug.LogWarning($"No icon found for StatusEffectType {type} on {gameObject.name}");
    }

    public void SetupTemporary(StatusEffectType type)
    {
        stackText.gameObject.SetActive(false);
        if (durationBarTransform != null)
            durationBarTransform.gameObject.SetActive(false);

        UpdateIcon(type);

        LeanTween.moveY(gameObject, transform.position.y + 1.2f, 1f).setEaseOutQuad();
        LeanTween.alpha(gameObject, 0f, 1f).setOnComplete(() => Destroy(gameObject));
        LeanTween.scale(gameObject, Vector3.one * 1.3f, 0.2f).setEaseOutBack();
    }
}

[System.Serializable]
public struct StatusEffectSprite
{
    public StatusEffectType effectType;
    public Sprite icon;
}