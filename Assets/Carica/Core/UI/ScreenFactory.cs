using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Carica.Core.AssetManagement;
using Carica.Core.Configs;
using Carica.Core.Utility.Interfaces;
using UnityEngine;
using Zenject;

namespace Carica.Core.UI
{
    public class ScreenFactory : IInitializableAsync, IDisposable
    {
        private readonly DiContainer _container;
        private readonly IAssetProvider _assetProvider;
        private readonly UIRoot _uiRoot;
        private readonly IStaticDataProvider _staticDataProvider;
        
        private IUIConfig _screenConfig;
        private Dictionary<ScreenID, GameObject> _screenPrefabs = new ();

        public ScreenFactory(DiContainer container, IAssetProvider assetProvider, UIRoot uiRoot,
            IStaticDataProvider staticDataProvider)
        {
            _container = container;
            _assetProvider = assetProvider;
            _uiRoot = uiRoot;
            _staticDataProvider = staticDataProvider;
        }

        public async Task<TController> CreateScreen<TController>(ScreenID id,
            CancellationToken cancellationToken = default)
            where TController : ScreenControllerBase
        {
            if (!_screenConfig.ScreenReference.TryGetValue(id, out var screen))
            {
                Debug.LogFormat("Screen with id {0} not found in screen config", id);
            }

            if (!_screenPrefabs.TryGetValue(id, out var prefab))
            {
                prefab = await LoadScreen(id, CancellationToken.None);
                _screenPrefabs.Add(id, prefab);
            }

            var controller = _container.InstantiatePrefab(prefab, _uiRoot.MainCanvasRoot).GetComponent<TController>();
            return controller;
        }

        public async Task InitializeAsync(CancellationToken cancellationToken = default)
        {
            _screenConfig = _staticDataProvider.UIConfig;
            if (!_screenConfig.LazyLoad)
            {
                await LoadScreens(cancellationToken);
            }
        }

        private Task LoadScreens(CancellationToken cancellationToken)
        {
            var screenList = _screenConfig.ScreenReference.Keys;
            var tasks = screenList.Select(screen => LoadScreen(screen, cancellationToken));
            return Task.WhenAll(tasks);
        }

        private async Task<GameObject> LoadScreen(ScreenID id, CancellationToken cancellationToken = default)
        {
            if(!_screenConfig.ScreenReference.TryGetValue(id, out var screen))
            {
                Debug.LogFormat("Screen with id {0} not found in screen config", id);
            }
            
            var screenPrefab = await _assetProvider.Load<GameObject>(screen, cancellationToken);
            _screenPrefabs.Add(id, screenPrefab);
            return screenPrefab;
        }

        public void Dispose()
        {
            foreach (var id in _screenPrefabs.Keys)
            {
                if (_screenConfig.ScreenReference.TryGetValue(id, out var screen))
                    _assetProvider.Release(screen);
            }
        }
    }
}