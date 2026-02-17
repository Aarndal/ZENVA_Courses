using System;
using UnityEngine;

[Serializable]
public class AnimationEventBroadcaster : MonoBehaviour
{
    public event Action<AnimationEvent> AnimationEventTriggered;

    public void OnAnimationEvent(AnimationEvent eventArgs)
    {
        AnimationEventTriggered?.Invoke(eventArgs);
    }
}