using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Carica.Core.Utility.Interfaces;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace Carica.Core.AssetManagement
{
    public class AddressableProvider : IAssetProvider, IInitializableAsync
    {
        private readonly Dictionary<string, AsyncOperationHandle> _cache = new ();
        private readonly ConcurrentDictionary<string, int> _counters = new ();
        private Task _initializationTask = null;
        
        public Task InitializeAsync(CancellationToken cancellationToken = default)
        {
            _initializationTask = Addressables.InitializeAsync().Task;
            return _initializationTask;
        }
        
        public async Task<T> Load<T>(string key, CancellationToken cancellationToken = default) where T : class
        {
            await CheckInitialization(); 
            cancellationToken.ThrowIfCancellationRequested();
       
            _counters.AddOrUpdate(key, 1, (_, v) => v + 1);
            if (_cache.TryGetValue(key, out var resultHandle))
            {
                return resultHandle.Task is Task<T> taskResult ? await taskResult : null;
            }

            var handle = Addressables.LoadAssetAsync<T>(key);
            _cache[key] = handle;
            return await handle.Task;
        }

        public async Task<SceneInstance> LoadScene(string sceneName, LoadSceneMode loadMode,
            CancellationToken cancellationToken = default)
        {
            await CheckInitialization();
            cancellationToken.ThrowIfCancellationRequested();

            var operationHandle = Addressables.LoadSceneAsync(sceneName, loadMode, false);
            return await operationHandle.Task;
        }

        public Task UnloadScene(SceneInstance scene, CancellationToken cancellationToken = default)
        {
            return Addressables.UnloadSceneAsync(scene).Task;
        }

        public void Release(string key)
        {
            if (!_counters.TryGetValue(key, out var counter))
            {
                return;
            }

            switch (counter)
            {
                case 0:
                    return;
                case 1:
                    _counters.AddOrUpdate(key, counter - 1, (_, v) => v - 1);
                    if (_cache.TryGetValue(key, out var handle))
                    {
                        Addressables.Release(handle);
                        _cache.Remove(key);
                    }
                    break;
                case > 1:
                    _counters.AddOrUpdate(key, counter - 1, (_, v) => v - 1);
                    break;
                default:
                    _counters.TryRemove(key, out _);
                    if (_cache.TryGetValue(key, out handle))
                    {
                        Addressables.Release(handle);
                        _cache.Remove(key);
                    }
                    break;
            }
        }

        public void Cleanup()
        {
            foreach (var handle in _cache.Values)
            {
                Addressables.Release(handle);
            }
            _cache.Clear();
            _counters.Clear();
        }
        
        private Task CheckInitialization()
        {
            if (_initializationTask == null)
            {
                throw new System.Exception("AddressableProvider is not initialized");
            }
            
            return _initializationTask;
        }
    }
}