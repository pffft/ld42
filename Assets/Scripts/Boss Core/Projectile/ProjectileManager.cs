using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Profiling;

namespace Projectiles
{
    public class ProjectileManager : MonoBehaviour
    {
        private static Queue<GameObject> cache;

        public void Start() {
            cache = new Queue<GameObject>(100);
            for (int i = 0; i < 100; i++)
            {
                GameObject obj = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Projectile"));
                obj.SetActive(false);
                cache.Enqueue(obj);
            }
        }

        public void Update()
        {
        }

        public static GameObject Checkout() {
            // Allocate a new GO for the cache
            if (cache.Count == 0)
            {
                GameObject toReturn = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Projectile"));
                return toReturn;
            }
            GameObject obj = cache.Dequeue();
            obj.SetActive(true);
            return obj;
        }

        public static void Return(GameObject obj) {
            obj.transform.position = Vector3.zero;
            obj.transform.rotation = Quaternion.identity;
            obj.transform.localScale = Vector3.one;

            // We don't reset the ProjectileComponent- it'll be reset when it's used next time
            obj.SetActive(false);

            // Add this slot back to the list.
            cache.Enqueue(obj);
        }
    }
}
