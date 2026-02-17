using System;
using System.Collections.Generic;
using UnityEngine;

namespace EventTransmission
{
    [Serializable]
    public class EventTagsContainer
    {
        [Header("Event Categories")]
        [Tooltip("BitFlag-based tags that are fixed.")]
        public EventCategoryFlags EventTagFlags;

        [Header("Event Tags")]
        [Tooltip("Guid-based tags that can be enhanced.")]
        public List<CustomEventTag> customTags = new();
    }
}
