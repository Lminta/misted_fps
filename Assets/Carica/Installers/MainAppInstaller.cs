using Carica.Core.Services.AssetManagement;
using Carica.Core.UI;
using Fog.Services.Input;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Carica.Installers
{
    public class MainAppInstaller : MonoInstaller
    {
        [SerializeField] private UIRoot _uiRoot;
        public override void InstallBindings()
        {
            Debug.Log("MainAppInstaller: Installing bindings...");
            BindCustomFactories();
            BindServices();
            BindGOs();
            BindControls();
        }

        private void BindGOs()
        {
            Container.Bind<UIRoot>().FromComponentInNewPrefab(_uiRoot)
                .WithGameObjectName("UIRoot")
                .AsSingle()
                .NonLazy();
        }

        private void BindServices()
        {
            Container.BindInterfacesAndSelfTo<LocalDataProvider>()
                .AsSingle()
                .NonLazy();
            Container.BindInterfacesAndSelfTo<AddressableProvider>()
                .AsSingle()
                .NonLazy();
            Container.BindInterfacesAndSelfTo<UIManager>()
                .AsSingle()
                .NonLazy();
            Container.BindInterfacesAndSelfTo<SceneManager>()
                .AsSingle()
                .NonLazy();
        }

        private void BindControls()
        {
#if UNITY_EDITOR || UNITY_WEBGL
            Container.BindInterfacesAndSelfTo<PCControls>()
                .AsSingle()
                .NonLazy();
#endif
        }

        private void BindCustomFactories()
        {
            Container.BindInterfacesAndSelfTo<ScreenFactory>().AsSingle().NonLazy();
        }
    }
}