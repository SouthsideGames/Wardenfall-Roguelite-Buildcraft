using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InGameHUDController : MonoBehaviour
{
    [Header("Player HUD Elements")]
    [SerializeField] private TextMeshProUGUI liveLabel;
    [SerializeField] private Image liveDot;
    [SerializeField] private TextMeshProUGUI viewerCommentText;


    [Header("Sponsor Banner")]
    [SerializeField] private RectTransform sponsorBanner;
    [SerializeField] private CanvasGroup sponsorCanvasGroup;
    [SerializeField] private TextMeshProUGUI sponsorText;
    [SerializeField] private float scrollSpeed = 100f;
    [SerializeField] private float sponsorDelayDuration = 2.5f; // Adjust for "commercial break" feel
    private float sponsorDelayTimer = 0f;
    private bool isWaitingForSponsor = false;
    private bool blinkState = true;

    private readonly string[] sponsors = new string[]
    {
        "🥩 MeatCorp™ - Fueling the Carnage!",
        "💉 RedNeedle™ - Power Through Pain.",
        "🧠 Synapse+ - Think Fast. Die Last.",
        "📦 LootR™ - You Open It, You Own It.",
        "🔥 ScorchSpice™ - Taste Victory or Burn!",
        "⚡ ShockDrop™ - Delivering Death Daily.",
        "🔒 LockBox™ - Secrets Worth Bleeding For.",
        "💰 BloodBank™ - Earn. Bleed. Repeat.",
        "🎥 KillCam+ – We're Always Watching.",
        "🪖 WardenTech™ - Gear for Gladiators.",
        "🎯 CritCola™ - Guaranteed to Hit the Spot.",
        "🦴 BoneZone™ - Break. Bash. Belong.",
        "📡 DeathCast™ - Broadcasting Your Final Moments.",
        "🍖 Prime Cuts™ - Only the Freshest Meat Survives.",
        "⚙️ GrindWorks™ - Where Pain is Progress.",
        "🍬 GritSnax™ - Bite Down or Bow Out.",
        "🧼 BloodSudz™ - Wash Away the Weak.",
        "🎮 GlitchPlay™ - Fun. Fatal. Fast.",
        "🧪 XPerience Labs™ - Trial by Fire.",
        "⛓️ ChainGain™ - Bound for Glory.",
        "🧊 ChillPoint™ - Freeze First, Think Later.",
        "🔋 VoltVault™ - Shockingly Reliable.",
        "🏹 Piercify™ - Every Shot, A Story.",
        "🪓 HatchetWorks™ - Cut Costs. Literally.",
        "🧲 PullZone™ - Get Drawn Into Mayhem.",
        "🔮 HexNet™ - Predictably Dangerous.",
        "📔 LoreVerse™ - Know Before You Go.",
        "💀 SkullRate™ - Kill Faster, Score Higher.",
        "🍿 FleshPop™ - Crunchy. Bloody. Delicious.",
        "🛰️ WardenSat™ - Always Watching. Always Judging."
    };

    private readonly string[] viewerComments = new string[]
    {
        "That crit was insane!",
        "SEND IN THE BOSS!",
        "They're not gonna make it...",
        "MeatCorp is proud!",
        "CritCola moment!",
        "💀 Brutal takedown!",
        "The crowd wants BLOOD!",
        "Who gave them that card?!",
        "WardenTech approved.",
        "You call that dodging?",
        "Peak gaming right here.",
        "Needs more fire.",
        "First time player?",
        "They're actually cracked.",
        "Rest in pieces.",
        "👁 We're watching.",
        "DPS check passed.",
        "More violence!",
        "This is better than cable.",
        "Glory or guts?"
    };


    private void Start()
    {
        InvokeRepeating(nameof(BlinkLiveDot), 0f, 0.5f);
        SetSponsorOpacity(0.35f);

        if (sponsorText != null)
            sponsorText.text = $"Sponsored by: {GetRandomSponsor()}";
    }

    private void OnEnable()
    {
        Enemy.OnEnemyKilled += TryShowComment;
        Weapon.OnCriticalHit += TryShowComment;
        CardEffectManager.OnCardActivated += TryShowComment;
       
    }

    private void OnDisable()
    {
        Enemy.OnEnemyKilled -= TryShowComment;
        Weapon.OnCriticalHit -= TryShowComment;
        CardEffectManager.OnCardActivated -= TryShowComment;
    
    }

    private void Update() => ScrollSponsor();

    private void BlinkLiveDot()
    {
        blinkState = !blinkState;
        liveDot.enabled = blinkState;
    }

    private void ScrollSponsor()
    {
        if (isWaitingForSponsor)
        {
            sponsorDelayTimer -= Time.deltaTime;
            if (sponsorDelayTimer <= 0f)
            {
                sponsorText.text = $"Sponsored by: {GetRandomSponsor()}";
                float resetX = Screen.width + sponsorBanner.rect.width;
                sponsorBanner.anchoredPosition = new Vector2(resetX, sponsorBanner.anchoredPosition.y);
                isWaitingForSponsor = false;
            }
            return;
        }

        sponsorBanner.anchoredPosition += Vector2.left * scrollSpeed * Time.deltaTime;

        if (sponsorBanner.anchoredPosition.x < -sponsorBanner.rect.width)
        {
            isWaitingForSponsor = true;
            sponsorDelayTimer = sponsorDelayDuration;
        }
    }

    private void SetSponsorOpacity(float alpha) => sponsorCanvasGroup.alpha = Mathf.Clamp01(alpha);

    private string GetRandomSponsor()
    {
        int index = UnityEngine.Random.Range(0, sponsors.Length);
        return sponsors[index];
    }

    private void ShowRandomViewerComment()
    {
        int index = UnityEngine.Random.Range(0, viewerComments.Length);
        viewerCommentText.text = $"💬 {viewerComments[index]}";

        viewerCommentText.gameObject.SetActive(true);
        LeanTween.alphaCanvas(viewerCommentText.GetComponent<CanvasGroup>(), 1f, 0.2f)
                .setOnComplete(() =>
                    LeanTween.alphaCanvas(viewerCommentText.GetComponent<CanvasGroup>(), 0f, 1.5f)
                            .setDelay(1.5f)
                );
    }
    
    private void TryShowComment()
    {
        float chance = UnityEngine.Random.Range(0f, 1f);
        if (chance <= 0.5f) // 50% chance
        {
            ShowRandomViewerComment();
        }
    }
}
