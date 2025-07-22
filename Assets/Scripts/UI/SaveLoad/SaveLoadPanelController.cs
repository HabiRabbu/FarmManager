using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Harvey.Farm.UI;
using Harvey.Farm.Utilities;
using Harvey.SaveSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveLoadPanelController : MonoBehaviour
{
    [Header("Hierarchy refs")]
    [SerializeField] private Transform slotContainer;
    [SerializeField] private SaveSlotView slotPrefab;
    [SerializeField] private TMP_InputField txtNameField;

    [SerializeField] private Button btnSave;
    [SerializeField] private Button btnLoad;
    [SerializeField] private Button btnRename;
    [SerializeField] private Button btnDelete;
    [SerializeField] private Button btnBack;

    [SerializeField] private GameObject optionsSelectionGO;

    /* -------------------------------------------------- */

    private readonly List<SaveSlotView> slots = new();
    private SaveSlotView selected;

    private void Awake()
    {
        btnSave.onClick.AddListener(OnSaveClicked);
        btnLoad.onClick.AddListener(() => _ = OnLoadClicked());
        btnRename.onClick.AddListener(OnRenameClicked);
        btnDelete.onClick.AddListener(OnDeleteClicked);
        btnBack.onClick.AddListener(Close);

        txtNameField.onValueChanged.AddListener(OnNameFieldChanged);

        RefreshList();
        UpdateButtons();
    }

    /* ---------- public API ---------- */

    public void Open()
    {
        gameObject.SetActive(true);
        RefreshList();
        SelectFirstSlot();
    }

    public void Close()
    {
        gameObject.SetActive(false);
        optionsSelectionGO.SetActive(true);
    }

    /* ---------- UI ---------- */

    private void RefreshList()
    {
        foreach (var s in slots) Destroy(s.gameObject);
        slots.Clear();
        selected = null;

        AddNewSlotRow();

        var saves = SaveService.Instance.GetAllSaves();
        foreach (var (name, timestamp) in saves)
        {
            var view = Instantiate(slotPrefab, slotContainer);
            view.InitExisting(name, timestamp);
            view.OnClick += OnSlotClicked;
            slots.Add(view);
        }
    }

    private void AddNewSlotRow()
    {
        var view = Instantiate(slotPrefab, slotContainer);
        view.InitNewSlot();
        view.OnClick += OnSlotClicked;
        slots.Add(view);
    }

    private void OnSlotClicked(SaveSlotView view)
    {
        selected?.SetSelected(false);
        selected = view;
        selected.SetSelected(true);

        txtNameField.text = view.IsNewSlot ? "" : view.SlotName;
        UpdateButtons();
    }

    private void UpdateButtons()
    {
        bool haveSel = selected && !selected.IsNewSlot;
        bool haveNameText = !string.IsNullOrWhiteSpace(txtNameField.text);

        btnSave.interactable = selected && haveNameText;
        btnLoad.interactable = haveSel;
        btnRename.interactable = haveSel && haveNameText;
        btnDelete.interactable = haveSel && !selected.IsNewSlot;
    }

    private void OnSaveClicked()
    {
        var name = txtNameField.text.Trim();
        if (string.IsNullOrEmpty(name)) return;

        SaveService.Instance.SaveGame(name);
        RefreshList();

        selected = slots.FirstOrDefault(s => s.SlotName == name);
        selected?.SetSelected(true);
        UpdateButtons();

        var n = new NotificationData
            (
                $"Game saved to slot: {name}",
                textColor: Color.white,
                backgroundColor: Colors.COLOR_GREY,
                fadeDuration: 6f
            );

        UIManager.Notify(n);
    }

    private async Task OnLoadClicked()
    {
        if (selected == null || selected.IsNewSlot) return;
        await SaveService.Instance.LoadGame(selected.SlotName);
        Close();

        var n = new NotificationData
            (
                $"Game loaded from slot: {selected.SlotName}",
                textColor: Color.white,
                backgroundColor: Colors.COLOR_GREY,
                fadeDuration: 6f
            );

        UIManager.Notify(n);
    }

    private void OnRenameClicked()
    {
        if (selected == null || selected.IsNewSlot) return;
        var newName = txtNameField.text.Trim();
        if (string.IsNullOrEmpty(newName) || newName == selected.SlotName) return;

        if (SaveService.Instance.RenameSave(selected.SlotName, newName))
        {
            RefreshList();
            // Auto-reselect renamed slot
            selected = slots.FirstOrDefault(s => s.SlotName == newName);
            selected?.SetSelected(true);
            UpdateButtons();
        }
    }

    private void OnDeleteClicked()
    {
        if (selected == null || selected.IsNewSlot) return;

        if (SaveService.Instance.DeleteSave(selected.SlotName))
        {
            RefreshList();
            SelectFirstSlot();
        }
    }

    /* ---------- helpers ---------- */

    private void SelectFirstSlot()
    {
        if (slots.Count > 0) OnSlotClicked(slots[0]);
    }

    /* ---------- event hooks ---------- */

    public void OnNameFieldChanged(string _)
    {
        UpdateButtons();
    }
}
