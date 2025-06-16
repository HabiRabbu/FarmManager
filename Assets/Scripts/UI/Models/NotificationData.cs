using UnityEngine;

[System.Serializable]
public readonly struct NotificationData
{
    public readonly string Text;
    public readonly Color TextColor;
    public readonly Color BackgroundColor;
    public readonly float FadeDuration;

    public NotificationData(
        string text,
        Color? textColor = null,
        Color? backgroundColor = null,
        float fadeDuration = 4f)
    {
        Text = text;
        TextColor = textColor ?? Color.white;
        BackgroundColor = backgroundColor ?? Color.green;
        FadeDuration = fadeDuration;
    }
}
