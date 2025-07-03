using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Harvey.Farm.Implements;

public class UIImplementSlot : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private Image durabilityBar;
    [SerializeField] private Sprite emptySlotSprite;

    public void ShowEmpty()
    {
        iconImage.sprite = emptySlotSprite;
        nameText.text = "";
        durabilityBar.fillAmount = 0;
    }

    public void Show(ImplementBehaviour implement)
    {
        if (implement == null)
        {
            ShowEmpty();
            return;
        }
        iconImage.sprite = implement.Icon;
        nameText.text = implement.Def.DisplayName;
        durabilityBar.fillAmount = implement.Durability;
    }
}
