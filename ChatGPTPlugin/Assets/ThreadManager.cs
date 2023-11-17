using System;
using System.Collections.Generic;
using UnityEngine;

namespace GPTPlugin
{
    public class ThreadManager : MonoBehaviour
    {
        private static readonly List<Action> m_executeOnMainThread = new List<Action>();
        private static readonly List<Action> m_executeCopiedOnMainThread = new List<Action>();
        private static bool m_actionToExecuteOnMainThread = false;

        void Update()
        {
            UpdateMain();
        }

        public static void ExecuteOnMainThread(Action _action)
        {
            if (_action == null)
            {
                Debug.Log("[ThreadManager:ExecuteOnMainThread] ExecuteOnMainThread called without delegate");
                return;
            }

            lock (m_executeOnMainThread)
            {
                m_executeOnMainThread.Add(_action);
                m_actionToExecuteOnMainThread = true;
            }
        }

        public static void UpdateMain()
        {
            if (m_actionToExecuteOnMainThread)
            {
                m_executeCopiedOnMainThread.Clear();
                lock (m_executeOnMainThread)
                {
                    m_executeCopiedOnMainThread.AddRange(m_executeOnMainThread);
                    m_executeOnMainThread.Clear();
                    m_actionToExecuteOnMainThread = false;
                }

                for (int i = 0; i < m_executeCopiedOnMainThread.Count; i++)
                {
                    try
                    {
                        m_executeCopiedOnMainThread[i]();
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"[ThreadManager:UpdateMain] Error while calling delegate function: {ex}");
                    }
                }
            }
        }
    }
}