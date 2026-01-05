using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using Harvey.Data.Coffee;

public class FieldJobBeanPage : MonoBehaviour, IFieldVehiclePage
{
    [Header("Visibility")]
    [SerializeField] CanvasGroup root;          // fade when task ≠ Seed

    [Header("Grid")]
    [SerializeField] Transform gridRoot;      // parent with GridLayoutGroup
    [SerializeField] GameObject beanIconPrefab;// must have Button + Portrait + Ring

    [Header("Info panel")]
    [SerializeField] TMP_Text txtName;

    FieldVehicleJobParams model;
    readonly List<CoffeeCropData> choices = new();
    readonly List<GameObject> spawned = new();

    public UnityEvent OnChanged { get; } = new();

    /*─────────────────────────────── IFieldVehiclePage ───────────────────────────────*/
    public void Enter(FieldVehicleJobParams m)
    {
        model = m;

        bool active = model.Task == JobType.Seed;
        root.alpha = active ? 1f : 0f;
        root.interactable = active;
        root.blocksRaycasts = active;

        if (active)
        {
            BuildGrid();
            Select(0);                      // auto-pick first crop
        }
        else
        {
            model.Crop = null;
            OnChanged.Invoke();
        }
    }

    public void Exit() { }

    /*─────────────────────────────── Grid builder ───────────────────────────────*/
    void BuildGrid()
    {
        foreach (var go in spawned) Destroy(go);
        spawned.Clear();

        choices.Clear();
        choices.AddRange(CoffeeManager.Instance.GetAllCoffeeCrops());

        for (int i = 0; i < choices.Count; i++)
        {
            var crop = choices[i];
            var go = Instantiate(beanIconPrefab, gridRoot);

            go.name = $"BeanIcon_{crop.DisplayName}";

            // TODO: basic icon
            // var img = go.transform.Find("Portrait")?.GetComponent<Image>();
            // if (img) img.sprite = crop.Icon;

            var label = go.GetComponentInChildren<TMP_Text>();
            if (label) label.text = crop.DisplayName;

            int captured = i;
            go.GetComponent<Button>()
              .onClick.AddListener(() => Select(captured));

            spawned.Add(go);
        }
    }

    /*─────────────────────────────── Selection ───────────────────────────────*/
    void Select(int ix)
    {
        if (ix < 0 || ix >= choices.Count) return;

        // highlight
        for (int i = 0; i < spawned.Count; i++)
        {
            var highlight = spawned[i].transform.Find("Highlight")?.GetComponent<Image>();
            if (highlight) highlight.enabled = (i == ix);
        }

        model.Crop = choices[ix];
        txtName.text = choices[ix].DisplayName;

        OnChanged.Invoke();
    }
}
