using System.Collections.Generic;
using Harvey.Farm.Events;
using Harvey.Farm.FieldScripts;
using Harvey.Farm.JobScripts;
using Harvey.Farm.VehicleScripts;
using UnityEngine;
using Harvey.Farm.Utilities;
using UnityEngine.Playables;

namespace Harvey.Farm.UI
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }
        public static void Notify(in NotificationData data) => Instance?.ShowNotification(data);


        [SerializeField] private Transform canvasTransform;
        [SerializeField] private Transform notificationContainer;


        [Header("Fading Popup Config")]
        [SerializeField] private GameObject fadingTextPopupPrefab;
        [SerializeField] private PopupMessageModel jobStartedPopupMsg;

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
        }
        void OnDisable()
        {
            UIEvents.OnJobButtonPressed -= HandleJobBtn;
            GameEvents.OnJobStarted -= HandleJobStarted;
        }

        public void Register(FieldJobPanel p) => panels.Add(p);
        public void CloseAll() { foreach (var p in panels) p.Hide(); }

        void HandleJobBtn(Field f, Vehicle v, JobType t)
        {
            JobManager.Instance.EnqueueJob(f, v, t);
        }

        private void HandleJobStarted(Vehicle v, Field f, JobType j)
        {
            JobStartedPopup(v, f, j);

            var n = new NotificationData
            (
                $"{v.vehicleName} started to {j} on {f.fieldName}",
                textColor: Color.white,
                backgroundColor: new Color(0.15f, 0.6f, 0.1f),
                fadeDuration: 4f
            );

            ShowNotification(n);
        }

        private void JobStartedPopup(Vehicle v, Field f, JobType j)
        {
            var go = fadingPopupPool.UIGetOrInstantiate();
            var popup = go.GetComponent<FadingPopupText>();

            string popupText = v.vehicleName + " started work on field " + f.fieldName;
            jobStartedPopupMsg.text = popupText;

            popup.Show(jobStartedPopupMsg, () => fadingPopupPool.UIRelease(go));
        }

        public void ShowNotification(in NotificationData data)
        {
            var go = notificationPopupPool.UIGetOrInstantiate();
            var popup = go.GetComponent<NotificationPopup>();
            popup.Setup(data, () => notificationPopupPool.UIRelease(go));
        }
    }
}