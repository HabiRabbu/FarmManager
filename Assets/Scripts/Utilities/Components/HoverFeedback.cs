using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

public class HoverFeedback : MonoBehaviour
{
    [Header("Scale")]
    public float scaleUpFactor = 1.3f;
    public float scaleDuration = 0.15f;

    [Header("Glow")]
    [ColorUsage(true, true)]
    public Color hoverEmission = Color.white;
    public float emissionBoost = 0.3f;

    readonly int Emission = Shader.PropertyToID("_EmissionColor");

    Vector3 originalScale;
    Tween   scaleTween;

    readonly List<Material> mats  = new();
    readonly List<Color>    baseEmission = new();

    void Start()
    {
        originalScale = transform.localScale;

        foreach (var r in transform.Find("Meshes")
                                   .GetComponentsInChildren<Renderer>())
        {
            var m = r.material;
            m.EnableKeyword("_EMISSION");
            baseEmission.Add(m.GetColor(Emission));
            mats.Add(m);
        }
    }

    public void OnHoverEnter()
    {
        scaleTween?.Kill();
        transform.DOScale(originalScale * scaleUpFactor, scaleDuration);

        Color target = hoverEmission * emissionBoost;
        foreach (var m in mats)
            m.DOColor(target, Emission, scaleDuration);
    }

    public void OnHoverExit()
    {
        scaleTween?.Kill();
        transform.DOScale(originalScale, scaleDuration);

        for (int i = 0; i < mats.Count; i++)
            mats[i].DOColor(baseEmission[i], Emission, scaleDuration);
    }
}
