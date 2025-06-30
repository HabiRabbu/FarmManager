using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Harvey.Farm.Fields;
using System.Collections.Generic;
using Harvey.Farm.VehicleScripts;
using Harvey.Farm.Events;
using Harvey.Farm.UI;
using Harvey.Farm.Crops;

public class UIFieldInfo : MonoBehaviour
{
    [Header("UI refs")]
    [SerializeField] private TMP_Text txtFieldName;
    [SerializeField] private TMP_Text txtStatus;
    [SerializeField] private Button btnClose;
    [SerializeField] private Button btnJob;
    [SerializeField] private Image progressBar;

    FieldController boundField;

    System.Action<FieldTile> onPlowed, onHarvested;
    System.Action<FieldTile, CropDefinition> onSeeded;
    System.Action<FieldController> onGrown, onComplete;

    void Awake()
    {
        onPlowed = _ => Refresh();
        onSeeded = (_, __) => Refresh();
        onHarvested = _ => Refresh();
        onGrown = f => { if (f == boundField) Refresh(); };
        onComplete = f => { if (f == boundField) Refresh(); };

        gameObject.SetActive(false);
    }

    void OnEnable()
    {
        GameEvents.OnTilePlowed += onPlowed;
        GameEvents.OnTileSeeded += onSeeded;
        GameEvents.OnTileHarvested += onHarvested;
        GameEvents.OnFieldGrown += onGrown;
        GameEvents.OnFieldCompleted += onComplete;
    }
    void OnDisable()
    {
        GameEvents.OnTilePlowed -= onPlowed;
        GameEvents.OnTileSeeded -= onSeeded;
        GameEvents.OnTileHarvested -= onHarvested;
        GameEvents.OnFieldGrown -= onGrown;
        GameEvents.OnFieldCompleted -= onComplete;
    }

    public void Bind(FieldController f)
    {
        boundField = f;
        txtFieldName.text = f.Definition.fieldName;
        Refresh();
    }

    void Refresh()
    {
        if (!boundField) return;

        // progress bar
        progressBar.fillAmount = boundField.TilesCompletedFraction;

        // status text
        txtStatus.text = boundField.Current.ToString();
    }

    /* ---------- Button ---------- */
    public void OnJobMenuClicked()
    {
        UIManager.Instance.OpenFieldMenu(boundField);
    }

}
