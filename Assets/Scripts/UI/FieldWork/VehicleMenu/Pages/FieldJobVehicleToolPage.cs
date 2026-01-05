using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using Harvey.Farm.VehicleScripts;
using UnityEngine.Events;
using Harvey.Farm.Buildings;
using Harvey.Farm.Implements;

public class FieldJobVehicleToolPage : MonoBehaviour, IFieldVehiclePage
{
    [SerializeField] TMP_Dropdown dpnTractor;
    [SerializeField] TMP_Dropdown dpnImplement;

    FieldVehicleJobParams model;
    public UnityEvent OnChanged { get; } = new();

    List<Vehicle> tractorChoices = new();
    List<ImplementBehaviour> implementChoices = new();

    public void Enter(FieldVehicleJobParams m)
    {
        model = m;
        RefreshLists();
    }

    public void Exit() { }

    /* ---------- UI callbacks ---------- */
    public void OnDropdownTractor(int _)
    {
        ValidateCurrentDropdowns();
        OnChanged.Invoke();
    }

    public void OnDropdownImplement(int _)
    {
        ValidateCurrentDropdowns();
        OnChanged.Invoke();
    }

    void RefreshLists()
    {
        tractorChoices = VehicleManager.Instance.IdleVehicles
                         .Where(v => v.CanDo(model.Task)).ToList();
        dpnTractor.ClearOptions();
        dpnTractor.AddOptions(tractorChoices.Select(t => t.DisplayName).ToList());

        if (tractorChoices.Count > 0)
        {
            dpnTractor.value = 0;
            model.Vehicle = tractorChoices[0];
        }
        else
        {
            model.Vehicle = null;
        }

        // 2. implement list (skip for harvest)
        if (model.Task is JobType.Harvest)
        {
            model.ImplementId = null;
            dpnImplement.ClearOptions();
            dpnImplement.interactable = false;
            OnChanged.Invoke();
            return;
        }
        else
        {
            dpnImplement.interactable = true;
        }

        implementChoices = ImplementManager.Instance
                                .GetAllByType(model.Task == JobType.Plow
                                              ? ImplementType.Plow
                                              : ImplementType.Seeder)
                                .ToList();
        //TODO: There will be points where the implement is busy - Workers need to check "Is the tractor and implement available?"
        // If not, wait until another job finishes -> Make sure that fires off an event to worker brains that the board has changed

        dpnImplement.ClearOptions();
        dpnImplement.AddOptions(implementChoices
                                .Select(i => $"{i.name} ({i.Durability:P0})").ToList());

        // Auto-select first implement if available
        if (implementChoices.Count > 0)
        {
            dpnImplement.value = 0;
        }
        else
        {
            model.ImplementId = null;
        }

        ValidateCurrentDropdowns();
        OnChanged.Invoke();
    }

    void ValidateCurrentDropdowns()
    {
        model.Vehicle = (tractorChoices.Count > 0 && dpnTractor.value < tractorChoices.Count)
            ? tractorChoices[dpnTractor.value]
            : null;

        model.ImplementId = (implementChoices.Count > 0 && dpnImplement.value < implementChoices.Count)
            ? implementChoices[dpnImplement.value].Model.Id
            : null;
    }
}