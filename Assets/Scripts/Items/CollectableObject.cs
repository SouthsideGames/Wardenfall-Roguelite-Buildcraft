using UnityEngine;

/// <summary>
/// Handles collectible objects that can be added to the character's object list when collected.
/// Objects do not fuse and are randomly selected from an array of object data.
/// </summary
public class CollectableObject : Item
{
    [Header("SETTING:")]
    [SerializeField] private SpriteRenderer outline;
    [SerializeField] private SpriteRenderer iconRenderer;
    private ObjectDataSO[] objectDataArray;
    private ObjectDataSO selectedObject;

    private void Start()
    {
        objectDataArray = Resources.LoadAll<ObjectDataSO>("Data/Objects");
        SelectRandomObject();
    }

    private void SelectRandomObject()
    {
        if (objectDataArray.Length == 0) return;

        selectedObject = objectDataArray[Random.Range(0, objectDataArray.Length)];

        if (iconRenderer != null)
        {
            iconRenderer.sprite = selectedObject.Icon;
        }

        
        Color imageColor = ColorHolder.GetColor(selectedObject.Rarity);
        outline.color = imageColor;
    }

    protected override void Collected()
    {
        CharacterObjects characterObjects = FindFirstObjectByType<CharacterObjects>();

        if (characterObjects == null)
        {
            Debug.LogError("CharacterObjects component not found!");
            return;
        }

        characterObjects.AddObject(selectedObject);
        Destroy(gameObject);
    }
}
