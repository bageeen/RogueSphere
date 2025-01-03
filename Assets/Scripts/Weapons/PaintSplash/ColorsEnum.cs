using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ColorEnum
{
    Red,
    Green,
    Blue,
    Yellow,
    Orange,
    Purple,
    Pink,
    Gray,
    Brown,
    Cyan,
    Magenta,
    Lavender,
    Mint,
    Peach,
    Teal
}

public static class ColorEnumExtensions
{
    public static Color GetColor(this ColorEnum colorEnum)
    {
        switch (colorEnum)
        {
            case ColorEnum.Red:
                return new Color(1.0f, 0.6f, 0.6f); // Pastel Red
            case ColorEnum.Green:
                return new Color(0.6f, 1.0f, 0.6f); // Pastel Green
            case ColorEnum.Blue:
                return new Color(0.6f, 0.6f, 1.0f); // Pastel Blue
            case ColorEnum.Yellow:
                return new Color(1.0f, 1.0f, 0.6f); // Pastel Yellow
            case ColorEnum.Orange:
                return new Color(1.0f, 0.8f, 0.6f); // Pastel Orange
            case ColorEnum.Purple:
                return new Color(0.8f, 0.6f, 1.0f); // Pastel Purple
            case ColorEnum.Pink:
                return new Color(1.0f, 0.8f, 0.86f); // Pastel Pink
            case ColorEnum.Gray:
                return new Color(0.8f, 0.8f, 0.8f); // Pastel Gray
            case ColorEnum.Brown:
                return new Color(0.8f, 0.6f, 0.4f); // Pastel Brown
            case ColorEnum.Cyan:
                return new Color(0.6f, 1.0f, 1.0f); // Pastel Cyan
            case ColorEnum.Magenta:
                return new Color(1.0f, 0.6f, 1.0f); // Pastel Magenta
            case ColorEnum.Lavender:
                return new Color(0.9f, 0.8f, 1.0f); // Pastel Lavender
            case ColorEnum.Mint:
                return new Color(0.74f, 0.98f, 0.74f); // Pastel Mint
            case ColorEnum.Peach:
                return new Color(1.0f, 0.85f, 0.7f); // Pastel Peach
            case ColorEnum.Teal:
                return new Color(0.6f, 0.8f, 0.8f); // Pastel Teal
            default:
                return Color.white; // Default to white if the color is not found
        }
    }
}