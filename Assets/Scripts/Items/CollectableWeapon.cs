using UnityEngine;

/// <summary>
/// Handles collectible weapons that can either be added to an empty weapon position
/// or fused with an existing weapon if they share the same ID and level.
/// If both weapons are at max level, the collectible is destroyed.
/// </summary>
public class CollectableWeapon : Item
{
    [Header("SETTING:")]
    [SerializeField] private SpriteRenderer outline;
    [SerializeField] private SpriteRenderer iconRenderer;
    private WeaponDataSO[] weaponDataArray;
    private WeaponDataSO selectedWeapon;
    private int weaponLevel = 1;

    private void Start()
    {
        weaponDataArray = Resources.LoadAll<WeaponDataSO>("Data/Weapons");
        SelectRandomWeapon();

    }

    private void SelectRandomWeapon()
    {
        if (weaponDataArray.Length == 0) return;

        selectedWeapon = weaponDataArray[Random.Range(0, weaponDataArray.Length)];

        if (iconRenderer != null)
        {
            iconRenderer.sprite = selectedWeapon.Icon;
        }

        Color imageColor = ColorHolder.GetColor(weaponLevel);
        outline.color = imageColor;
    }

    protected override void Collected()
    {
        CharacterWeapon characterWeapon = FindObjectOfType<CharacterWeapon>();

        if (characterWeapon == null)
        {
            Debug.LogError("CharacterWeapon component not found!");
            return;
        }

        Weapon[] playerWeapons = characterWeapon.GetWeapons();
        foreach (Weapon playerWeapon in playerWeapons)
        {
            if (playerWeapon == null) continue;

            if (playerWeapon.WeaponData.ID == selectedWeapon.ID)
            {
                if (WeaponFuserManager.Instance.CanFuse(playerWeapon))
                {
                    WeaponFuserManager.Instance.Fuse();
                    Destroy(gameObject);
                    return;
                }
            }
        }

        bool weaponAdded = characterWeapon.AddWeapon(selectedWeapon, weaponLevel);
        if (weaponAdded)
        {
            Destroy(gameObject);
        }
        else
        {
            Debug.Log("No empty weapon positions available!");
        }
    }
}
