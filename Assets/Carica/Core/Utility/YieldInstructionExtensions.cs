using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Carica.Core.Utility
{
    public static class YieldInstructionExtensions
    {
        public static CustomYieldAwaiter GetAwaiter(this YieldInstruction instruction)
        {
            return new CustomYieldAwaiter(instruction);
        }
    }

    public class CustomYieldAwaiter : INotifyCompletion
    {
        private readonly YieldInstruction _instruction;
        private Action _continuation;

        public CustomYieldAwaiter(YieldInstruction instruction)
        {
            _instruction = instruction;
        }

        public bool IsCompleted { get; private set; } = false;

        public void GetResult() { } 

        public void OnCompleted(Action continuation)
        {
            _continuation = continuation;
            CoroutineRunner.Instance.StartCoroutine(WaitCoroutine());
        }

        private IEnumerator WaitCoroutine()
        {
            yield return _instruction;
            IsCompleted = true;
            _continuation?.Invoke();
        }
    }

    public class CoroutineRunner : MonoBehaviour
    {
        private static CoroutineRunner _instance;
        public static CoroutineRunner Instance
        {
            get
            {
                if (_instance == null)
                {
                    var obj = new GameObject("CoroutineRunner");
                    DontDestroyOnLoad(obj);
                    _instance = obj.AddComponent<CoroutineRunner>();
                }
                return _instance;
            }
        }
    }

}