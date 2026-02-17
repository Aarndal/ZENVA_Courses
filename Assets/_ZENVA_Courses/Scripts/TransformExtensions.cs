using UnityEngine;

public static class TransformExtensions
{
    /// <summary>
    /// Method that tries to get a component of type T on the transform or any of its children. 
    /// Uses Unity's TryGetComponent method to check for the component on the current transform, and if not found, recursively checks each child transform.
    /// </summary>
    /// <typeparam name="T">The type of component to search for.</typeparam>
    /// <param name="transform">The transform to search on.</param>
    /// <param name="component">The found component, if any.</param>
    /// <returns>true if the component is found; otherwise, false.</returns>
    public static bool TryGetComponentInChildren<T>(this Transform transform, out T component)
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
