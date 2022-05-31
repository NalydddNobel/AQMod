using Aequus.Common.Networking;
using Aequus.Content.Necromancy;
using Aequus.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Buffs.Debuffs.Necro
{
    public class NecromancyDebuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            NecromancyDatabase.NecromancyDebuffs.Add(Type);
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            var zombie = npc.GetGlobalNPC<NecromancyNPC>();
            zombie.zombieDrain = 2 * AequusHelpers.NPCREGEN;

            if (zombie.renderLayer < NecromancyScreenRenderer.TargetIDs.FriendlyZombie)
            {
                zombie.renderLayer = NecromancyScreenRenderer.TargetIDs.FriendlyZombie;
            }
        }

        public static void ApplyDebuff<T>(NPC npc, int time, int player, float tier) where T : NecromancyDebuff
        {
            bool cheat = tier >= 100;
            if (cheat)
            {
                npc.buffImmune[ModContent.BuffType<T>()] = false;
            }
            if (cheat || (NecromancyDatabase.TryGetByNetID(npc, out var value) && value.PowerNeeded <= tier))
            {
                npc.AddBuff(ModContent.BuffType<T>(), time);
                npc.GetGlobalNPC<NecromancyNPC>().zombieOwner = player;
                npc.GetGlobalNPC<NecromancyNPC>().zombieDebuffTier = tier;
                if (Main.netMode == NetmodeID.MultiplayerClient)
                {
                    PacketHandler.SyncNecromancyOwnerTier(npc.whoAmI, player, tier);
                }
            }
        }
    }
}