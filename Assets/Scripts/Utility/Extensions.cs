using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Extensions
{
    /// <summary>
    /// Extension methods for UnityEngine.GameObject.
    /// </summary>
    public static class GameObjectExtensions
    {
        /// <summary>
        /// Checks whether a game object has a component of type T attached.
        /// </summary>
        /// <param name="gameObject">Game object.</param>
        /// <returns>True when component is attached.</returns>
        public static bool HasComponent<T>(this GameObject gameObject) where T : Component
        {
            return gameObject.GetComponent<T>() != null;
        }
    }


    /// <summary>
    /// Extension methods for UnityEngine.LayerMask.
    /// </summary>
    public static class LayerMaskExtensions
    {
        public static bool HasLayer(this LayerMask layerMask, int layer)
        {
            if (layerMask == (layerMask | (1 << layer)))
            {
                return true;
            }

            return false;
        }

        public static bool[] HasLayers(this LayerMask layerMask)
        {
            var hasLayers = new bool[32];

            for (int i = 0; i < 32; i++)
            {
                if (layerMask == (layerMask | (1 << i)))
                {
                    hasLayers[i] = true;
                }
            }

            return hasLayers;
        }
    }
}
