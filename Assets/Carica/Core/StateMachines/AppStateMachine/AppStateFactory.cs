using Zenject;

namespace Carica.Core.StateMachines.AppStateMachine
{
    public class AppStateFactory
    {
        private readonly DiContainer _container;

        public AppStateFactory(DiContainer container) =>
            _container = container;

        public TState CreateState<TState>() where TState : IInternalAppState =>
            _container.Resolve<TState>();
    }
}
