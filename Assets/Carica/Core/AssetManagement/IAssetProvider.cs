using System.Threading;
using System.Threading.Tasks;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace Carica.Core.AssetManagement
{
    public interface IAssetProvider
    {
        public Task<T> Load<T>(string key, CancellationToken cancellationToken = default) where T : class;
        public void Release(string key);

        public Task<SceneInstance> LoadScene(string sceneName, LoadSceneMode loadMode,
            CancellationToken cancellationToken = default);
        public Task UnloadScene(SceneInstance scene, CancellationToken cancellationToken = default);
        
        public void Cleanup();
    }
}