using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WaveIntroUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private CanvasGroup panelCanvasGroup;
    [SerializeField] private Transform traitIconParent;
    [SerializeField] private GameObject traitIconPrefab;
    [SerializeField] private Button continueButton;

    [SerializeField] private GameObject traitListSection;
    [SerializeField] private TextMeshProUGUI emptyMessageText;

    [Header("LeanTween Settings")]
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private float scaleDuration = 0.5f;

    [Header("Empty Message")]
    [SerializeField] private string noTraitsMessage = "We’ll Be Right Back";

    [Header("Sponsor Messages")]
    [SerializeField]
    private List<string> sponsorMessages = new List<string>()
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


    private void OnEnable() => WaveManager.OnWaveIntroRequested += ShowIntro;

    private void OnDisable()=> WaveManager.OnWaveIntroRequested -= ShowIntro;
    
    public void ShowIntro(List<(TraitDataSO trait, int stack)> activeTraits)
    {
        panelCanvasGroup.alpha = 1f;
        panelCanvasGroup.transform.localScale = Vector3.one;
        panelCanvasGroup.gameObject.SetActive(true);

        bool hasTraits = activeTraits != null && activeTraits.Count > 0;

        if (hasTraits)
        {
            traitListSection.SetActive(true);
            emptyMessageText.gameObject.SetActive(false);

            foreach (Transform child in traitIconParent)
                Destroy(child.gameObject);

            foreach (var (trait, stack) in activeTraits)
            {
                if (trait == null) continue;
                var entry = Instantiate(traitIconPrefab, traitIconParent);
                var entryUI = entry.GetComponent<TraitEntryUI>();
                entryUI.Setup(trait, stack);
            }
        }
        else
        {
            traitListSection.SetActive(false);
            emptyMessageText.gameObject.SetActive(true);

            string sponsor = GetRandomSponsorMessage();
            emptyMessageText.text = sponsor;
        }

        continueButton.onClick.RemoveAllListeners();
        continueButton.onClick.AddListener(OnContinueClicked);
    }

    private void OnContinueClicked()
    {
        LeanTween.alphaCanvas(panelCanvasGroup, 0f, fadeDuration).setEaseInQuad();
        LeanTween.scale(panelCanvasGroup.gameObject, Vector3.one * 0.8f, scaleDuration).setEaseInBack()
            .setOnComplete(() =>
            {
                panelCanvasGroup.gameObject.SetActive(false);
                WaveManager.OnWaveIntroCompleted?.Invoke();
            });
    }
    
    private string GetRandomSponsorMessage()
    {
        if (sponsorMessages == null || sponsorMessages.Count == 0)
            return "We’ll Be Right Back";

        int index = Random.Range(0, sponsorMessages.Count);
        return sponsorMessages[index];
    }
}
