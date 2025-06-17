using UnityEngine;

[System.Serializable]
public readonly struct FadingPopupData
{
    public readonly string Text;
    public readonly Color Color;
    public readonly float FadeDuration;

    public FadingPopupData(
        string text,
        Color? color = null,
        float fadeDuration = 4f)
    {
        Text = text;
        Color = color ?? Color.white;
        FadeDuration = fadeDuration;
    }
}
