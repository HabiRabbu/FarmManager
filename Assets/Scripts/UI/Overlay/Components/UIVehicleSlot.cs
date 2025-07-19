using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Harvey.Farm.VehicleScripts;

public class UIVehicleSlot : MonoBehaviour
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

    public void Show(Vehicle vehicle)
    {
        if (vehicle == null)
        {
            ShowEmpty();
            return;
        }
        iconImage.sprite = vehicle._stats.Model.Icon;
        nameText.text = vehicle._stats.Model.DisplayName;
        durabilityBar.fillAmount = vehicle._stats.Model.Durability / 100f;
    }
}
