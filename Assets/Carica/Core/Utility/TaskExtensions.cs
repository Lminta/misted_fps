using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Carica.Core.Utility
{
    public class UnityTaskSync
    {
        public static TaskScheduler UnityTaskScheduler { get; }
        public static int UnityThreadId { get; }
        public static SynchronizationContext UnitySynchronizationContext { get; }

#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
#endif
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Initialize()
        {
            // force initialization
            if (Thread.CurrentThread.ManagedThreadId == UnityThreadId)
            {
                Debug.Log("Unity thread context initialized");
            }
        }

        static UnityTaskSync()
        {
            UnityTaskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
            UnityThreadId = Thread.CurrentThread.ManagedThreadId;
            UnitySynchronizationContext = SynchronizationContext.Current;
        }
    }

    public static class TaskExtensions
    {
        public static bool IsSuccess(this Task task)
        {
            switch (task.Status)
            {
                case TaskStatus.Faulted:
                {
                    if (task.Exception != null)
                    {
                        foreach (var ex in task.Exception.InnerExceptions)
                        {
                            Debug.LogException(ex);
                        }
                    }

                    return false;
                }
                case TaskStatus.Canceled:
                    Debug.LogWarning("Task was canceled");
                    return false;
                default:
                    return true;
            }
        }

        public static Task ProcessErrors(this Task task)
        {
            return task.ContinueWith(t => Debug.LogException(t.Exception), TaskContinuationOptions.OnlyOnFaulted);
        }

        static TResult ResultOrDefault<TResult>(this Task<TResult> task)
        {
            if (task.IsSuccess())
            {
                return task.Result;
            }

            return default;
        }
        
        public static async Task OnMainThread(this Task task, Action action)
        {
            await task;
            if (Thread.CurrentThread.ManagedThreadId != UnityTaskSync.UnityThreadId)
                await new WaitForEndOfFrame();
            
            task.IsSuccess();
            action();
        }

        public static async Task OnMainThread<TResult>(this Task<TResult> task, Action<Task<TResult>> action)
        {
            await task;
            if (Thread.CurrentThread.ManagedThreadId != UnityTaskSync.UnityThreadId)
                await new WaitForEndOfFrame();
            
            action(task);
        }
        
        public static async void CheckResult(this Task task)
        {
            await task;
            if (Thread.CurrentThread.ManagedThreadId != UnityTaskSync.UnityThreadId)
                await new WaitForEndOfFrame();

            task.IsSuccess();
        }
    }
}