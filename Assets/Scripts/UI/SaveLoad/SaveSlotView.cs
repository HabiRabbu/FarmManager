using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class SaveSlotView : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TMP_Text txtTitle;
    [SerializeField] private TMP_Text txtTimestamp;
    [SerializeField] private Image selectionImage;

    public string SlotName { get; private set; }
    public bool IsNewSlot { get; private set; }

    public System.Action<SaveSlotView> OnClick;

    public void InitNewSlot()
    {
        IsNewSlot = true;
        SlotName = null;
        txtTitle.text = "< New Slot >";
        txtTimestamp.text = "";
        SetSelected(false);
        GetComponent<Button>().onClick.AddListener(() => OnClick?.Invoke(this));
    }

    public void InitExisting(string slotName, System.DateTime modified)
    {
        SlotName = slotName;
        IsNewSlot = false;
        txtTitle.text = slotName;
        txtTimestamp.text = modified.ToString("yyyy-MM-dd  HH:mm");
        SetSelected(false);
        GetComponent<Button>().onClick.AddListener(() => OnClick?.Invoke(this));
    }

    public void SetSelected(bool sel)
    {
        var c = sel ? Color.black : Color.black * 0.7f;
        txtTitle.color = c;
        txtTimestamp.color = c;

        selectionImage.enabled = sel;
    }
}
