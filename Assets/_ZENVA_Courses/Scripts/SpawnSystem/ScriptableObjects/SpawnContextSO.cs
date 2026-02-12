using System;
using UnityEngine;

namespace SpawnSystem
{
    [CreateAssetMenu(fileName = "newSpawnContext", menuName = "Spawn System/Spawn Context", order = 0)]
    public class SpawnContextSO : ScriptableObject, ISpawnContext
    {
        [SerializeField, HideInInspector]
        private string id = null;

        [SerializeField]
        private float despawnTimeOut = 0.0f;
        [SerializeField]
        private float spawnDelay = 0.0f;
        [SerializeField]
        private Vector3 targetOffset = Vector3.zero;


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
        
        public float DespawnTimeOut => despawnTimeOut;
        public float SpawnDelay => spawnDelay;
        public Vector3 TargetOffset => targetOffset;


        public bool Equals(IDataProvider other)
        {
            return other is ISpawnContext context && context.ID == this.ID;
        }
    }
}