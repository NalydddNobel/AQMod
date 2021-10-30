using AQMod.Common.Config;
using AQMod.Common.DeveloperTools;
using AQMod.Content.MapMarkers;
using System;
using System.Collections.Generic;

namespace AQMod.Content
{
    public static class AQContent
    {
        public static MapMarkerManager MapMarkers { get; private set; }

        private static List<CachedTask> cachedLoadTasks;

        internal static void addLoadTask(CachedTask task)
        {
            cachedLoadTasks.Add(task);
        }
        private static void invokeTasks()
        {
            if (cachedLoadTasks == null)
                return;
            foreach (var task in cachedLoadTasks)
            {
                try
                {
                    task.Invoke();
                }
                catch (Exception e)
                {
                    var aQMod = AQMod.Instance;
                    aQMod.Logger.Error("An error occured when invoking cached load tasks.");
                    aQMod.Logger.Error(e.Message);
                    aQMod.Logger.Error(e.StackTrace);
                }
            }
        }

        /// <summary>
        /// Ran right before Aequus starts truly loading.
        /// </summary>
        internal static void OnInitNewModInstance()
        {
            cachedLoadTasks = new List<CachedTask>();
        }

        /// <summary>
        /// Ran in <see cref="AQMod.Load"/>.
        /// </summary>
        internal static void OnLoad(AQConfigServer server)
        {
            MapMarkers = new MapMarkerManager();
        }

        /// <summary>
        /// Ran in <see cref="AQMod.Load"/>.
        /// </summary>
        internal static void OnLoadAssets(AQConfigServer server, AQConfigClient client)
        {
        }

        /// <summary>
        /// Ran in <see cref="AQMod.PostSetupContent"/>.
        /// </summary>
        internal static void OnPostSetupContent()
        {
            MapMarkers.Setup(setupStatics: true);
            invokeTasks();
            cachedLoadTasks.Clear();
        }

        /// <summary>
        /// Ran in <see cref="AQMod.AddRecipes"/>.
        /// </summary>
        internal static void OnAddRecipes()
        {
            invokeTasks();
            cachedLoadTasks = null;
        }

        internal static void Unload()
        {
            cachedLoadTasks = null;
            MapMarkers = null;
        }
    }
}