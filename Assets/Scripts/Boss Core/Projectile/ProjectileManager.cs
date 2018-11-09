using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Profiling;

namespace Projectiles
{
    public class ProjectileManager : MonoBehaviour
    {
        public static Queue<GameObject> cache;
        private static Queue<GameObject> hotCache;

        public void Start() {
            cache = new Queue<GameObject>(100);
            hotCache = new Queue<GameObject>(100);
            for (int i = 0; i < 100; i++)
            {
                GameObject obj = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Projectile"));
                obj.SetActive(false);
                cache.Enqueue(obj);
            }
        }

        int count = 0;

        public void Update()
        {
            if (count++ > 10) {
                Profiler.BeginSample("Flushing hot cache");
                count = 0;
                GameObject current;
                while (hotCache.Count > 0) {
                    current = hotCache.Dequeue();
                    current.SetActive(false);
                    cache.Enqueue(current);
                }
                Profiler.EndSample();
            }
        }

        public static GameObject Checkout() {
            Profiler.BeginSample("ProjectileManager Checkout");

            if (hotCache.Count > 0)
            {
                GameObject obj = hotCache.Dequeue();
                //obj.GetComponent<ProjectileComponent>().Poke();
                Profiler.EndSample();
                return obj;
            }

            if (cache.Count > 0)
            {
                Profiler.EndSample();
                GameObject obj = cache.Dequeue();
                obj.SetActive(true);
                return obj;
            }

            // Allocate a new GO for the cache
            GameObject toReturn = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Projectile"));
            Profiler.EndSample();
            return toReturn;
        }

        public static void Return(GameObject obj)
        {
            Profiler.BeginSample("ProjectileManager Return");
            //obj.transform.position = Vector3.zero;
            //obj.transform.rotation = Quaternion.identity;
            //obj.transform.localScale = Vector3.one;

            // We don't reset the ProjectileComponent- it'll be reset when it's used next time
            //obj.SetActive(false);

            // Add this slot back to the list.
            //cache.Enqueue(obj);
            hotCache.Enqueue(obj);

            Profiler.EndSample();
        }
    }
}
