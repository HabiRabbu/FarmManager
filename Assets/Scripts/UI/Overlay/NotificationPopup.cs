using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using System;
using System.Collections;

public class NotificationPopup : MonoBehaviour
{
    private Coroutine fadeRoutine;
    [SerializeField] private AnimationCurve fadeCurve = AnimationCurve.Linear(0, 1, 1, 0);

    [Header("UI Refs")]
    [SerializeField] TMP_Text notificationTxt;
    [SerializeField] Image panelImage;

    public void Setup(in NotificationData data, Action onComplete)
    {
        notificationTxt.text = data.Text;
        notificationTxt.color = data.TextColor;

        if (panelImage) panelImage.color = data.BackgroundColor;

        StartCoroutine(RemoveAfter(data.FadeDuration, onComplete));
    }

    private IEnumerator RemoveAfter(float duration, Action onComplete)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        onComplete?.Invoke();
    }

    void OnDestroy()
    {
        StopAllCoroutines();
    }
}
