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

public class UIFieldMenu : MonoBehaviour
{

    [SerializeField] private CropRegistry cropRegistry;

    [Header("UI refs")]
    [SerializeField] private Button btnPlow;
    [SerializeField] private Button btnSeed;
    [SerializeField] private Button btnHarvest;
    [SerializeField] private Button btnClose;
    [SerializeField] private Button btnGo;
    [SerializeField] private GameObject plowMenu, seedMenu, HarvestMenu;

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

    //[Header("Harvest Menu Refs")]

    Field field;
    JobType selectedTask;
    List<Vehicle> idleTractors = new();

    void Awake()
    {
        gameObject.SetActive(false);
    }

    public void Show(Field f)
    {
        field = f;
        gameObject.SetActive(true);

        if (f.Needs(JobType.Plow)) OnClickPlow();
        else if (f.Needs(JobType.Seed)) OnClickSeed();
        else OnClickHarvest();
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }

    /* ---------- task buttons ---------- */
    public void OnClickPlow() => SetTask(JobType.Plow, plowMenu);
    public void OnClickSeed() => SetTask(JobType.Seed, seedMenu);
    public void OnClickHarvest() => SetTask(JobType.Harvest, HarvestMenu);
    void SetTask(JobType t, GameObject active)
    {
        selectedTask = t;
        plowMenu.SetActive(active == plowMenu);
        seedMenu.SetActive(active == seedMenu);
        HarvestMenu.SetActive(active == HarvestMenu);

        PopulateTractors(t);
        PopulateImplements();
        PopulateCrops();
    }

    public void OnStartClicked()
    {
        Vehicle tractor = selectedTask switch
        {
            JobType.Plow => idleTractors[dpnTractorSelectPlow.value],
            JobType.Seed => idleTractors[dpnTractorSelectSeed.value],
            JobType.Harvest => idleTractors[dpnTractorSelectPlow.value], //TODO: Implement Harvesting
            _ => null
        };

        CropDefinition crop = (selectedTask == JobType.Seed)
            ? cropRegistry.crops[dpnCropSelectSeed.value]
            : null;

        var job = new FieldJob(field, selectedTask, crop);
        GameEvents.JobButtonPressed(job, tractor);
        Hide();
    }

    void PopulateTractors(JobType task)
    {
        idleTractors = VehicleManager.Instance
                   .IdleVehicles
                   .Where(v => v.CanDo(task))
                   .ToList();
        var names = idleTractors.ConvertAll(t => t.vehicleName);

        switch (task)
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
                //TODO: Implement Harvesting
                break;
        }
    }
    private void PopulateImplements()
    {

    }
    private void PopulateCrops()
    {
        dpnCropSelectSeed.ClearOptions();
        var names = cropRegistry.crops.ConvertAll(c => c.cropName);
        dpnCropSelectSeed.AddOptions(names);

        dpnCropSelectSeed.value = 0;
    }
}
