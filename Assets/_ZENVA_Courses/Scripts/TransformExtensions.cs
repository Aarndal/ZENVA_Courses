using UnityEngine;

public static class TransformExtensions
{
    public static bool TryGetComponentInChildren<T>(this Transform transform, out T component) where T : Component
    {
        if (transform.TryGetComponent(out component))
        {
            return true;
        }

        if (transform.childCount == 0)
        {
            return false;
        }

        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).TryGetComponentInChildren(out component))
            {
                return true;
            }
        }

        return false;
    }
}
