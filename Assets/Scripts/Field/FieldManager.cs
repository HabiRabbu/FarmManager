using System.Collections.Generic;
using UnityEngine;
using Harvey.Farm.Utilities;
using Harvey.Farm.Events;

namespace Harvey.Farm.FieldScripts
{
    public class FieldManager : Singleton<FieldManager>
    {
        [Header("Field Settings")]
        [SerializeField] public float tileSize;

        private Field current = null;
        readonly List<Field> fields = new();

        public void RegisterField(Field f) => fields.Add(f);
        public void UnregisterField(Field f) => fields.Remove(f);

        public Field GetFieldAtPoint(Vector3 worldPos)
           => fields.Find(f => f.ContainsPoint(worldPos));

        public void SelectField(Field f)
        {
            if (current == f) return;

            //if (current) current.ShowOutline(false);   // deselect previous
            current = f;
            //if (current) current.ShowOutline(true);    // show new outline

            GameEvents.FieldSelected(current);
        }
    }
}

