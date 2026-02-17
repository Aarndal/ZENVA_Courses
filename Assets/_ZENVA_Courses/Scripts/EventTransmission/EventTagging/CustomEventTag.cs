using System;
using UnityEngine;

namespace EventTransmission
{
    [Serializable]
    public class CustomEventTag
    {
        [SerializeField]
        private string name;

        [SerializeField]
        private string id;

        public string Name => name;
        public Guid ID => string.IsNullOrEmpty(id) ? Guid.Empty : Guid.Parse(id);

        public CustomEventTag(string name, Guid guid)
        {
            this.name = name;
            id = guid.ToString();
        }
    }
}
