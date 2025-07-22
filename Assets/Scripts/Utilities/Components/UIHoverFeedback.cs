using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

[RequireComponent(typeof(RectTransform))]
public class UIHoverFeedback : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Scale")]
    [SerializeField] float overScale = 1.1f;
    [SerializeField] float scaleDuration = 0.15f;

    [Header("Optional shake")]
    [SerializeField] bool shakeOnEnter = false;
    [SerializeField] float shakeDuration = 0.25f;
    [SerializeField] float shakeStrength = 10f;
    [SerializeField] int vibrato = 10;

    Vector3 startScale;
    RectTransform rect;
    Tween scaleTween;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        startScale = rect.localScale;
    }

    public void OnPointerEnter(PointerEventData _)
    {
        scaleTween?.Kill();
        scaleTween = rect.DOScale(startScale * overScale, scaleDuration)
                         .SetEase(Ease.OutBack)
                         .SetUpdate(true);

        if (shakeOnEnter)
        {
            rect.DOShakeAnchorPos(shakeDuration, shakeStrength, vibrato, 90, false, true).SetUpdate(true);
        }
    }

    public void OnPointerExit(PointerEventData _)
    {
        scaleTween?.Kill();
        scaleTween = rect.DOScale(startScale, scaleDuration)
                         .SetEase(Ease.OutBack)
                         .SetUpdate(true);
    }

    void OnDisable()
    {
        scaleTween?.Kill();
        rect.localScale = startScale;
    }
}
