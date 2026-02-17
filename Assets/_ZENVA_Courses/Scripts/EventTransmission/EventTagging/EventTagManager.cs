using System;
using System.Collections.Generic;
using UnityEngine;

namespace EventTransmission
{
    [CreateAssetMenu(menuName = "EventSystem/EventTagManager")]
    public class EventTagManager : ScriptableObject
    {
        [Serializable]
        public class TagEntry
        {
            public string tagName;
            public string guidString;

            public Guid Guid => Guid.Parse(guidString);

            public TagEntry(string name, Guid guid)
            {
                tagName = name;
                guidString = guid.ToString();
            }
        }

        [SerializeField]
        private List<TagEntry> customTags = new();

        public IReadOnlyList<TagEntry> CustomTags => customTags;

        /// <summary>
        /// Register or fetch a custom tag by name. Creates new Guid if not present.
        /// </summary>
        public Guid RegisterOrGetTag(string tagName)
        {
            var entry = customTags.Find(t => t.tagName == tagName);
            if (entry != null)
                return entry.Guid;

            var guid = Guid.NewGuid();
            entry = new TagEntry(tagName, guid);
            customTags.Add(entry);
            // Optionally: Notify listeners/Inspector of update
            return guid;
        }

        public string GetTagName(Guid guid) =>
            customTags.Find(t => t.Guid == guid)?.tagName;
    }
}
