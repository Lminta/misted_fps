using System.Collections.Generic;
using System.Threading.Tasks;
using Carica.Core.StateMachines.AppStateMachine;
using Carica.Core.Utility.Interfaces;
using Carica.Core.Utility;
using Zenject;

namespace Carica.AppStates
{
    public class AppInitializationState : IAppState
    {
        private readonly List<IInitializableAsync> _services;  
        private readonly AppStateMachine _appStateMachine;
        
        
        public AppInitializationState(AppStateMachine appStateMachine, List<IInitializableAsync> services)
        {
            _services = services;
            _appStateMachine = appStateMachine;
        }
        
        public void Enter()
        {
            InitializeServices().OnMainThread(_appStateMachine.Enter<AppInitializationState>).CheckResult();
        }
        
        private async Task InitializeServices()
        {
            foreach (var service in _services)
            {
                await service.InitializeAsync();
            }
            
            _appStateMachine.Enter<AppMainMenuState>();
        }
    }
}