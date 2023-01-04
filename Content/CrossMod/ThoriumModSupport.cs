using Aequus.Buffs;
using Aequus.Common;
using System;
using System.Collections.Generic;
using System.Reflection;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Content.CrossMod
{
    public class ThoriumModSupport : ModSystem
    {
        public static Mod ThoriumMod { get; private set; }

        private static Type thoriumCacheHandler;

        public override void Load()
        {
            ThoriumMod = null;
            if (ModLoader.TryGetMod("ThoriumMod", out var thoriumMod))
            {
                ThoriumMod = thoriumMod;
                thoriumCacheHandler = ThoriumMod.Code.GetType("ThoriumMod.ThoriumCacheHandler");
            }
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

        public override void Unload()
        {
            thoriumCacheHandler = null;
            ThoriumMod = null;
        }
    }
}