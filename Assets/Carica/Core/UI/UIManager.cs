using System.Threading;
using System.Threading.Tasks;
using Carica.Core.AssetManagement;
using UnityEngine;
using Zenject;

namespace Carica.Core.UI
{
    public class UIManager
    {
        private static class ScreenContainer<TScreen> where TScreen : ScreenControllerBase
        {
            public static TScreen Screen = null;
        }
        
        private ScreenFactory _screenFactory;
        private ScreenControllerBase _currentScreen = null;
        private IStaticDataProvider _dataProvider;

        [Inject]
        private void Construct(ScreenFactory screenFactory, IStaticDataProvider dataProvider)
        {
            _screenFactory = screenFactory;
            _dataProvider = dataProvider;
        }

        public async Task OpenScreen<TScreen, TPayload>(ScreenID screenID, TPayload payload,
            CancellationToken cancellationToken = default)
            where TScreen : ScreenControllerBase, TPayload where TPayload : IScreenPayload
        {
            var screen = _dataProvider.UIConfig.ReUse ? ScreenContainer<TScreen>.Screen : null;
            screen ??= await _screenFactory.CreateScreen<TScreen>(screenID, cancellationToken);
            
            _currentScreen?.Close();
            if (_dataProvider.UIConfig.ReUse)
            {
                ScreenContainer<TScreen>.Screen = screen;
            }
            else
            {
                GameObject.Destroy(_currentScreen);
            }
            
            screen.Setup(payload);
            _currentScreen = screen;
            screen.Open();
        }
    }
}