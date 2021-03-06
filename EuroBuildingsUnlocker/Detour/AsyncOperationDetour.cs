﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using EuroBuildingsUnlocker.Redirection;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EuroBuildingsUnlocker.Detour
{
    [TargetType(typeof(AsyncOperation))]
    public class AsyncOperationDetour : AsyncOperation
    {
        public static AsyncOperation nativelevelOperation = null;
        private static AsyncOperation nativeLevelOperation_addition = null;
        private static readonly object Lock = new object();
        private static RedirectCallsState _tempState;
        public static Queue<string> additionalLevels = new Queue<string>();


        private static Dictionary<MethodInfo, RedirectCallsState> _redirects;

        public static void Deploy()
        {
            if (_redirects != null)
            {
                return;
            }
            _redirects = RedirectionUtil.RedirectType(typeof(AsyncOperationDetour));
        }
        public static void Revert()
        {
            if (_redirects == null)
            {
                return;
            }
            foreach (var redirect in _redirects)
            {
                RedirectionHelper.RevertRedirect(redirect.Key, redirect.Value);
            }
            _redirects = null;
        }


        private static void RevertTemp()
        {
            if (_redirects == null)
            {
                return;
            }
            foreach (var redirect in _redirects)
            {
                _tempState = RedirectionHelper.RevertJumpTo(redirect.Key.MethodHandle.GetFunctionPointer(), redirect.Value);
                break;
            }
        }

        private static void DeployBack()
        {
            if (_redirects == null)
            {
                return;
            }
            foreach (var redirect in _redirects)
            {
                RedirectionHelper.RevertJumpTo(redirect.Key.MethodHandle.GetFunctionPointer(), _tempState);
                break;
            }
        }


        [RedirectMethod]
        public bool get_isDone()
        {
            Monitor.Enter(Lock);
            try
            {
                bool result;
                RevertTemp();
                if (this != nativelevelOperation)
                {
                    result = isDone;
                }
                else
                {
                    if (nativelevelOperation.isDone)
                    {
                        if (nativeLevelOperation_addition == null || nativeLevelOperation_addition.isDone)
                        {

                            if (additionalLevels.Count > 0)
                            {
                                var additionalLevel = additionalLevels.Dequeue();
                                if (EuroBuildingsUnlocker.debug)
                                {
                                    Debug.Log($"EuroBuildingsUnlocker - AsyncOperationDetour - Loading level '{additionalLevel}'");
                                }
                                nativeLevelOperation_addition = SceneManager.LoadSceneAsync(additionalLevel, LoadSceneMode.Additive);
                                result = false;
                            }
                            else
                            {
                                nativeLevelOperation_addition = null;
                                nativelevelOperation = null;
                                result = true;
                            }
                        }
                        else
                        {
                            result = false;
                        }
                    }
                    else
                    {
                        result = false;
                    }
                }
                return result;
            }
            finally
            {
                DeployBack();
                Monitor.Exit(Lock);
            }
        }
    }
}