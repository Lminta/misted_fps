using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Carica.Core.StateMachines.AppStateMachine
{
    public class AppStateMachine : IInitializable
    {
        private readonly AppStateFactory _stateFactory;
        private readonly Dictionary<Type, IInternalAppState> _states = new ();
        private IExitableAppState _currentState;
        
        public AppStateMachine(AppStateFactory stateFactory)
        {
            _stateFactory = stateFactory;
        }

        public void Initialize()
        {
            
        }
        
        public void Enter<TState>() where TState : IAppState
        {
            var state = ChangeState<TState>();
            state.Enter();
        }
        
        public void Enter<TState, TPayload>(TPayload payload) where TState : IAppState<TPayload>
        {
            var state = ChangeState<TState>();
            state.Enter(payload);
        }
        
        private TState ChangeState<TState>() where TState : IInternalAppState
        {
            _currentState?.Exit();

            if (!_states.TryGetValue(typeof(TState), out var state))
            {
                state = _stateFactory.CreateState<TState>();
                _states.Add(typeof(TState), state);
            }

            _currentState = state is IExitableAppState exitableState ? exitableState : null; 
            Debug.LogFormat("Changed state to {0}", typeof(TState).Name);
            return (TState)state;
        }
    }
}