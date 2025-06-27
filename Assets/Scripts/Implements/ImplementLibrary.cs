using UnityEngine;
using Harvey.Farm.JobScripts;
using Harvey.Farm.Implements;
using System.Collections.Generic;
using System.Linq;

namespace Harvey.Farm.Implements
{
    [CreateAssetMenu(fileName = "ImplementLibrary", menuName = "Farm/Implement Library")]
    public class ImplementLibrary : ScriptableObject
    {
        [SerializeField] private List<ImplementDefinition> all = new();

        // ---------- Runtime look-up ----------
        Dictionary<JobType, List<ImplementDefinition>> byJob;

        void OnEnable()
        {
            byJob = all.GroupBy(d => d.Job)
                       .ToDictionary(g => g.Key, g => g.ToList());
        }

        public List<ImplementDefinition> GetAllForJob(JobType job) =>
            byJob.TryGetValue(job, out var list) ? list : new List<ImplementDefinition>();

        public ImplementDefinition GetFirstForJob(JobType job) =>
            GetAllForJob(job).FirstOrDefault();


        // ---------- Data-singleton pattern ----------
        static ImplementLibrary _instance;
        public  static ImplementLibrary Instance
        {
            get
            {
                if (_instance) return _instance;

                _instance = Resources.Load<ImplementLibrary>("ImplementLibrary");

                if (!_instance)
                    Debug.LogError("ImplementLibrary asset not found in a Resources folder!");

                return _instance;
            }
        }
    }
}