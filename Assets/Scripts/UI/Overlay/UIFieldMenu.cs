using Harvey.Farm.Events;
using Harvey.Farm.FieldScripts;
using Harvey.Farm.JobScripts;
using Harvey.Farm.Crops;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Harvey.Farm.VehicleScripts;
using System.Collections.Generic;
using System.Linq;
using Harvey.Farm.Implements;
using Harvey.Farm.Buildings;

public class UIFieldMenu : MonoBehaviour
{
    [Header("UI refs")]
    [SerializeField] private Button btnPlow;
    [SerializeField] private Button btnSeed;
    [SerializeField] private Button btnHarvest;
    [SerializeField] private Button btnClose;
    [SerializeField] private Button btnGo;
    [SerializeField] private GameObject plowMenu, seedMenu, harvestMenu;

    [Header("Plow Menu Refs")]
    [SerializeField] private TMP_Dropdown dpnTractorSelectPlow;
    [SerializeField] private TMP_Dropdown dpnImplementSelectPlow;
    [SerializeField] private TMP_Text textTractorSelectPlow;
    [SerializeField] private TMP_Text txtImplementSelectPlow;

    [Header("Seed Menu Refs")]
    [SerializeField] private TMP_Dropdown dpnTractorSelectSeed;
    [SerializeField] private TMP_Dropdown dpnImplementSelectSeed;
    [SerializeField] private TMP_Dropdown dpnCropSelectSeed;
    [SerializeField] private TMP_Text textTractorSelectSeed;
    [SerializeField] private TMP_Text txtImplementSelectSeed;
    [SerializeField] private TMP_Text txtCropSelectSeed;

    [Header("Harvest Menu Refs")]
    [SerializeField] private TMP_Dropdown dpnTractorSelectHarvest;
    [SerializeField] private TMP_Text textTractorSelectHarvest;


    [SerializeField] private CropRegistry cropRegistry;
    Field field;
    JobType selectedTask;
    List<Vehicle> idleTractors = new();
    List<ImplementBehaviour> implementChoices = new();

    void Start()
    {
        gameObject.SetActive(false);
        btnPlow.onClick.AddListener(() => SetTask(JobType.Plow, plowMenu));
        btnSeed.onClick.AddListener(() => SetTask(JobType.Seed, seedMenu));
        btnHarvest.onClick.AddListener(() => SetTask(JobType.Harvest, harvestMenu));
        btnGo.onClick.AddListener(OnStartClicked);
        btnClose.onClick.AddListener(Hide);
    }

    public void Show(Field f)
    {
        field = f;
        gameObject.SetActive(true);

        if (f.Needs(JobType.Plow)) SetTask(JobType.Plow, plowMenu);
        else if (f.Needs(JobType.Seed)) SetTask(JobType.Seed, seedMenu);
        else SetTask(JobType.Harvest, harvestMenu);
    }
    public void Hide() => gameObject.SetActive(false);

    public void Refresh()
    {
        if (!field) return;

        switch (selectedTask)
        {
            case JobType.Plow:
                PopulateTractors();
                PopulateImplements();
                break;
            case JobType.Seed:
                PopulateTractors();
                PopulateImplements();
                PopulateCrops();
                break;
            case JobType.Harvest:
                PopulateTractors();
                break;
        }

        ValidateReady();
    }

    /* ---------- task buttons ---------- */
    void SetTask(JobType job, GameObject menu)
    {
        selectedTask = job;
        plowMenu.SetActive(menu == plowMenu);
        seedMenu.SetActive(menu == seedMenu);
        harvestMenu.SetActive(menu == harvestMenu);

        PopulateTractors();
        PopulateImplements();
        if (job == JobType.Seed) PopulateCrops();

        ValidateReady();
    }

    void PopulateTractors()
    {
        idleTractors = VehicleManager.Instance.IdleVehicles
            .Where(v => v.CanDo(selectedTask)).ToList();
        var names = idleTractors.Select(t => t.DisplayName).ToList();

        switch (selectedTask)
        {
            case JobType.Plow:
                dpnTractorSelectPlow.ClearOptions();
                dpnTractorSelectPlow.AddOptions(names);
                break;
            case JobType.Seed:
                dpnTractorSelectSeed.ClearOptions();
                dpnTractorSelectSeed.AddOptions(names);
                break;
            case JobType.Harvest:
                dpnTractorSelectHarvest.ClearOptions();
                dpnTractorSelectHarvest.AddOptions(names);
                break;
        }
    }

    void PopulateImplements()
    {
        implementChoices.Clear();
        var shed = BuildingManager.Instance.GetNearestShed(field.transform.position);
        if (!shed) return;

        implementChoices = shed.Query(i => i.Job == selectedTask).ToList();
        var labels = implementChoices.Select(i => $"{i.name} ({i.Durability:P0})").ToList();

        TMP_Dropdown dd = selectedTask switch
        {
            JobType.Plow => dpnImplementSelectPlow,
            JobType.Seed => dpnImplementSelectSeed,
            _ => null
        };

        if (dd != null)
        {
            dd.ClearOptions();
            dd.AddOptions(labels);
        }
    }

    void PopulateCrops()
    {
        dpnCropSelectSeed.ClearOptions();
        var cropNames = cropRegistry.crops.Select(c => c.cropName).ToList();
        dpnCropSelectSeed.AddOptions(cropNames);
    }

    int CurrentTractorIndex() => selectedTask switch
    {
        JobType.Plow => dpnTractorSelectPlow.value,
        JobType.Seed => dpnTractorSelectSeed.value,
        JobType.Harvest => dpnTractorSelectHarvest.value,
        _ => -1
    };

    int CurrentImplementIndex() => selectedTask switch
    {
        JobType.Plow => dpnImplementSelectPlow.value,
        JobType.Seed => dpnImplementSelectSeed.value,
        _ => -1
    };

    public void OnStartClicked()
    {
        if (idleTractors.Count == 0 || CurrentTractorIndex() < 0) return;
        var tractor = idleTractors[CurrentTractorIndex()];

        var shed = BuildingManager.Instance.GetNearestShed(field.transform.position);
        string implementId = null;

        if (selectedTask is JobType.Plow or JobType.Seed)
        {
            if (implementChoices.Count == 0 || CurrentImplementIndex() < 0) return;
            implementId = implementChoices[CurrentImplementIndex()].id;
            shed?.Reserve(implementId);
        }

        CropDefinition crop = selectedTask == JobType.Seed
            ? cropRegistry.crops[dpnCropSelectSeed.value]
            : null;

        var job = new FieldJob(field, selectedTask, crop, implementId);
        GameEvents.JobButtonPressed(job, tractor);
        Hide();
    }

    public void OnDropdownChanged(int _) => ValidateReady();

    void ValidateReady()
    {
        bool hasTractor = idleTractors.Count > 0 && CurrentTractorIndex() >= 0;
        bool hasTool = selectedTask switch
        {
            JobType.Plow or JobType.Seed => implementChoices.Count > 0 && CurrentImplementIndex() >= 0,
            _ => true
        };
        bool hasCrop = selectedTask != JobType.Seed || dpnCropSelectSeed.value >= 0;

        btnGo.interactable = hasTractor && hasTool && hasCrop;
    }

}
