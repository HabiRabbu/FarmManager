using System.Collections.Generic;
using Harvey.Farm.Events;
using Harvey.Farm.FieldScripts;
using Harvey.Farm.JobScripts;
using Harvey.Farm.VehicleScripts;
using UnityEngine;
using Harvey.Farm.Utilities;
using UnityEngine.Playables;
using UnityEngine.UI;

namespace Harvey.Farm.UI
{
    public class UIManager : Singleton<UIManager>
    {
        public static void Notify(in NotificationData data) => Instance?.ShowNotification(data);
        public static void CentrePopup(in FadingPopupData data) => Instance?.ShowCentrePopup(data);

        [SerializeField] private Transform canvasTransform;
        [SerializeField] private Transform notificationContainer;

        [Header("Notification Popup Config")]
        [SerializeField] private GameObject notificationPopupPrefab;
        [SerializeField] private GameObject fadingTextPopupPrefab;

        [Header("Field UI Config")]
        [SerializeField] private GameObject fieldInfoPrefab;
        [SerializeField] private GameObject fieldMenuPrefab;

        //Pools
        private UIPrefabPool fadingPopupPool;
        private UIPrefabPool notificationPopupPool;

        //Current UI
        private UIFieldInfo fieldInfo;
        private UIFieldMenu fieldMenu;

        protected override void Awake()
        {
            base.Awake();

            fadingPopupPool = new UIPrefabPool(fadingTextPopupPrefab, canvasTransform);
            notificationPopupPool = new UIPrefabPool(notificationPopupPrefab, notificationContainer);

            fieldInfo = Instantiate(fieldInfoPrefab, canvasTransform).GetComponent<UIFieldInfo>();
            fieldInfo.gameObject.SetActive(false);

            fieldMenu = Instantiate(fieldMenuPrefab, canvasTransform).GetComponent<UIFieldMenu>();
            fieldMenu.gameObject.SetActive(false);
        }

        void OnEnable()
        {
            GameEvents.OnFieldSelected += HandleFieldSelected;
            GameEvents.OnCloseUI += CloseAll;

            GameEvents.OnJobButtonPressed += HandleJobBtn;
            GameEvents.OnJobStarted += HandleJobStarted;
            GameEvents.OnFieldCompleted += HandleFieldCompleted;
            GameEvents.OnFieldGrown += HandleFieldGrown;
            GameEvents.OnFieldHarvested += HandleFieldHarvested;
        }
        void OnDisable()
        {
            GameEvents.OnFieldSelected -= HandleFieldSelected;
            GameEvents.OnCloseUI -= CloseAll;

            GameEvents.OnJobButtonPressed -= HandleJobBtn;
            GameEvents.OnJobStarted -= HandleJobStarted;
            GameEvents.OnFieldCompleted -= HandleFieldCompleted;
            GameEvents.OnFieldGrown -= HandleFieldGrown;
            GameEvents.OnFieldHarvested += HandleFieldHarvested;
        }

        public void CloseAll()
        {
            if (fieldInfo) fieldInfo.gameObject.SetActive(false);
            if (fieldMenu) fieldMenu.gameObject.SetActive(false);
        }

        void HandleFieldSelected(Field f)
        {
            if (f)
            {
                OpenFieldInfo(f);
            }
            else
            {
                CloseAll();
            }

        }

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

        private void HandleFieldHarvested(Field field)
        {
            var n = new NotificationData
            (
                $"{field.currentCrop.cropName} on {field.fieldName} has been harvested.",
                textColor: Color.white,
                backgroundColor: Colors.COLOR_TEAL,
                fadeDuration: 6f
            );

            ShowNotification(n);
        }

        public void OpenFieldInfo(Field f)
        {
            if (!fieldInfo)
            {
                Debug.LogError("Field Info UI is not initialized.");
                return;
            }

            fieldInfo.gameObject.SetActive(true);
            fieldInfo.Bind(f);

            if (fieldMenu) fieldMenu.gameObject.SetActive(false);
        }

        public void OpenFieldMenu(Field f)
        {
            if (!fieldMenu)
            {
                Debug.LogError("Field Menu UI is not initialized.");
                return;
            }

            fieldMenu.gameObject.SetActive(true);
            fieldMenu.Show(f);
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