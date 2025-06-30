using System.Collections.Generic;
using UnityEngine;
using Harvey.Farm.Utilities;
using Harvey.Farm.Events;

namespace Harvey.Farm.Fields
{
    public class FieldManager : Singleton<FieldManager>
    {
        [Header("Field Settings")]
        [SerializeField] public float tileSize;

        private FieldController current = null;
        readonly List<FieldController> fields = new();

        public void RegisterField(FieldController f) => fields.Add(f);
        public void UnregisterField(FieldController f) => fields.Remove(f);

        public FieldController GetFieldAtPoint(Vector3 worldPos)
        {
            return fields.Find(f => f.GetComponent<FieldBuilder>().ContainsPoint(worldPos));
        }

        public void SelectField(FieldController f)
        {
            if (current == f) return;

            //if (current) current.ShowOutline(false);   // deselect previous
            current = f;
            //if (current) current.ShowOutline(true);    // show new outline

            GameEvents.FieldSelected(current);
        }
    }
}

