using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections.Generic;

public class FieldJobInfoPage : MonoBehaviour, IFieldVehiclePage
{
    [SerializeField] TMP_Text txtFieldName;
    [SerializeField] TMP_Text txtNeeds;
    public UnityEvent OnChanged { get; } = new(); //Read only so no changes expected here

    public void Enter(FieldVehicleJobParams model)
    {
        txtFieldName.text = model.Field.DisplayName;

        var needs = new List<string>();
        if (model.Field.Needs(JobType.Plow)) needs.Add("Plow");
        if (model.Field.Needs(JobType.Seed)) needs.Add("Seed");
        if (model.Field.Needs(JobType.Harvest)) needs.Add("Harvest");
        txtNeeds.text = needs.Count > 0 ? string.Join(",\n", needs) : "No work";
    }

    public void Exit() { }
}
