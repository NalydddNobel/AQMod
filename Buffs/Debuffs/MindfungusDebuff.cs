using Aequus.Content.Necromancy;
using Aequus.Content.Necromancy.Renderer;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Buffs.Debuffs
{
    public class MindfungusDebuff : ModBuff
    {
        public override string Texture => Aequus.VanillaTexture + "Buff_" + BuffID.Bleeding;

        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;

            foreach (var n in ContentSamples.NpcsByNetId)
            {
                if (n.Value.boss || n.Value.defense >= 100)
                {
                    AequusBuff.AddStaticImmunity(n.Key, false, Type);
                }
            }
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            if (NecromancyDatabase.TryGet(npc, out var info) && info.EnoughPower(1.1f))
            {
                var zombie = npc.GetGlobalNPC<NecromancyNPC>();
                zombie.conversionChance = 2;
                zombie.zombieDebuffTier = 1.1f;
                zombie.renderLayer = GhostRenderer.IDs.BloodRed;
            }
        }
    }
}