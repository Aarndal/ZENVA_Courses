using ObjectPools;
using System;
using UnityEditor;
using UnityEngine;

namespace SpawnSystem
{
    public abstract class SpawnableDataSO : ScriptableObject, ISpawnableData
    {
        [SerializeField, HideInInspector]
        private string id = null;

        [SerializeField]
        private GameObject prefab = null;

        public Guid ID
        {
            get
            {
                if (string.IsNullOrEmpty(id))
                {
                    id = Guid.NewGuid().ToString();
                }
                return Guid.Parse(id);
            }
        }
        public string InstanceName => this.name;
        public GameObject Prefab => prefab;
        public IObjectPool<ISpawnable, ISpawnableData> SpawnPool { get; set; }


        private void Awake()
        {
#if UNITY_EDITOR
            if (prefab == null)
            {
                Debug.LogErrorFormat("Prefab reference is not assigned in SpawnableDataProvider: {0} | ID: {1}",
                    this.name,
                    this.GetEntityId());
            }
            else if (PrefabUtility.GetPrefabAssetType(prefab) == PrefabAssetType.NotAPrefab)
            {
                Debug.LogErrorFormat("Assigned GameObject is not a Prefab asset in SpawnableDataProvider: {0} | ID: {1}",
                    this.name,
                    this.GetEntityId());
            }
#endif
        }

        public bool Equals(IDataProvider other)
        {
            return other is ISpawnableData data && data.ID == this.ID;
        }
    }
}
