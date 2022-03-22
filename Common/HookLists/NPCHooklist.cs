using AQMod.NPCs;
using System.Reflection;
using Terraria;

namespace AQMod.Common.HookLists
{
    public sealed class NPCHooklist : HookList
    {
        [LoadHook(typeof(NPC), "SetDefaults", BindingFlags.Public | BindingFlags.Instance)]
        internal static void PostSetDefaults(On.Terraria.NPC.orig_SetDefaults orig, NPC self, int Type, float scaleOverride)
        {
            orig(self, Type, scaleOverride);
            try
            {
                if (!AQMod.Loading)
                {
                    self.GetGlobalNPC<AQNPC>().PostSetDefaults(self, Type, scaleOverride);
                }
            }
            catch
            {
            }
        }

        [LoadHook(typeof(NPC), "Collision_DecideFallThroughPlatforms", BindingFlags.NonPublic | BindingFlags.Instance)]
        internal static bool DecideFallThroughPlatforms(On.Terraria.NPC.orig_Collision_DecideFallThroughPlatforms orig, NPC self)
        {
            return self.type > Main.maxNPCTypes &&
            self.modNPC is IDecideFallThroughPlatforms decideToFallThroughPlatforms ?
            decideToFallThroughPlatforms.Decide() : orig(self);
        }
    }
}
