using Aequus.Buffs;
using System;
using System.Collections.Generic;
using System.Reflection;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Content.CrossMod
{
    internal class ThoriumMod : ModSupport<ThoriumMod>
    {
        private static Type thoriumCacheHandler;

        public override void SafeLoad(Mod mod)
        {
            thoriumCacheHandler = Instance.Code.GetType("ThoriumMod.ThoriumCacheHandler");
        }

        public override void AddRecipes()
        {
            if (thoriumCacheHandler != null)
            {
                TryAddingCurePatch();
            }
        }

        public static void TryAddingCurePatch()
        {
            if (thoriumCacheHandler.TryGetFieldOf<HashSet<int>>("PlayerDoTBuff",
                BindingFlags.NonPublic | BindingFlags.Static, null, out var playerDoTBuff))
            {
                foreach (var b in AequusBuff.PlayerDoTBuff)
                {
                    if (b >= Main.maxBuffTypes && BuffLoader.GetBuff(b).Mod.Name == "Aequus")
                    {
                        playerDoTBuff.Add(b);
                    }
                }
            }
            if (thoriumCacheHandler.TryGetFieldOf<HashSet<int>>("PlayerStatusBuff",
                BindingFlags.NonPublic | BindingFlags.Static, null, out var playerStatusBuff))
            {
                foreach (var b in AequusBuff.PlayerStatusBuff)
                {
                    if (b >= Main.maxBuffTypes && BuffLoader.GetBuff(b).Mod.Name == "Aequus")
                    {
                        playerStatusBuff.Add(b);
                    }
                }
            }
        }

        public override void SafeUnload()
        {
            thoriumCacheHandler = null;
        }
    }
}