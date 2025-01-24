using System;
using System.Threading;
using System.Threading.Tasks;
using Carica.Core.AssetManagement;
using UnityEngine;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace Carica.Core.SceneManagement
{
    public class SceneLoader
    {
        private readonly IAssetProvider _assetProvider;
        private SceneInstance _currentScene;

        public SceneLoader(IAssetProvider assetProvider)
        {
            _assetProvider = assetProvider;
        }

        public async Task<SceneInstance> LoadScene(string sceneName, CancellationToken cancellationToken = default)
        {
            var scene = await _assetProvider.LoadScene(sceneName, LoadSceneMode.Additive, cancellationToken);
            await UnloadScene(cancellationToken);
            await scene.ActivateAsync();
            _currentScene = scene;
            
            return scene;
        }
        
        public Task UnloadScene(CancellationToken cancellationToken = default)
        {
            return _assetProvider.UnloadScene(_currentScene, cancellationToken);
        }
    }
}