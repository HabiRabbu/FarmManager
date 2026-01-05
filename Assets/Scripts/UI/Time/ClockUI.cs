using Harvey.Farm.Events;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

public class ClockUI : MonoBehaviour
{
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private TMP_Text DayText;
    [SerializeField] private TMP_Text SeasonText;


    void OnEnable()
    {
        GameEvents.OnTimeChanged += UpdateClockDisplay;
    }
    void OnDisable()
    {
        GameEvents.OnTimeChanged -= UpdateClockDisplay;
    }
    void OnDestroy()
    {
        GameEvents.OnTimeChanged -= UpdateClockDisplay;
    }

    private void UpdateClockDisplay(int hour)
    {
        timeText.text = $"{hour:00}:00";
    }
}