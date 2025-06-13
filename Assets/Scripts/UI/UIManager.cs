using System.Collections.Generic;
using Harvey.Farm.Events;
using Harvey.Farm.FieldScripts;
using Harvey.Farm.JobScripts;
using Harvey.Farm.VehicleScripts;
using UnityEngine;

namespace Harvey.Farm.UI
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }
        void Awake()
        {
            if (Instance != null && Instance != this) Destroy(gameObject);
            else Instance = this;
        }

        readonly List<FieldJobPanel> panels = new();
        void OnEnable() { UIEvents.OnJobButtonPressed += HandleJobBtn; }
        void OnDisable() { UIEvents.OnJobButtonPressed -= HandleJobBtn; }

        public void Register(FieldJobPanel p) => panels.Add(p);
        public void CloseAll() { foreach (var p in panels) p.Hide(); }

        void HandleJobBtn(Field f, Vehicle v)
        {
            JobManager.Instance.EnqueuePlowJob(f, v);
        }
    }
}