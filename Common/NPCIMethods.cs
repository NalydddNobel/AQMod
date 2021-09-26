using Terraria;

namespace AQMod.Common
{
    public static class NPCIMethods
    {
        internal static void Setup()
        {
            On.Terraria.NPC.Collision_DecideFallThroughPlatforms += NPC_Collision_DecideFallThroughPlatforms;
        }

        private static bool NPC_Collision_DecideFallThroughPlatforms(On.Terraria.NPC.orig_Collision_DecideFallThroughPlatforms orig, NPC self) =>
            self.type > Main.maxNPCTypes && self.modNPC is idecidetofallthroughplatforms decideToFallThroughPlatforms ? decideToFallThroughPlatforms.decide() : orig(self);

        public interface idecidetofallthroughplatforms
        {
            bool decide();
        }
    }
}