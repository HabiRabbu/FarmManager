using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Harvey.Farm.UI.Radial
{
    [RequireComponent(typeof(CanvasGroup))]
    public class RadialMenuController : MonoBehaviour
    {
        [Serializable]
        struct SliceRefs
        {
            public string name;
            public RectTransform pivot;
            public Button button;
            public Image icon;
        }

        [Header("Wedge references (max 6)")]
        [SerializeField] SliceRefs[] slices;

        // ---------------------------------------------------------------------

        public void Show(Vector2 screenPos,
                         IReadOnlyList<RadialMenuItem> items,
                         Action onClosed = null)
        {
            int count = Mathf.Min(items.Count, slices.Length);

            transform.position = screenPos;
            gameObject.SetActive(true);

            float step = 360f / count;
            float startAngle = 0f;

            for (int i = 0; i < slices.Length; i++)
            {
                bool active = i < count;
                slices[i].pivot.gameObject.SetActive(active);
                if (!active) continue;

                var item = items[i];
                var slice = slices[i];

                slice.pivot.localRotation = Quaternion.Euler(0, 0, startAngle + i * step);

                slice.icon.rectTransform.rotation = Quaternion.identity;

                // icon / label
                slice.icon.sprite = item.icon;

                slice.button.onClick.RemoveAllListeners();
                slice.button.onClick.AddListener(() =>
                {
                    Close(onClosed);
                    item.onClick?.Invoke();
                });
            }
        }

        public void Close(Action onClosed = null)
        {
            gameObject.SetActive(false);
            onClosed?.Invoke();
        }
    }
}
