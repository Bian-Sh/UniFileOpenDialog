// Copyright (c) https://github.com/Bian-Sh/Loom
// Licensed under the MIT License.

using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Threading;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.LowLevel;
namespace zFramework.Internal
{
    public static class Loom
    {
        static int mainThreadId;
        public static bool IsMainThread => Thread.CurrentThread.ManagedThreadId == mainThreadId;
        static readonly ConcurrentQueue<Action> tasks = new();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Install()
        {
            mainThreadId = Thread.CurrentThread.ManagedThreadId;
            #region ʹ�� PlayerLoop �� Unity ���̵߳� Update �и��±�����ͬ����
            // Ϊ�� ref �� ref
            static ref PlayerLoopSystem FindSubSystem(PlayerLoopSystem root, Predicate<PlayerLoopSystem> predicate)
            {
                for (int i = 0; i < root.subSystemList.Length; i++)
                {
                    if (predicate.Invoke(root.subSystemList[i]))
                    {
                        // ���Թ�ע ref ��� return ���÷�����������ֱ���޸� root.subSystemList[i] ��ֵ
                        // https://learn.microsoft.com/zh-cn/dotnet/csharp/language-reference/keywords/ref#ref-returns
                        return ref root.subSystemList[i];
                    }
                }
                throw new Exception("Not Found!");
            }
            var rootLoopSystem = PlayerLoop.GetCurrentPlayerLoop();
            ref var sub_pls = ref FindSubSystem(rootLoopSystem, v => v.type == typeof(UnityEngine.PlayerLoop.Update));
            Array.Resize(ref sub_pls.subSystemList, sub_pls.subSystemList.Length + 1);
            sub_pls.subSystemList[^1] = new PlayerLoopSystem { type = typeof(Loom), updateDelegate = Update };
            PlayerLoop.SetPlayerLoop(rootLoopSystem);

#if UNITY_EDITOR
            //��Ϊ�༭��ֹͣ���ź��������� loopsystem ���ɻᴥ����������˳�Play ģʽ����� tasks
            EditorApplication.playModeStateChanged -= EditorApplication_playModeStateChanged;
            EditorApplication.playModeStateChanged += EditorApplication_playModeStateChanged;
            static void EditorApplication_playModeStateChanged(PlayModeStateChange obj)
            {
                if (obj == PlayModeStateChange.ExitingEditMode || obj == PlayModeStateChange.ExitingPlayMode)
                {
                    while (tasks.TryDequeue(out _)) { }//��������б�
                }
            }
#endif
            #endregion
        }

#if UNITY_EDITOR
        // ȷ���༭�������͵��¼�Ҳ�ܱ�ִ��
        [InitializeOnLoadMethod]
        static void EditorForceUpdate()
        {
            Install();
            EditorApplication.update -= ForceEditorPlayerLoopUpdate;
            EditorApplication.update += ForceEditorPlayerLoopUpdate;
            static void ForceEditorPlayerLoopUpdate()
            {
                if (EditorApplication.isPlayingOrWillChangePlaymode || EditorApplication.isCompiling || EditorApplication.isUpdating)
                {
                    return;   // Not in Edit mode, don't interfere
                }
                Update();
            }
        }
#endif

        /// <summary>
        ///  �����߳���ִ��
        /// </summary>
        /// <param name="task">Ҫִ�е�ί��</param>
        public static void Post(Action task)
        {
            if (IsMainThread)
            {
                task?.Invoke();
            }
            else
            {
                tasks.Enqueue(task);
            }
        }

        static void Update()
        {
            while (tasks.TryDequeue(out var task))
            {
                try
                {
                    task?.Invoke();
                }
                catch (Exception e)
                {
                    Debug.Log($"{nameof(Loom)}:  ���͵�����ִ�й����з����쳣����ȷ��: {e}");
                }
            }
        }
        /// <summary>
        /// �л������߳���ִ��
        /// </summary>
        public static SwitchToUnityThreadAwaitable ToMainThread => new();
        /// <summary>
        /// �л����̳߳���ִ��
        /// </summary>
        public static SwitchToThreadPoolAwaitable ToOtherThread => new();
        public struct SwitchToUnityThreadAwaitable
        {
            public Awaiter GetAwaiter() => new();
            public struct Awaiter : INotifyCompletion
            {
                public bool IsCompleted => IsMainThread;
                public void GetResult() { }
                public void OnCompleted(Action continuation) => Post(continuation);
            }
        }
        public struct SwitchToThreadPoolAwaitable
        {
            public Awaiter GetAwaiter() => new();
            public struct Awaiter : ICriticalNotifyCompletion
            {
                static readonly WaitCallback switchToCallback = state => ((Action)state).Invoke();
                public bool IsCompleted => false;
                public void GetResult() { }
                public void OnCompleted(Action continuation) => ThreadPool.UnsafeQueueUserWorkItem(switchToCallback, continuation);
                public void UnsafeOnCompleted(Action continuation) => ThreadPool.UnsafeQueueUserWorkItem(switchToCallback, continuation);
            }
        }
    }
}