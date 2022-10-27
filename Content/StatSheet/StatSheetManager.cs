using System.Collections.Generic;
using Terraria.ModLoader;

namespace Aequus.Content.StatSheet
{
    public class StatSheetManager : ILoadable
    {
        public class Hooks
        {
            public readonly List<IStatTracker> Player_PostUpdate;
            public readonly List<IStatTracker> Player_OnHitVictim;

            public Hooks()
            {
                Player_PostUpdate = new List<IStatTracker>();
                Player_OnHitVictim = new List<IStatTracker>();
            }

            public void UpdateHooklist(IStatUpdateInfo info, List<IStatTracker> hookList)
            {
                foreach (var hook in hookList)
                {
                    hook.Update(info);
                }
            }
        }

        public static Hooks StatHooks;
        public static List<IStatTracker> RegisteredStats { get; private set; }

        void ILoadable.Load(Mod mod)
        {
            StatHooks = new Hooks();
            RegisteredStats = new List<IStatTracker>();
        }

        void ILoadable.Unload()
        {
            StatHooks = null;
            RegisteredStats?.Clear();
        }
    }
}
