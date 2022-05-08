using Aequus.Common.Catalogues;
using Aequus.Common.Networking;
using Aequus.NPCs;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Buffs.Debuffs
{
    public class NecromancyDebuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            NecromancyTypes.NecromancyDebuffs.Add(Type);
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.GetGlobalNPC<NecromancyNPC>().zombieDrain = 5 * AequusHelpers.NPCREGEN;
        }

        public static void ApplyDebuff<T>(NPC npc, int time, int player, float tier) where T : NecromancyDebuff
        {
            bool cheat = tier >= 100;
            if (cheat)
            {
                npc.buffImmune[ModContent.BuffType<T>()] = false;
            }
            if (cheat || (NecromancyTypes.TryGetByNetID(npc, out var value) && value.PowerNeeded <= tier))
            {
                npc.AddBuff(ModContent.BuffType<T>(), time);
                npc.GetGlobalNPC<NecromancyNPC>().zombieOwner = player;
                npc.GetGlobalNPC<NecromancyNPC>().zombieDebuffTier = tier;
                if (Main.netMode == NetmodeID.MultiplayerClient)
                {
                    PacketSender.SyncNecromancyOwnerTier(npc.whoAmI, player, tier);
                }
            }
        }
    }
}