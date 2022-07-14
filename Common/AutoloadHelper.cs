using System;
using System.Collections.Generic;
using System.Reflection;
using Terraria.ModLoader;

namespace Aequus.Common
{
    internal class AutoloadHelper
    {
        public static List<T> GetAndOrganizeOfType<T>(Assembly ass) where T : class, ILoadable
        {
            var list = new List<T>();
            var loadBeforeDict = new Dictionary<Type, List<T>>();
            var loadAfterDict = new Dictionary<Type, List<T>>();
            var t = GetTypes(ass);
            foreach (var type in t)
            {
                if (TryGetInstanceOf<T>(type, out var instance))
                {
                    if (TryGetInstanceOf<ILoadBefore>(type, out var loadBefore))
                    {
                        if (loadBefore.SortBefore(typeof(T)))
                        {
                            var loadBeforeType = loadBefore.LoadBefore;
                            if (loadBeforeDict.ContainsKey(loadBeforeType))
                            {
                                loadBeforeDict[loadBeforeType].Add(instance);
                            }
                            else
                            {
                                loadBeforeDict.Add(loadBeforeType, new List<T>() { instance });
                            }
                            continue;
                        }
                    }
                    else if (TryGetInstanceOf<ILoadAfter>(type, out var loadAfter))
                    {
                        if (loadAfter.SortAfter(typeof(T)))
                        {
                            var loadAfterType = loadAfter.LoadAfter;
                            if (loadAfterDict.ContainsKey(loadAfterType))
                            {
                                loadAfterDict[loadAfterType].Add(instance);
                            }
                            else
                            {
                                loadAfterDict.Add(loadAfterType, new List<T>() { instance });
                            }
                            continue;
                        }
                    }

                    list.Add(instance);
                }
            }
            foreach (var pair in loadBeforeDict)
            {

                int index = list.FindIndex((instance) => instance.GetType().Equals(pair.Key));
                if (index != -1)
                {
                    foreach (var instance2 in pair.Value)
                    {
                        list.Insert(index, instance2);
                    }
                }
                else
                {
                    foreach (var instance2 in pair.Value)
                    {
                        list.Insert(index, instance2);
                    }
                }
            }
            foreach (var pair in loadAfterDict)
            {
                //Aequus.Instance.Logger.Info("Searching for " + pair.Key.Name);

                int index = list.FindIndex((instance) => instance.GetType().Equals(pair.Key));
                //Aequus.Instance.Logger.Info("Found Index: " + index);
                if (index != -1)
                {
                    foreach (var instance2 in pair.Value)
                    {
                        //Aequus.Instance.Logger.Info("Inserting " + instance2.GetType().FullName);
                        list.Insert(index + 1, instance2);
                    }
                }
                else
                {
                    foreach (var instance2 in pair.Value)
                    {
                        //Aequus.Instance.Logger.Info("Adding " + instance2.GetType().FullName);
                        list.Insert(index + 1, instance2);
                    }
                }
            }
            return list;
        }

        public static Type[] GetTypes(Assembly ass)
        {
            return ass.GetTypes();
        }

        public static bool TryGetInstanceOf<T>(Type t, out T instance) where T : class
        {
            instance = null;
            if (t.IsAbstract || t.IsInterface)
            {
                return false;
            }
            foreach (var i in t.GetInterfaces())
            {
                if (i.IsAssignableTo(typeof(T)))
                {
                    instance = ModContent.GetInstance<T>();
                    if (instance == null)
                    {
                        instance = (T)Activator.CreateInstance(t);
                    }
                    return instance != null;
                }
            }

            return false;
        }

        public static void AutoloadOfType<T>(Assembly ass, Aequus aequus) where T : ILoadable
        {
            var m = typeof(T).GetMethod("CheckAutoload", BindingFlags.Public | BindingFlags.Static);
            foreach (var t in GetTypes(ass))
            {
                if (t.IsAbstract || t.IsInterface)
                {
                    continue;
                }
                m.Invoke(null, new object[] { aequus, t });
            }
        }
    }
    internal interface ILoadBefore
    {
        Type LoadBefore { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="t">The loadable type. EX: <see cref="IOnModLoad"/></param>
        /// <returns></returns>
        bool SortBefore(Type t)
        {
            return true;
        }
    }
    internal interface ILoadAfter
    {
        Type LoadAfter { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="t">The loadable type. EX: <see cref="IOnModLoad"/></param>
        /// <returns></returns>
        bool SortAfter(Type t)
        {
            return true;
        }
    }
    internal interface IOnModLoad : ILoadable
    {
        void OnModLoad(Aequus aequus);
    }
    internal interface IPostSetupContent : ILoadable
    {
        void PostSetupContent(Aequus aequus);
    }
    internal interface IAddRecipes : ILoadable
    {
        void AddRecipes(Aequus aequus);
    }
    internal interface IPostAddRecipes : ILoadable
    {
        void PostAddRecipes(Aequus aequus);
    }
}