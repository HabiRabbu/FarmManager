using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Harvey.Farm.Buildings;
using System.Linq;

public class UIBuildingInfo : MonoBehaviour
{
    [SerializeField] private TMP_Text txtTitle;
    [SerializeField] private TMP_Text txtBody;
    [SerializeField] private Button btnClose;

    [SerializeField] private Transform slotContainer;
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private int defaultMaxSlots = 6;

    private Building current;

    void Awake()
    {
        btnClose.onClick.AddListener(Close);
        gameObject.SetActive(false);
    }

    void Start()
    {

    }

    public void Show(Building building)
    {
        current = building;

        ClearSlots();
        SetContent(current);

        gameObject.SetActive(true);
    }

    public void Refresh()
    {
        if (current == null || !gameObject.activeSelf) return;
        ClearSlots();
        SetContent(current);
    }

    void SetContent(Building building)
    {
        txtTitle.text = building.DisplayName;

        if (building is ShedBuilding shed)
        {
            SpawnImplementSlots(shed);
            int total = shed.Query(_ => true).Count();
            int reserved = 0;
            txtBody.text = $"Implements: {total}\nReserved: {reserved}";
        }
    }

    void SpawnImplementSlots(ShedBuilding shed)
    {
        int maxSlots = shed.ShedDef.ImplementSlots > 0 ? shed.ShedDef.ImplementSlots : defaultMaxSlots;
        var implements = shed.Query(b => true).ToList();
        int count = Mathf.Min(implements.Count, maxSlots);

        for (int i = 0; i < count; i++)
        {
            var slotGO = Instantiate(slotPrefab, slotContainer);
            var slot = slotGO.GetComponent<UIImplementSlot>();
            if (slot != null)
            {
                slot.Show(implements[i]);
            }
        }

        // Fill remaining slots with empty slots
        for (int i = count; i < maxSlots; i++)
        {
            var slotGO = Instantiate(slotPrefab, slotContainer);
            var slot = slotGO.GetComponent<UIImplementSlot>();
            if (slot != null)
            {
                slot.ShowEmpty();
            }
        }
    }

    void ClearSlots()
    {
        foreach (Transform child in slotContainer)
            Destroy(child.gameObject);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}
