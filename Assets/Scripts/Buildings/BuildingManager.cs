using System.Collections.Generic;
using UnityEngine;

namespace Harvey.Farm.Buildings
{
    public class BuildingManager : Singleton<BuildingManager>
    {
        [SerializeField] readonly List<ShedBuilding> sheds = new();

        public void Register(Building b)
        {
            if (b is ShedBuilding shed && !sheds.Contains(shed))
                sheds.Add(shed);
        }
        public void Unregister(Building b)
        {
            if (b is ShedBuilding shed) sheds.Remove(shed);
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
    }
}
