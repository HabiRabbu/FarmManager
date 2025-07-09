using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Harvey.Farm.Workers;

public class UIWorkerSlot : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private Sprite emptySlotSprite;

    public void ShowEmpty()
    {
        iconImage.sprite = emptySlotSprite;
        nameText.text = "";
    }

    public void Show(Worker worker)
    {
        if (worker == null)
        {
            ShowEmpty();
            return;
        }
        iconImage.sprite = worker.Portrait;
        nameText.text = worker.DisplayName;
    }
}
