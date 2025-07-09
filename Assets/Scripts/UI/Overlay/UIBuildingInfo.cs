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
    [SerializeField] private GameObject ImplementSlotPrefab;
    [SerializeField] private GameObject VehicleSlotPrefab;
    [SerializeField] private GameObject WorkerSlotPrefab;
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

        switch (building)
        {
            /* ───────── Shed ───────── */
            case ShedBuilding shed:
                SpawnImplementSlots(shed);

                int implTotal = shed.Query(_ => true).Count();
                int implReserved = shed.Query(i => shed.IsReserved(i.id)).Count();
                txtBody.text = $"Implements: {implTotal}\nReserved:   {implReserved}";
                break;

            /* ───────── Garage ──────── */
            case GarageBuilding garage:
                SpawnVehicleSlots(garage);

                int vehTotal = garage.Query(_ => true).Count();
                int vehReserved = garage.Query(v => garage.IsReserved(v.Id)).Count();
                txtBody.text = $"Tractors:   {vehTotal}\nReserved:  {vehReserved}";
                break;

            /* ───────── House ───────── */
            case HouseBuilding house:
                SpawnWorkerSlots(house);

                int occ = house.GetIdleWorkers().Count();
                int cap = house.Capacity;
                txtBody.text = $"Idle Workers: {occ}\nCapacity:     {cap}";
                break;

            /* ───────── Default ─────── */
            default:
                txtBody.text = "No detailed info available.";
                break;
        }
    }

    void SpawnImplementSlots(ShedBuilding shed)
    {
        int maxSlots = shed.ShedDef.ImplementSlots > 0 ? shed.ShedDef.ImplementSlots : defaultMaxSlots;
        var implements = shed.Query(b => true).ToList();
        int count = Mathf.Min(implements.Count, maxSlots);

        for (int i = 0; i < count; i++)
        {
            var slotGO = Instantiate(ImplementSlotPrefab, slotContainer);
            var slot = slotGO.GetComponent<UIImplementSlot>();
            if (slot != null)
            {
                slot.Show(implements[i]);
            }
        }

        // Fill remaining slots with empty slots
        for (int i = count; i < maxSlots; i++)
        {
            var slotGO = Instantiate(ImplementSlotPrefab, slotContainer);
            var slot = slotGO.GetComponent<UIImplementSlot>();
            if (slot != null)
            {
                slot.ShowEmpty();
            }
        }
    }

    void SpawnVehicleSlots(GarageBuilding garage)
    {
        int maxSlots = garage.Capacity > 0
                     ? garage.Capacity
                     : defaultMaxSlots;

        var tractors = garage.Query(_ => true).ToList();
        int count = Mathf.Min(tractors.Count, maxSlots);

        for (int i = 0; i < count; i++)
        {
            var go = Instantiate(VehicleSlotPrefab, slotContainer);
            var slot = go.GetComponent<UIVehicleSlot>();
            if (slot) slot.Show(tractors[i]);
        }
        for (int i = count; i < maxSlots; i++)
        {
            var go = Instantiate(VehicleSlotPrefab, slotContainer);
            var slot = go.GetComponent<UIVehicleSlot>();
            if (slot) slot.ShowEmpty();
        }
    }

    void SpawnWorkerSlots(HouseBuilding house)
    {
        int maxSlots = house.Capacity > 0
                     ? house.Capacity
                     : defaultMaxSlots;

        var workers = house.GetIdleWorkers().ToList();
        int count = Mathf.Min(workers.Count, maxSlots);

        for (int i = 0; i < count; i++)
        {
            var go = Instantiate(WorkerSlotPrefab, slotContainer);
            var slot = go.GetComponent<UIWorkerSlot>();
            if (slot) slot.Show(workers[i]);
        }
        for (int i = count; i < maxSlots; i++)
        {
            var go = Instantiate(WorkerSlotPrefab, slotContainer);
            var slot = go.GetComponent<UIWorkerSlot>();
            if (slot) slot.ShowEmpty();
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
