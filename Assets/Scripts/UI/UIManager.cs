using System.Collections.Generic;
using Harvey.Farm.Events;
using Harvey.Farm.FieldScripts;
using Harvey.Farm.JobScripts;
using Harvey.Farm.VehicleScripts;
using UnityEngine;
using Harvey.Farm.Utilities;
using UnityEngine.Playables;
using UnityEngine.UI;
using UnityEditor.Rendering.Universal.ShaderGraph;

namespace Harvey.Farm.UI
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }
        public static void Notify(in NotificationData data) => Instance?.ShowNotification(data);
        public static void CentrePopup(in FadingPopupData data) => Instance?.ShowCentrePopup(data);


        [SerializeField] private Transform canvasTransform;
        [SerializeField] private Transform notificationContainer;


        [Header("Fading Popup Config")]
        [SerializeField] private GameObject fadingTextPopupPrefab;

        [Header("Notification Popup Config")]
        [SerializeField] private GameObject notificationPopupPrefab;

        // TODO: Eventually handle removing based on player input: 
        // private readonly List<NotificationPopup> notifications = new();
        // notifications.Add(entry); and .Remove(entry);

        //Pools
        private UIPrefabPool fadingPopupPool;
        private UIPrefabPool notificationPopupPool;


        void Awake()
        {
            fadingPopupPool = new UIPrefabPool(fadingTextPopupPrefab, canvasTransform);
            notificationPopupPool = new UIPrefabPool(notificationPopupPrefab, notificationContainer);

            if (Instance != null && Instance != this) Destroy(gameObject);
            else Instance = this;
        }
        readonly List<FieldJobPanel> panels = new();

        void OnEnable()
        {
            UIEvents.OnJobButtonPressed += HandleJobBtn;
            GameEvents.OnJobStarted += HandleJobStarted;
            GameEvents.OnFieldCompleted += HandleFieldCompleted;
        }
        void OnDisable()
        {
            UIEvents.OnJobButtonPressed -= HandleJobBtn;
            GameEvents.OnJobStarted -= HandleJobStarted;
            GameEvents.OnFieldCompleted -= HandleFieldCompleted;
        }

        public void Register(FieldJobPanel p) => panels.Add(p);
        public void CloseAll() { foreach (var p in panels) p.Hide(); }

        void HandleJobBtn(Field f, Vehicle v, JobType t)
        {
            JobManager.Instance.EnqueueJob(f, v, t);
        }

        private void HandleJobStarted(Vehicle v, Field f, JobType j)
        {

            var n = new NotificationData
            (
                $"{v.vehicleName} started to {j} on {f.fieldName}",
                textColor: Color.white,
                backgroundColor: new Color(0.15f, 0.6f, 0.1f),
                fadeDuration: 4f
            );

            ShowNotification(n);
        }

        private void HandleFieldCompleted(Field field)
        {
            var n = new FadingPopupData
            (
                text: $"Work completed on field {field.fieldName}",
                color: Color.white,
                fadeDuration: 2f
            );

            ShowCentrePopup(n);
        }

        public void ShowCentrePopup(in FadingPopupData data)
        {
            var go = fadingPopupPool.UIGetOrInstantiate();
            var popup = go.GetComponent<FadingPopupText>();
            popup.Show(data, () => notificationPopupPool.UIRelease(go));
        }

        public void ShowNotification(in NotificationData data)
        {
            var go = notificationPopupPool.UIGetOrInstantiate();
            var popup = go.GetComponent<NotificationPopup>();
            popup.Setup(data, () => notificationPopupPool.UIRelease(go));
        }
    }
}