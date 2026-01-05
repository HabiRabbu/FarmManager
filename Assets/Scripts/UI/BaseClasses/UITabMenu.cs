using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UITabMenu : MonoBehaviour
{
    [Header("Current Index")]
    [SerializeField] protected int pageIndex;

    [Header("Components")]
    [SerializeField] protected ToggleGroup _toggleGroup;
    [SerializeField] protected List<Toggle> _tabs = new List<Toggle>();
    [SerializeField] protected List<CanvasGroup> _pages = new List<CanvasGroup>();

    [Header("Events")]
    public UnityEvent<int> OnPageChanged;

    protected virtual void Awake()
    {
        foreach (var toggle in _tabs)
        {
            toggle.onValueChanged.AddListener(CheckForTab);
            toggle.group = _toggleGroup;
        }
    }

    protected virtual void OnDestroy()
    {
        foreach (var toggle in _tabs)
        {
            toggle.onValueChanged.RemoveListener(CheckForTab);
        }
    }

    private void initialise()
    {
        _toggleGroup = GetComponentInChildren<ToggleGroup>();
        _tabs.Clear();
        _pages.Clear();

        _tabs.AddRange(GetComponentsInChildren<Toggle>());
        _pages.AddRange(GetComponentsInChildren<CanvasGroup>());
    }

    private void Reset()
    {
        initialise();
    }

    private void OnValidate()
    {
        initialise();
        OpenPage(pageIndex);
        if (_tabs.Count > pageIndex)
            _tabs[pageIndex].SetIsOnWithoutNotify(true);
    }

    private void CheckForTab(bool isOn)
    {
        for (int i = 0; i < _tabs.Count; i++)
        {
            if (_tabs[i].isOn)
            {
                OpenPage(i);
                return;
            }
        }
    }

    protected virtual void OpenPage(int index)
    {
        EnsureIndexIsInRange(index);

        for (int i = 0; i < _pages.Count; i++)
        {
            bool isActivePage = (i == index);

            _pages[i].alpha = isActivePage ? 1f : 0f;
            _pages[i].interactable = isActivePage;
            _pages[i].blocksRaycasts = isActivePage;
        }

        if (Application.isPlaying)
        {
            OnPageChanged?.Invoke(index);
        }
    }

    private void EnsureIndexIsInRange(int index)
    {
        if (index < 0 || index >= _pages.Count)
        {
            Debug.LogWarning($"Index {index} is out of range. Resetting to 0.");
            pageIndex = 0;
            index = 0;
        }

        pageIndex = index;
    }

    public void JumpToPage(int index)
    {
        EnsureIndexIsInRange(index);
        if (_tabs.Count > pageIndex)
            _tabs[pageIndex].isOn = true;
    }

    public void SetTabInteractable(int tabIndex, bool interactable)
    {
        if (tabIndex >= 0 && tabIndex < _tabs.Count)
        {
            _tabs[tabIndex].interactable = interactable;
        }
    }

    // Public getters for derived classes
    public int CurrentPageIndex => pageIndex;
    public int PageCount => _pages.Count;
}