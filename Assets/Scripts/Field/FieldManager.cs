using System.Collections.Generic;
using UnityEngine;
using Harvey.Farm.Utilities;

namespace Harvey.Farm.FieldScripts
{
    public class FieldManager : Singleton<FieldManager>
    {
        [Header("Field Settings")]
        [SerializeField] public float tileSize;

        readonly List<Field> fields = new();

        public void RegisterField(Field f) => fields.Add(f);
        public void UnregisterField(Field f) => fields.Remove(f);

        public Field GetFieldAtPoint(Vector3 worldPos)
           => fields.Find(f => f.ContainsPoint(worldPos));
    }
}

