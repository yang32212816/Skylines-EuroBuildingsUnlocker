﻿using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using ColossalFramework;
using EuroBuildingsUnlocker.Redirection;

namespace EuroBuildingsUnlocker.Detour
{
    //TODO(earalov): make sure no overlaps between biomes
    [TargetType(typeof(CitizenCollection))]
    public class CitizenCollectionDetour : CitizenCollection
    {

        private string GameObjectName => gameObject?.name;
        private string ParentName => gameObject?.transform?.parent?.gameObject?.name;

        [RedirectMethod]
        private void Awake()
        {
            //TODO(earalov): fix this later
            //            if (ParentName == Constants.TropicalCollections || ParentName == Constants.SunnyCollections ||
            //                ParentName == Constants.NorthCollections || ParentName == Constants.WinterCollections)
            //            {
            //                if (EuroBuildingsUnlocker._nativeLevelName == Constants.EuropeLevel)
            //                {
            //                    if (GameObjectName != Constants.TropicalBeautification || GameObjectName != Constants.SunnyBeautification)
            //                    {
            //                        Destroy(this);
            //                        return;
            //                    }
            //                }
            //            }
            //            else if (ParentName == Constants.EuropeCollections)
            //            {
            //                if (EuroBuildingsUnlocker._nativeLevelName != Constants.EuropeLevel) {
            //                    if (GameObjectName != Constants.EuropeBeautification)
            //                    {
            //                        Destroy(this);
            //                        return;
            //                    }
            //                    if (EuroBuildingsUnlocker._nativeLevelName == Constants.NorthLevel)
            //                    {
            //                        Destroy(this);
            //                        return;
            //                    }
            //                }
            //            }
            if (this.IsIgnored())
            {
                Destroy(this);
                return;
            }
            Singleton<LoadingManager>.instance.QueueLoadingAction(InitializePrefabs(this.gameObject.name, this.m_prefabs, this.m_replacedNames));
        }

        [RedirectReverse]
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static IEnumerator InitializePrefabs(string name, CitizenInfo[] prefabs, string[] replaces)
        {
            UnityEngine.Debug.Log($"{name}-{prefabs}-{(replaces == null ? "Null" : "Nonnull")}");
            return null;
        }
    }
}