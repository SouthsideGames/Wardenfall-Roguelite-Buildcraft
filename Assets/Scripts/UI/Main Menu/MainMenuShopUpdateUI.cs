using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuShopManager : MonoBehaviour
{
    [Header("Shop Sections")]
    [SerializeField] private GameObject cashContainer;
    [SerializeField] private GameObject gemsContainer;
    [SerializeField] private GameObject cardsContainer;

    [Header("Shop Buttons")]
    [SerializeField] private Button cashButton;
    [SerializeField] private Button gemsButton;
    [SerializeField] private Button cardsButton;

    [Header("Alerts")]
    [SerializeField] private GameObject cardAlert; 
    private bool isCardAlertActive = false;

    private void Awake() 
    {
        CardShopManager.OnWeeklyRefresh += ActivateCardAlert;
    }

    private void Start()
    {
    
        cashButton.onClick.AddListener(() => SwitchShopSection(cashContainer));
        gemsButton.onClick.AddListener(() => SwitchShopSection(gemsContainer));
        cardsButton.onClick.AddListener(() => OpenCardSection());

        SwitchShopSection(cashContainer);
    }

    private void OnDestroy()
    {
       
        CardShopManager.OnWeeklyRefresh -= ActivateCardAlert;
    }

    private void SwitchShopSection(GameObject targetContainer)
    {
        // Close all containers
        cashContainer.SetActive(false);
        gemsContainer.SetActive(false);
        cardsContainer.SetActive(false);

        // Open the target container
        targetContainer.SetActive(true);
    }

    private void OpenCardSection()
    {
        // Open the Cards section
        SwitchShopSection(cardsContainer);

        // Dismiss the card alert when entering the Cards section
        if (isCardAlertActive)
        {
            DeactivateCardAlert();
        }
    }

    private void ActivateCardAlert()
    {
        isCardAlertActive = true;
        if (cardAlert != null)
        {
            cardAlert.SetActive(true); // Show the alert
        }
    }

    private void DeactivateCardAlert()
    {
        isCardAlertActive = false;
        if (cardAlert != null)
        {
            cardAlert.SetActive(false); // Hide the alert
        }
    } 

   
}
