using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Harvey.Farm.Buildings
{
    public class BuildingManager : Singleton<BuildingManager>
    {
        [SerializeField] readonly List<ShedBuilding> sheds = new();
        [SerializeField] readonly List<HouseBuilding> houses = new();

        public void Register(Building b)
        {
            if (b is ShedBuilding shed && !sheds.Contains(shed))
                sheds.Add(shed);
            if (b is HouseBuilding house && !houses.Contains(house))
                houses.Add(house);
        }
        public void Unregister(Building b)
        {
            if (b is ShedBuilding shed) sheds.Remove(shed);
            if (b is HouseBuilding house) houses.Remove(house);
        }

        public ShedBuilding GetNearestShed(Vector3 point)
        {
            ShedBuilding best = null;
            float bestSqr = float.MaxValue;
            foreach (var s in sheds)
            {
                float d = (s.transform.position - point).sqrMagnitude;
                if (d < bestSqr) { best = s; bestSqr = d; }
            }
            return best;
        }

        public IEnumerable<T> GetAllBuildings<T>() where T : Building
        {
            if (typeof(T) == typeof(ShedBuilding))
                return sheds.Cast<T>();
            if (typeof(T) == typeof(HouseBuilding))
                return houses.Cast<T>();

            return Enumerable.Empty<T>();
        }
    }
}
