using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuShopUpdateUI : MonoBehaviour
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
    private bool isCardAlertActive;

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
       
        cashContainer.SetActive(false);
        gemsContainer.SetActive(false);
        cardsContainer.SetActive(false);

        targetContainer.SetActive(true);
    }

    private void OpenCardSection()
    {
        SwitchShopSection(cardsContainer);

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
            cardAlert.SetActive(true); 
        }
    }

    private void DeactivateCardAlert()
    {
        isCardAlertActive = false;
        
        if (cardAlert != null)
        {
            cardAlert.SetActive(false);
        }
    } 

   
}
