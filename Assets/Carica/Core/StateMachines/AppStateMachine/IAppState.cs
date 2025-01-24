namespace Carica.Core.StateMachines.AppStateMachine
{
    public interface IInternalAppState
    {
        
    }
    
    public interface IAppState<TPayload> : IInternalAppState
    {
        void Enter(TPayload payload);
    }
    
    public interface IAppState : IInternalAppState
    {
        void Enter();
    }
    
    public interface IExitableAppState : IInternalAppState
    {
        void Exit();
    }
}
