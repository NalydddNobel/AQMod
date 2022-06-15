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
        public override string Texture => Aequus.Debuff;

        public virtual float Tier => 1f;

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
            zombie.DebuffTier(Tier);
            zombie.RenderLayer(GhostOutlineTarget.IDs.Zombie);
        }

        public static void ApplyDebuff<T>(NPC npc, int time, int player) where T : NecromancyDebuff
        {
            float tier = ModContent.GetInstance<T>().Tier;
            bool cheat = tier >= 100;
            if (cheat)
            {
                npc.buffImmune[ModContent.BuffType<T>()] = false;
            }
            if (cheat || (NecromancyDatabase.TryGetByNetID(npc, out var value) && value.PowerNeeded <= tier))
            {
                npc.AddBuff(ModContent.BuffType<T>(), time);
                npc.GetGlobalNPC<NecromancyNPC>().zombieOwner = player;
                if (Main.netMode == NetmodeID.MultiplayerClient)
                {
                    PacketHandler.SyncNecromancyOwner(npc.whoAmI, player, tier);
                }
            }
        }
    }
}