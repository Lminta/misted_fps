using Carica.AssetManagement;
using Carica.Core.AssetManagement;
using Carica.Core.UI;
using UnityEngine;
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
        }

        void BindCustomFactories()
        {
            Container.BindInterfacesAndSelfTo<ScreenFactory>().AsSingle().NonLazy();
        }
    }
}