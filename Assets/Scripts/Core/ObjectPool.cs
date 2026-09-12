using System.Collections.Generic;
using UnityEngine;

namespace Anubis.Core
{
    public sealed class ObjectPool : MonoBehaviour
    {
        public static ObjectPool Current { get; private set; }

        readonly Dictionary<string, Queue<GameObject>> _pools = new();
        readonly Dictionary<GameObject, string> _instanceKeys = new();
        readonly Dictionary<string, GameObject> _prefabs = new();
        Transform _root;

        public void Initialize()
        {
            Current = this;
            if (_root != null)
            {
                return;
            }

            var rootObject = new GameObject("ObjectPool");
            rootObject.transform.SetParent(transform, false);
            _root = rootObject.transform;
        }

        public void RegisterPrefab(string key, GameObject prefab, int prewarm)
        {
            if (string.IsNullOrEmpty(key) || prefab == null)
            {
                return;
            }

            _prefabs[key] = prefab;
            if (!_pools.ContainsKey(key))
            {
                _pools[key] = new Queue<GameObject>();
            }

            for (var i = 0; i < prewarm; i++)
            {
                var instance = CreateInstance(key);
                Return(instance);
            }
        }

        public GameObject Get(string key, Vector3 position, Quaternion rotation)
        {
            if (!_pools.TryGetValue(key, out var queue))
            {
                queue = new Queue<GameObject>();
                _pools[key] = queue;
            }

            GameObject instance = queue.Count > 0 ? queue.Dequeue() : CreateInstance(key);
            instance.transform.SetPositionAndRotation(position, rotation);
            instance.SetActive(true);

            if (instance.TryGetComponent<IPoolable>(out var poolable))
            {
                poolable.OnSpawnFromPool();
            }

            return instance;
        }

        public T Get<T>(string key, Vector3 position, Quaternion rotation) where T : Component
        {
            return Get(key, position, rotation).GetComponent<T>();
        }

        public void Return(GameObject instance)
        {
            if (instance == null)
            {
                return;
            }

            if (instance.TryGetComponent<IPoolable>(out var poolable))
            {
                poolable.OnReturnToPool();
            }

            instance.SetActive(false);
            instance.transform.SetParent(_root, false);

            if (_instanceKeys.TryGetValue(instance, out var key))
            {
                _pools[key].Enqueue(instance);
            }
        }

        GameObject CreateInstance(string key)
        {
            if (!_prefabs.TryGetValue(key, out var prefab))
            {
                throw new KeyNotFoundException($"Pool prefab not registered: {key}");
            }

            var instance = Instantiate(prefab, _root);
            instance.name = $"{key}_Pooled";
            _instanceKeys[instance] = key;
            instance.SetActive(false);
            return instance;
        }
    }
}
