using System.Collections.Generic;
using Harvey.Farm.Events;
using Harvey.Farm.FieldScripts;
using Harvey.Farm.VehicleScripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

namespace Harvey.Farm.UI
{
    public class FieldJobPanel : MonoBehaviour
    {
        [Header("UI refs")]
        [SerializeField] private TMP_Dropdown dropdown;
        [SerializeField] private Slider progress;
        [SerializeField] private Button plowButton;
        [SerializeField] private TMP_Text statusLabel;

        private Field field;
        private List<Vehicle> idleCache;

        /* ------------------------- PUBLIC API ------------------------- */
        public void Init(Field owner)
        {
            field = owner;
            Hide();
        }

        public void Show(Vector3 worldPos)
        {
            transform.position = new Vector3(worldPos.x, worldPos.y + 3.5f, worldPos.z);

            BuildDropdownOptions();
            UpdateVisualState();
            gameObject.SetActive(true);
        }
        public void Hide() => gameObject.SetActive(false);

        /* ------------------------- BUTTON EVENT ----------------------- */
        public void OnPlowClicked()
        {
            Vehicle chosen = null;

            if (dropdown.interactable && idleCache.Count > 0)
                chosen = idleCache[dropdown.value];

            UIEvents.JobButtonPressed(field, chosen, JobType.Plow);
            Hide();
        }

        /* ------------------------- INTERNAL --------------------------- */
        void Awake()
        {
            GameEvents.OnTilePloughed += _ => UpdateBar();
            GameEvents.OnFieldCompleted += f =>
            {
                if (f == field) progress.value = 1f;
            };
        }

        void BuildDropdownOptions()
        {
            idleCache = VehicleManager.Instance.IdleVehicles.ToList();
            dropdown.ClearOptions();

            if (idleCache.Count == 0)
            {
                dropdown.interactable = false;
                return;
            }

            var names = new List<string>();
            foreach (var v in idleCache)
                names.Add(v.vehicleName);

            dropdown.AddOptions(names);
            dropdown.interactable = true;
        }

        void UpdateVisualState()
        {
            if (field.IsPlowed)
            {
                plowButton.gameObject.SetActive(false);
                dropdown.gameObject.SetActive(false);
                statusLabel.gameObject.SetActive(true);
                statusLabel.text = "Field plowed";
            }
            else if (field.IsPlowing)
            {
                plowButton.gameObject.SetActive(false);
                dropdown.gameObject.SetActive(false);
                statusLabel.gameObject.SetActive(true);
                statusLabel.text = "Plowing in progress…";
            }
            else
            {
                statusLabel.gameObject.SetActive(false);
                plowButton.gameObject.SetActive(true);
                dropdown.gameObject.SetActive(true);
            }
        }

        void UpdateBar()
        {
            if (!field || progress == null) return;
            progress.value = field.TilesPlowedFraction;
        }
    }
}
