using System.Collections.Generic;
using Harvey.Farm.Events;
using Harvey.Farm.Fields;
using Harvey.Farm.VehicleScripts;
using Harvey.Farm.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using DG.Tweening;
using Harvey.Farm.JobScripts;

namespace Harvey.Farm.UI
{
    public class FieldJobPanel : MonoBehaviour
    {
        [Header("UI refs")]
        [SerializeField] private TMP_Dropdown dropdown;
        [SerializeField] private Image progressBar;
        [SerializeField] private Button plowButton;
        [SerializeField] private TMP_Text statusLabel;

        private FieldController field;
        private List<Vehicle> idleCache;

        /* ------------------------- PUBLIC API ------------------------- */
        public void Init(FieldController owner)
        {
            field = owner;
            Hide();
        }

        public void Show(Vector3 worldPos)
        {
            transform.position = new Vector3(worldPos.x, worldPos.y + 3.5f, worldPos.z);

            BuildDropdownOptions();
            UpdateVisualState();
            UpdateBar();
            gameObject.SetActive(true);
        }
        public void Hide() => gameObject.SetActive(false);

        /* ------------------------- BUTTON EVENT ----------------------- */
        public void OnPlowClicked()
        {
            Vehicle chosen = null;

            if (dropdown.interactable && idleCache.Count > 0)
                chosen = idleCache[dropdown.value];

            var job = new FieldJob(field, JobType.Plow, null);
            GameEvents.JobButtonPressed(job, chosen);
            Hide();
        }

        public void OnSeedClicked()
        {
            Vehicle chosen = null;

            //Dropdown might need to be reworked idk
            if (dropdown.interactable && idleCache.Count > 0)
                chosen = idleCache[dropdown.value];

            var job = new FieldJob(field, JobType.Seed, null);
            GameEvents.JobButtonPressed(job, chosen);
            Hide();
        }

        /* ------------------------- INTERNAL --------------------------- */
        void Awake()
        {
            GameEvents.OnTilePlowed += _ => UpdateBar();
            GameEvents.OnFieldCompleted += f =>
            {
                if (f == field) progressBar.fillAmount = 1f;
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
                names.Add(v.DisplayName);

            dropdown.AddOptions(names);
            dropdown.interactable = true;
        }

        void UpdateVisualState()
        {
            if (field.Is(FieldController.State.Plowed))
            {
                plowButton.gameObject.SetActive(false);
                dropdown.gameObject.SetActive(false);
                statusLabel.gameObject.SetActive(true);
                statusLabel.text = "Field plowed";
            }
            else if (field.Is(FieldController.State.Plowing))
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
            if (!field || progressBar == null) return;

            float frac = field.TilesCompletedFraction;
            progressBar.fillAmount = frac;

            Color target = frac < 0.5f
                ? Color.Lerp(Colors.COLOR_RED, Colors.COLOR_YELLOW, frac * 2f)
                : Color.Lerp(Colors.COLOR_YELLOW, Colors.COLOR_GREEN, (frac - 0.5f) * 2f);

            progressBar.DOColor(target, 0.25f);
        }
    }
}
