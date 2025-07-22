using Harvey.Farm.Managers;
using Harvey.Farm.Events;
using UnityEngine;
using UnityEngine.UI;

namespace Harvey.Farm.UI
{
    public class OptionsMenuController : MonoBehaviour
    {
        [SerializeField] private GameObject optionsSelection;
        [SerializeField] private Button btnResume;
        [SerializeField] private Button btnSaveLoad;
        [SerializeField] private Button btnOptions;
        [SerializeField] private Button btnQuit;

        [SerializeField] private SaveLoadPanelController saveLoadPanel;

        void Awake()
        {
            btnResume.onClick.AddListener(OnResumePressed);
            btnSaveLoad.onClick.AddListener(OnSaveLoadPressed);
            btnOptions.onClick.AddListener(OnOptionsPressed);
            btnQuit.onClick.AddListener(OnQuitPressed);
            gameObject.SetActive(false);
        }

        public void Show()
        {
            gameObject.SetActive(true);

            //Close panels if open
            saveLoadPanel.Close();
        }

        public void Hide()
        {
            GameEvents.OptionsMenuClosed();
            gameObject.SetActive(false);
        }

        public void OnResumePressed()
        {
            Hide();
        }

        public void OnSaveLoadPressed()
        {
            optionsSelection.SetActive(false);
            saveLoadPanel.Open();
        }

        public void OnOptionsPressed()
        {
            Debug.Log("Options button pressed");
        }

        public void OnQuitPressed()
        {
            Application.Quit();
        }
    }
}
