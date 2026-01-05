using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Harvey.Farm.Fields;
using Harvey.Farm.Jobs;
using Harvey.Farm.Events;
using Harvey.Farm.Workers;
using Harvey.Data.Coffee;

public class UIWorkerMenu : MonoBehaviour
{
    [Header("Tabs")]
    [SerializeField] Button btnPlow;
    [SerializeField] Button btnSeed;
    [SerializeField] Button btnHarvest;

    [Header("Grids")]
    [SerializeField] Transform idleGrid;
    [SerializeField] Transform selGrid;
    [SerializeField] GameObject workerIconPrefab;

    [Header("Controls")]
    [SerializeField] Button btnAdd;
    [SerializeField] Button btnRemove;
    [SerializeField] Button btnGo;

    [Header("Seed Menu Refs")]
    [SerializeField] TMP_Dropdown dpnCropSelectSeed;
    [SerializeField] TMP_Text txtCropSelectSeed;
    [SerializeField] GameObject seedMenu;

    FieldController field;
    JobType currentTask;

    readonly List<Worker> idleWorkers = new();
    readonly List<Worker> selectedWorkers = new();
    readonly List<GameObject> idleIcons = new();
    readonly List<GameObject> selIcons = new();
    List<CoffeeCropData> cropChoices = new();

    void Start()
    {
        btnPlow.onClick.AddListener(() => SetTask(JobType.Plow));
        btnSeed.onClick.AddListener(() => SetTask(JobType.Seed));
        btnHarvest.onClick.AddListener(() => SetTask(JobType.Harvest));

        btnAdd.onClick.AddListener(AddOneWorker);
        btnRemove.onClick.AddListener(RemoveOneWorker);
        btnGo.onClick.AddListener(OnGoClicked);

        dpnCropSelectSeed.onValueChanged.AddListener(_ => ValidateReady());

        gameObject.SetActive(false);
    }

    // -------- public API --------
    public void Show(FieldController f)
    {
        field = f;
        gameObject.SetActive(true);

        currentTask = field.Needs(JobType.Plow) ? JobType.Plow
                    : field.Needs(JobType.Seed) ? JobType.Seed
                    : JobType.Harvest;

        RefreshIdleList();
        selectedWorkers.Clear();

        if (currentTask == JobType.Seed) PopulateCrops();

        Refresh();
    }
    public void Hide() => gameObject.SetActive(false);

    // -------- core logic --------
    void SetTask(JobType job)
    {
        currentTask = job;
        RefreshIdleList();
        selectedWorkers.Clear();

        if (job == JobType.Seed) PopulateCrops();

        Refresh();
    }

    void RefreshIdleList()
    {
        idleWorkers.Clear();
        idleWorkers.AddRange(
            WorkerManager.Instance.GetAllAvailable()
                .Where(w => w.CanDo(currentTask)));
    }

    void PopulateCrops()
    {
        dpnCropSelectSeed.ClearOptions();

        cropChoices = CoffeeManager.Instance.GetAllCoffeeCrops().ToList();
        var cropNames = cropChoices.Select(c => c.DisplayName).ToList();

        dpnCropSelectSeed.AddOptions(cropNames);
        dpnCropSelectSeed.value = 0;
    }


    void AddOneWorker()
    {
        if (idleWorkers.Count == 0) return;
        var w = idleWorkers[0];
        idleWorkers.RemoveAt(0);
        selectedWorkers.Add(w);
        Refresh();

        Debug.Log(selectedWorkers.Count + " workers selected, " +
                  idleWorkers.Count + " idle workers left.");
    }
    void RemoveOneWorker()
    {
        if (selectedWorkers.Count == 0) return;
        var w = selectedWorkers[^1];
        selectedWorkers.RemoveAt(selectedWorkers.Count - 1);
        idleWorkers.Insert(0, w);
        Refresh();
    }

    public void Refresh()
    {
        // ----- reuse/recycle idle grid icons -----
        RefreshGrid(idleIcons, idleWorkers, idleGrid);

        // ----- reuse/recycle selected grid icons -----
        RefreshGrid(selIcons, selectedWorkers, selGrid);

        if (seedMenu) seedMenu.SetActive(currentTask == JobType.Seed);

        ValidateReady();
    }

    void RefreshGrid(List<GameObject> icons, List<Worker> workers, Transform parent)
    {
        for (int i = 0; i < icons.Count; i++)
        {
            if (i < workers.Count)
            {
                UpdateIcon(icons[i], workers[i]);
                icons[i].SetActive(true);
            }
            else
            {
                icons[i].SetActive(false);
            }
        }

        for (int i = icons.Count; i < workers.Count; i++)
        {
            icons.Add(SpawnIcon(workers[i], parent));
        }
    }

    void UpdateIcon(GameObject go, Worker w)
    {
        var portrait = go.transform.Find("WorkerPortrait")?.GetComponent<Image>();
        var label = go.GetComponentInChildren<TMP_Text>(true);

        //FIXME: if (portrait) portrait.sprite = w.Model.Portrait;
        if (label) label.text = w.DisplayName;
    }

    void ValidateReady()
    {
        bool hasCrop = currentTask != JobType.Seed || dpnCropSelectSeed.value >= 0;
        btnAdd.interactable = idleWorkers.Count > 0;
        btnRemove.interactable = selectedWorkers.Count > 0;
        btnGo.interactable = selectedWorkers.Count > 0 && hasCrop;
    }

    GameObject SpawnIcon(Worker w, Transform parent)
    {
        var go = Instantiate(workerIconPrefab, parent);

        var portrait = go.transform.Find("WorkerPortrait")?.GetComponent<Image>();
        var label = go.GetComponentInChildren<TMP_Text>(true);

        //FIXME: if (portrait) portrait.sprite = w.Model.Portrait;
        if (label) label.text = w.DisplayName;

        return go;
    }

    void OnGoClicked()
    {

        CoffeeCropData selectedCrop =
            currentTask == JobType.Seed && dpnCropSelectSeed.value >= 0
            ? cropChoices[dpnCropSelectSeed.value]
            : null;

        if (field == null)
        {
            Debug.LogError("Field is null in OnGoClicked");
            return;
        }

        int workerIndex = 0;
        foreach (var w in selectedWorkers)
        {

            if (w == null)
            {
                Debug.LogError($"Worker at index {workerIndex} is null!");
                workerIndex++;
                continue;
            }

            var jobData = new FieldJob(field, currentTask, selectedCrop);
            var jobInst = new FieldJobInstance(jobData, w.GetId());
            JobBoard.Instance.Post(new JobEntry(jobInst));

            workerIndex++;
        }

        Hide();
    }

}
