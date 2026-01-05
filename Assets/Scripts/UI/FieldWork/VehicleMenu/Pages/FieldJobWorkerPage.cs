using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using Harvey.Farm.Workers;

public class FieldJobWorkerPage : MonoBehaviour, IFieldVehiclePage
{
    [Header("Grid")]
    [SerializeField] Transform gridRoot;          // parent with GridLayoutGroup
    [SerializeField] GameObject workerIconPrefab; // see table above

    [Header("Info panel")]
    [SerializeField] TMP_Text txtName;
    [SerializeField] TMP_Text txtHouse;
    [SerializeField] TMP_Text txtActivity;

    FieldVehicleJobParams model;
    readonly List<Worker> choices = new();
    readonly List<GameObject> spawnedIcons = new();

    public UnityEvent OnChanged { get; } = new();

    /*─────────────────────────────── IFieldVehiclePage ───────────────────────────────*/
    public void Enter(FieldVehicleJobParams m)
    {
        model = m;
        BuildGrid();
        Select(0);
    }

    public void Exit() { }

    /*─────────────────────────────── Grid builder ───────────────────────────────*/
    void BuildGrid()
    {
        // destroy old icons
        foreach (var go in spawnedIcons) Destroy(go);
        spawnedIcons.Clear();

        // repopulate worker list (licensed only)
        choices.Clear();
        choices.AddRange(WorkerManager.Instance.AllWorkers
                         .Where(w => w.Model.HasDrivingLicense));

        for (int i = 0; i < choices.Count; i++)
        {
            var w = choices[i];
            var go = Instantiate(workerIconPrefab, gridRoot);
            go.name = $"WorkerIcon_{w.DisplayName}";

            // TODO: basic portrait & label
            //var portrait = go.transform.Find("Portrait")?.GetComponent<Image>();
            //if (portrait) portrait.sprite = w.Model.Portrait;

            var label = go.GetComponentInChildren<TMP_Text>();
            if (label) label.text = w.DisplayName;

            int captured = i;   // avoid modified-closure bug
            go.GetComponent<Button>()
              .onClick.AddListener(() => Select(captured));

            spawnedIcons.Add(go);
        }
    }

    /*─────────────────────────────── Selection ───────────────────────────────*/
    void Select(int ix)
    {
        if (ix < 0 || ix >= choices.Count) return;
        Debug.Log($"Selecting worker #{ix} ({choices[ix].DisplayName})");

        // visual highlight
        for (int i = 0; i < spawnedIcons.Count; i++)
        {
            var highlight = spawnedIcons[i].transform.Find("Highlight")?.GetComponent<Image>();
            if (highlight) highlight.enabled = (i == ix);
        }

        model.AssignedWorker = choices[ix];
        FillInfo(choices[ix]);
        OnChanged.Invoke();
    }

    void FillInfo(Worker w)
    {
        txtName.text = w.DisplayName;
        txtHouse.text = w.GetHomeName() ?? "—";
        txtActivity.text = w.Brain.GetCurrentState().ToString() ?? "Idle";
    }
}
