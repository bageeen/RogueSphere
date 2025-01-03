using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RarityColors
{
    Common,
    Uncommon,
    Rare,
    Mythic,
    Legendary
}

public static class RarityColorsExtensions
{
    public static Color GetColor(this RarityColors rarityColors)
    {
        switch (rarityColors)
        {
            case RarityColors.Common:
                return new Color(0.75f, 0.75f, 0.75f); // Light Gray: (192, 192, 192)
            case RarityColors.Uncommon:
                return new Color(0.6f, 0.8f, 0.6f); // Light Green: (153, 204, 153)
            case RarityColors.Rare:
                return new Color(0.6f, 0.6f, 1.0f); // Light Blue: (153, 153, 255)
            case RarityColors.Mythic:
                return new Color(0.75f, 0.5f, 0.75f); // Light Purple: (192, 128, 192)
            case RarityColors.Legendary:
                return new Color(1.0f, 0.6f, 0.6f); // Pastel Red: (255, 153, 153)
            default:
                return Color.white; // Default to white if the color is not found
        }
    }
}
