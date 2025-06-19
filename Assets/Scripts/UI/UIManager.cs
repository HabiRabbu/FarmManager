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
    public class UIManager : Singleton<UIManager>
    {
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

        protected override void Awake()
        {
            base.Awake();

            fadingPopupPool = new UIPrefabPool(fadingTextPopupPrefab, canvasTransform);
            notificationPopupPool = new UIPrefabPool(notificationPopupPrefab, notificationContainer);
        }
        readonly List<FieldJobPanel> panels = new();

        void OnEnable()
        {
            GameEvents.OnJobButtonPressed += HandleJobBtn;
            GameEvents.OnJobStarted += HandleJobStarted;
            GameEvents.OnFieldCompleted += HandleFieldCompleted;
            GameEvents.OnFieldGrown += HandleFieldGrown;
        }
        void OnDisable()
        {
            GameEvents.OnJobButtonPressed -= HandleJobBtn;
            GameEvents.OnJobStarted -= HandleJobStarted;
            GameEvents.OnFieldCompleted -= HandleFieldCompleted;
            GameEvents.OnFieldGrown -= HandleFieldGrown;
        }

        public void Register(FieldJobPanel p) => panels.Add(p);
        public void CloseAll() { foreach (var p in panels) p.Hide(); }

        void HandleJobBtn(FieldJob j, Vehicle v)
        {
            JobManager.Instance.EnqueueJob(j.Field, v, j.Type, j.Crop);
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

        private void HandleFieldGrown(Field field)
        {
            var n = new NotificationData
            (
                $"Crops on {field.fieldName} are ready to harvest!",
                textColor: Color.white,
                backgroundColor: Colors.COLOR_YELLOW,
                fadeDuration: 6f
            );

            ShowNotification(n);
        }

        public void ShowCentrePopup(in FadingPopupData data)
        {
            var go = fadingPopupPool.UIGetOrInstantiate();
            var popup = go.GetComponent<FadingPopupText>();
            popup.Show(data, () => fadingPopupPool.UIRelease(go));
        }

        public void ShowNotification(in NotificationData data)
        {
            var go = notificationPopupPool.UIGetOrInstantiate();
            var popup = go.GetComponent<NotificationPopup>();
            popup.Setup(data, () => notificationPopupPool.UIRelease(go));
        }
    }
}