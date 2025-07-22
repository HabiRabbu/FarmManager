using System.Collections.Generic;
using Harvey.Farm.Events;
using Harvey.Farm.Jobs;
using Harvey.Farm.VehicleScripts;
using UnityEngine;
using Harvey.Farm.Utilities;
using UnityEngine.Playables;
using UnityEngine.UI;
using Harvey.Farm.Fields;
using Harvey.Farm.Buildings;
using UnityEditor.IMGUI.Controls;
using Harvey.Farm.UI.Radial;
using Harvey.Farm.Factory;
using Harvey.Farm.Managers;

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
        [SerializeField] private GameObject fieldTractorMenuPrefab;
        [SerializeField] private GameObject fieldWorkerMenuPrefab;

        [Header("Building Info Config")]
        [SerializeField] private GameObject buildingInfoPrefab;

        [Header("Options Menu Config")]
        [SerializeField] private GameObject optionsMenuPrefab;

        [Header("Radial Menu Config")]
        [SerializeField] GameObject radialPrefab;


        //Public Getters
        public Transform CanvasTransform => canvasTransform;

        //Current UI
        private RadialMenuController radialMenu;
        private UIFieldInfo fieldInfo;
        private UITractorMenu fieldTractorMenu;
        private UIWorkerMenu fieldWorkerMenu;
        private UIBuildingInfo buildingInfo;
        private OptionsMenuController optionsMenu;

        protected override void Awake()
        {
            base.Awake();

            radialMenu = UIFactory.Instance.Spawn(radialPrefab, canvasTransform).GetComponent<RadialMenuController>();
            radialMenu.gameObject.SetActive(false);

            fieldInfo = UIFactory.Instance.Spawn(fieldInfoPrefab, canvasTransform).GetComponent<UIFieldInfo>();
            fieldInfo.gameObject.SetActive(false);

            fieldTractorMenu = UIFactory.Instance.Spawn(fieldTractorMenuPrefab, canvasTransform).GetComponent<UITractorMenu>();

            fieldWorkerMenu = UIFactory.Instance.Spawn(fieldWorkerMenuPrefab, canvasTransform).GetComponent<UIWorkerMenu>();

            buildingInfo = UIFactory.Instance.Spawn(buildingInfoPrefab, canvasTransform).GetComponent<UIBuildingInfo>();
            buildingInfo.gameObject.SetActive(false);

            optionsMenu = UIFactory.Instance.Spawn(optionsMenuPrefab, canvasTransform).GetComponent<OptionsMenuController>();
            optionsMenu.gameObject.SetActive(false);

        }

        #region Event Handlers
        void OnEnable()
        {
            GameEvents.OnEscapePressed += HandleEscape;

            GameEvents.OnBuildingStatsChanged += RefreshUI;

            GameEvents.OnJobButtonPressed += HandleJobBtn;
            GameEvents.OnJobStarted += HandleJobStarted;

            GameEvents.OnFieldCompleted += HandleFieldCompleted;
            GameEvents.OnFieldGrown += HandleFieldGrown;
            GameEvents.OnFieldHarvested += HandleFieldHarvested;

            GameEvents.OnRadialFieldInfoOpened += OpenFieldInfo;
            GameEvents.OnRadialFieldTractorOpened += OpenTractorJobMenu;
            GameEvents.OnRadialFieldWorkersOpened += OpenWorkerJobMenu;

            GameEvents.OnRadialBuildingInfoOpened += OpenBuildingInfo;

            GameEvents.OnOptionsMenuOpened += HandleOptionsMenuOpened;
            GameEvents.OnOptionsMenuClosed += HandleOptionsMenuClosed;
        }
        void OnDisable()
        {
            GameEvents.OnEscapePressed -= HandleEscape;

            GameEvents.OnBuildingStatsChanged -= RefreshUI;

            GameEvents.OnJobButtonPressed -= HandleJobBtn;
            GameEvents.OnJobStarted -= HandleJobStarted;

            GameEvents.OnFieldCompleted -= HandleFieldCompleted;
            GameEvents.OnFieldGrown -= HandleFieldGrown;
            GameEvents.OnFieldHarvested += HandleFieldHarvested;

            GameEvents.OnRadialFieldInfoOpened -= OpenFieldInfo;
            GameEvents.OnRadialFieldTractorOpened -= OpenTractorJobMenu;
            GameEvents.OnRadialFieldWorkersOpened -= OpenWorkerJobMenu;

            GameEvents.OnRadialBuildingInfoOpened -= OpenBuildingInfo;

            GameEvents.OnOptionsMenuOpened -= HandleOptionsMenuOpened;
            GameEvents.OnOptionsMenuClosed -= HandleOptionsMenuClosed;
        }
        #endregion

        public bool IsAnyUIOpen()
        {
            return (radialMenu && radialMenu.gameObject.activeSelf) ||
                   (fieldInfo && fieldInfo.gameObject.activeSelf) ||
                   (fieldTractorMenu && fieldTractorMenu.gameObject.activeSelf) ||
                   (fieldWorkerMenu && fieldWorkerMenu.gameObject.activeSelf) ||
                   (buildingInfo && buildingInfo.gameObject.activeSelf) ||
                   (optionsMenu && optionsMenu.gameObject.activeSelf);
        }

        public void HandleEscape()
        {
            if (IsAnyUIOpen())
            {
                CloseAll();
            }
            else
            {
                OpenOptionsMenu();
            }
        }

        public void CloseAll()
        {
            if (radialMenu) radialMenu.gameObject.SetActive(false);
            if (fieldInfo) fieldInfo.gameObject.SetActive(false);
            if (fieldTractorMenu) fieldTractorMenu.gameObject.SetActive(false);
            if (fieldWorkerMenu) fieldWorkerMenu.gameObject.SetActive(false);
            if (buildingInfo) buildingInfo.gameObject.SetActive(false);
            if (optionsMenu)
            {
                optionsMenu.Hide();
                if (GameManager.Instance.CurrentState == GameState.Paused)
                {
                    GameManager.Instance.TogglePause();
                }
            }

            Debug.Log("Hide by CloseAll");
        }

        void RefreshUI()
        {
            if (fieldTractorMenu && fieldTractorMenu.gameObject.activeSelf)
            {
                fieldTractorMenu.Refresh();
            }
            if (fieldInfo && fieldInfo.gameObject.activeSelf)
            {
                fieldInfo.Refresh();
            }
            if (buildingInfo && buildingInfo.gameObject.activeSelf)
            {
                buildingInfo.Refresh();
            }
            if (fieldWorkerMenu && fieldWorkerMenu.gameObject.activeSelf)
            {
                fieldWorkerMenu.Refresh();
            }
        }

        // -------- Handle UI Events Methods --------
        void HandleJobBtn(FieldJob j, Vehicle v)
        {
            JobManager.Instance.EnqueueJob(j, v);
        }

        private void HandleJobStarted(IJobAgent agent, FieldJob j)
        {

            var n = new NotificationData
            (
                $"{agent.DisplayName} started to {j.Type} on {j.Field.Model.DisplayName}",
                textColor: Color.white,
                backgroundColor: new Color(0.15f, 0.6f, 0.1f),
                fadeDuration: 4f
            );

            ShowNotification(n);
        }

        private void HandleFieldCompleted(FieldController field)
        {
            var n = new FadingPopupData
            (
                text: $"Work completed on field {field.Model.DisplayName}",
                color: Color.white,
                fadeDuration: 2f
            );

            ShowCentrePopup(n);
        }

        private void HandleFieldGrown(FieldController field)
        {
            var n = new NotificationData
            (
                $"Crops on {field.Model.DisplayName} are ready to harvest!",
                textColor: Color.white,
                backgroundColor: Colors.COLOR_YELLOW,
                fadeDuration: 6f
            );

            ShowNotification(n);
        }

        private void HandleFieldHarvested(FieldController field)
        {
            var n = new NotificationData
            (
                $"{field.currentCrop.DisplayName} on {field.Model.DisplayName} has been harvested.",
                textColor: Color.white,
                backgroundColor: Colors.COLOR_TEAL,
                fadeDuration: 6f
            );

            ShowNotification(n);
        }

        private void HandleOptionsMenuOpened()
        {
            Debug.Log("Options menu opened");
        }

        private void HandleOptionsMenuClosed()
        {
            // Options menu closed - could add logic here if needed
            Debug.Log("Options menu closed");
        }

        // -------- Show/Open UI Methods --------

        public void OpenOptionsMenu()
        {
            if (!optionsMenu)
            {
                Debug.LogError("Options Menu UI is not initialized.");
                return;
            }

            optionsMenu.Show();

            GameEvents.OptionsMenuOpened();
        }

        public void ShowRadial(IRadialProvider provider, Vector2 screenPos)
        {
            if (provider == null)
            {
                CloseAll();
                return;
            }

            radialMenu.Show(screenPos, provider.BuildRadialItems());
        }

        public void OpenFieldInfo(FieldController f)
        {
            if (!fieldInfo)
            {
                Debug.LogError("Field Info UI is not initialized.");
                return;
            }

            fieldInfo.gameObject.SetActive(true);
            fieldInfo.Bind(f);

            if (fieldTractorMenu) fieldTractorMenu.gameObject.SetActive(false);
            if (fieldWorkerMenu) fieldWorkerMenu.gameObject.SetActive(false);
        }

        public void OpenTractorJobMenu(FieldController f)
        {
            if (!fieldTractorMenu)
            {
                Debug.LogError("Field Menu UI is not initialized.");
                return;
            }

            Debug.Log("UIManager.OpenFieldMenu fired for " + f?.name);
            fieldTractorMenu.gameObject.SetActive(true);
            fieldTractorMenu.Show(f);
        }

        public void OpenWorkerJobMenu(FieldController field)
        {
            if (!fieldWorkerMenu)
            {
                Debug.LogError("Worker menu not set up"); return;
            }
            fieldWorkerMenu.Show(field);

            if (fieldTractorMenu) fieldTractorMenu.gameObject.SetActive(false);
        }

        public void OpenBuildingInfo(Building b)
        {
            if (b == null)
            {
                if (buildingInfo)
                {
                    buildingInfo.Close();
                }
                return;
            }
            if (buildingInfo)
            {
                buildingInfo.Show(b);
            }
        }

        public void ShowCentrePopup(in FadingPopupData data)
        {
            var go = UIFactory.Instance.Spawn(fadingTextPopupPrefab, canvasTransform);
            var popup = go.GetComponent<FadingPopupText>();
            popup.Show(data, () => UIFactory.Instance.Despawn(fadingTextPopupPrefab, go));
        }

        public void ShowNotification(in NotificationData data)
        {
            var go = UIFactory.Instance.Spawn(notificationPopupPrefab, notificationContainer);
            var popup = go.GetComponent<NotificationPopup>();
            popup.Setup(data, () => UIFactory.Instance.Despawn(notificationPopupPrefab, go));
        }
    }
}