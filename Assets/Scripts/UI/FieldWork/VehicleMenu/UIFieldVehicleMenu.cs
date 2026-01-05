// Scripts/UI/FieldVehicle/UIFieldVehicleMenu.cs
using Harvey.Farm.Fields;
using Harvey.Farm.Jobs;
using Harvey.Farm.Jobs.VehicleField;

using UnityEngine;
using UnityEngine.UI;

public class UIFieldVehicleMenu : MonoBehaviour
{
    [Header("Pages")]
    [SerializeField] UITabMenu tabMenu;
    [SerializeField] FieldJobInfoPage pageInfo;
    [SerializeField] FieldJobWorkerPage pageWorker;
    [SerializeField] FieldJobVehicleToolPage pageVehicle;
    [SerializeField] FieldJobBeanPage pageBean;

    [Header("Buttons")]
    [SerializeField] Button btnGo;
    [SerializeField] Button btnClose;

    FieldVehicleJobParams model = new();

    void Awake()
    {
        // Wire change events for validation
        pageWorker.OnChanged.AddListener(Validate);
        pageVehicle.OnChanged.AddListener(Validate);
        pageBean.OnChanged.AddListener(Validate);

        tabMenu.OnPageChanged.AddListener(OnTabSwitched);

        btnGo.onClick.AddListener(OnGoClicked);
        //btnClose.onClick.AddListener(() => gameObject.SetActive(false));
    }

    /* ---------- public API ---------- */
    public void Show(FieldController field)
    {
        var GetNeededJobType = field.GetNeededJobType();
        if (GetNeededJobType == null) return;

        model = new FieldVehicleJobParams { Field = field, Task = GetNeededJobType.Value };

        pageInfo.Enter(model);
        pageWorker.Enter(model);
        pageVehicle.Enter(model);
        pageBean.Enter(model);

        tabMenu.JumpToPage(0);
        Validate();
        gameObject.SetActive(true);
    }

    public void Refresh()
    {
        if (model == null) return;

        pageInfo.Enter(model);
        pageWorker.Enter(model);
        pageVehicle.Enter(model);
        pageBean.Enter(model);

        Validate();
    }

    /* ---------- private ---------- */
    void OnTabSwitched(int newIndex)
    {
        // we could call Exit() on old page if you need to store state
    }

    void Validate()
    {
        // Update tab interactability
        UpdateTabInteractability();

        // Debug logging
        Debug.Log($"Validate called - model.IsReady: {model.IsReady}");

        btnGo.interactable = model.IsReady;
    }

    void UpdateTabInteractability()
    {
        bool beanTabInteractable = model.Task == JobType.Seed;

        int beanTabIndex = 3;

        if (beanTabIndex < tabMenu.PageCount)
        {
            tabMenu.SetTabInteractable(beanTabIndex, beanTabInteractable);
        }
    }

    void OnGoClicked()
    {
        if (!model.IsReady) return;

        var def = new VehicleFieldJob(
            model.Field,
            model.Task,
            model.Crop,
            vehicleId: model.Vehicle.GetId(),
            implementId: model.ImplementId);

        var inst = new VehicleFieldJobInstance(def, model.AssignedWorker.GetId());

        JobBoard.Instance.Post(new JobEntry(inst));
        gameObject.SetActive(false);
    }
}
