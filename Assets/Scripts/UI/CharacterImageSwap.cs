using UnityEngine;
using UnityEngine.UI;
using SouthsideGames.SaveManager;

public class CharacterImageSwap : MonoBehaviour
{
    [SerializeField] private Image targetImage;

    private const string LAST_SELECTED_CHARACTER_KEY = "last_selected_character";

    private void Start()
    {
        LoadAndApplyCharacterImage();
    }

    private void LoadAndApplyCharacterImage()
    {
        if (SaveManager.GameData.TryGetValue(LAST_SELECTED_CHARACTER_KEY, out var _, out var characterID))
        {
            CharacterDataSO selectedCharacter = CharacterDatabase.Instance.GetCharacterByID(characterID);
            if (selectedCharacter != null && targetImage != null)
            {
                targetImage.sprite = selectedCharacter.Icon;
            }
        }
    }
}