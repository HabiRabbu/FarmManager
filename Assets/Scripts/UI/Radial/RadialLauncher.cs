using UnityEngine;
using UnityEngine.Events;
using Harvey.Farm.UI.Radial;

[DisallowMultipleComponent]
public class RadialLauncher : MonoBehaviour, IRadialProvider
{
    [System.Serializable]
    public class Slice
    {
        public string name;
        public Sprite icon;
        public UnityEvent onClick; //Event to call in UIManager
    }

    [Tooltip("Max 6 items")]
    [SerializeField] Slice[] slices = new Slice[0];

    public System.Collections.Generic.IReadOnlyList<RadialMenuItem> BuildRadialItems()
    {
        int count = Mathf.Min(6, slices.Length);
        var list = new RadialMenuItem[count];

        for (int i = 0; i < count; i++)
        {
            var s = slices[i];
            list[i] = new RadialMenuItem
            {
                name = s.name,
                icon = s.icon,
                onClick = () => s.onClick?.Invoke()
            };
        }
        return list;
    }
}
