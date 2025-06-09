using System.Collections.Generic;
using UnityEngine;

namespace Harvey.Farm.FieldScripts
{
    public class FieldManager : MonoBehaviour
    {

        [Header("Field Settings")]
        [SerializeField] public float tileSize;


        public static FieldManager Instance { get; private set; }
        readonly List<Field> fields = new();

        void Awake()
        {
            if (Instance != null && Instance != this) Destroy(gameObject);
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }

        public void RegisterField(Field f) => fields.Add(f);
        public void UnregisterField(Field f) => fields.Remove(f);

        public Field GetFieldAtPoint(Vector3 worldPos)
           => fields.Find(f => f.ContainsPoint(worldPos));
    }
}

