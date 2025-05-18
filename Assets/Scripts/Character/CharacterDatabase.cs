using System.Collections.Generic;
using UnityEngine;

public class CharacterDatabase : MonoBehaviour
{

    [SerializeField] private List<CharacterDataSO> allCharacters;

    public CharacterDataSO GetCharacterByID(string id)
    {
        return allCharacters.Find(c => c.ID == id);
    }
}
