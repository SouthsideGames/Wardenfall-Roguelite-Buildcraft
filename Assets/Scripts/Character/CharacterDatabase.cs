using System.Collections.Generic;
using UnityEngine;

public class CharacterDatabase : MonoBehaviour
{
    public static CharacterDatabase Instance;

    [SerializeField] private List<CharacterDataSO> allCharacters;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public CharacterDataSO GetCharacterByID(string id)
    {
        return allCharacters.Find(c => c.ID == id);
    }
}
