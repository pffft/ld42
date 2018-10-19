using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Profiling;

namespace Projectiles
{
    public class ProjectileManager : MonoBehaviour
    {

        public enum CullingAggression {
            LOW,
            MEDIUM,
            HIGH
        }

        private static GameObject[] cache;
        private static Queue<int> openSlots;

        public static CullingAggression cullingAggression;

        public void Start() {
            Profiler.BeginSample("Initialize Projectile Cache");
            cache = new GameObject[10000];
            openSlots = new Queue<int>();
            for (int i = 0; i < cache.Length; i++)
            {
                cache[i] = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Projectile"));
                cache[i].SetActive(false);
                openSlots.Enqueue(i);
            }
            cullingAggression = CullingAggression.LOW;
            Profiler.EndSample();
        }

        private static void Expand() {
            GameObject[] newCache = new GameObject[cache.Length * 2];

            for (int i = 0; i < cache.Length; i++) {
                newCache[i] = cache[i];
            }

            for (int i = cache.Length; i < newCache.Length; i++)
            {
                newCache[i] = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Projectile"));
                openSlots.Enqueue(i);
            }

            cache = newCache;
            Debug.Log("Expanded to size: " + newCache.Length);
        }

        public static int NextSlot() {
            if (openSlots.Count == 0) 
            {
                //Expand();
                Debug.LogError("Ran out of space in the cache!");
                return -1;
            }

            int nextIndex = openSlots.Dequeue();

            if (openSlots.Count >= 9500)
            {
                cullingAggression = CullingAggression.LOW;
            }
            else if (openSlots.Count < 9500 && openSlots.Count >= 9000)
            {
                cullingAggression = CullingAggression.MEDIUM;
            }
            else
            {
                cullingAggression = CullingAggression.HIGH;
            }

            return nextIndex;
        }

        public static GameObject Checkout(int index) {
            if (index == -1) 
            {
                return GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Projectile"));
            }
            GameObject obj = cache[index];
            obj.SetActive(true);
            return obj;
        }

        public static void Return(int index, GameObject toReturn) {
            if (index == -1) 
            {
                GameObject.Destroy(toReturn);
            }

            GameObject obj = cache[index];

            obj.transform.position = Vector3.zero;
            obj.transform.rotation = Quaternion.identity;
            obj.transform.localScale = Vector3.one;

            // We don't reset the ProjectileComponent- it'll be reset when it's used next time
            //obj.GetComponent<ProjectileComponent>

            obj.SetActive(false);

            // Add this slot back to the list.
            openSlots.Enqueue(index);
        }
    }
}
