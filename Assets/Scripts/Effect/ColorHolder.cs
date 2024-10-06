using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorHolder : MonoBehaviour
{
    public static ColorHolder Instance;

    [SerializeField] private PaletteSO palette;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public static Color GetColor(int _level)
    {
        _level = Mathf.Clamp(_level, 0, Instance.palette.LevelColors.Length);

        return Instance.palette.LevelColors[_level];
    }

    public static Color GetOutlineColor(int _level)
    {
        _level = Mathf.Clamp(_level, 0, Instance.palette.LevelOutlineColors.Length);

        return Instance.palette.LevelOutlineColors[_level];
    }
}
