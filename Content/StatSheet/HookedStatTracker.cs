using System;
using System.Collections.Generic;
using Terraria.ModLoader.IO;

namespace Aequus.Content.StatSheet
{
    public class HookedStatTracker<T> : IStatTracker
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }

        public T stat;
        public Func<IStatUpdateInfo, HookedStatTracker<T>, T> OnUpdate;

        public HookedStatTracker(string name, Func<IStatUpdateInfo, HookedStatTracker<T>, T> updateStat, string key = null)
        {
            Name = name;
            DisplayName = key ?? name;
            OnUpdate = updateStat;
        }

        public static HookedStatTracker<T> Register(List<IStatTracker> hookList, Func<IStatUpdateInfo, HookedStatTracker<T>, T> hookFunction, string name, string key = null)
        {
            var stat = new HookedStatTracker<T>(name, hookFunction, key);
            StatSheetManager.RegisteredStats.Add(stat);
            hookList.Add(stat);
            return stat;
        }

        public string ProvideStatText()
        {
            return stat == null ? "N/A" : stat.ToString();
        }

        public void Update(IStatUpdateInfo info)
        {
            stat = OnUpdate.Invoke(info, this);
        }

        public object ProvideSaveData()
        {
            return stat;
        }

        public void LoadSaveData(TagCompound tag)
        {
            if (tag.TryGet<T>(Name, out var val))
                stat = val;
        }
    }
}
