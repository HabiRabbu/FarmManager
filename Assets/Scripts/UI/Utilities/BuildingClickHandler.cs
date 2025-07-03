using UnityEngine;
using Harvey.Farm.Buildings;
using Harvey.Farm.UI;

[RequireComponent(typeof(Building), typeof(Collider))]
public class BuildingClickHandler : MonoBehaviour
{
    void OnMouseDown()
    {
        Debug.Log($"Clicked on building: {gameObject.name}");

        var building = GetComponent<Building>();
        if (building != null)
        {
            UIManager.Instance.OpenBuildingInfo(building);
        }
    }



}
