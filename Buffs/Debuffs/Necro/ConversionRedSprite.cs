using Aequus.Content.Necromancy;
using Aequus.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Buffs.Debuffs.Necro
{
    public class ConversionRedSprite : ModBuff
    {
        public override string Texture => Aequus.Debuff;

        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            var ghost = npc.GetGlobalNPC<NecromancyNPC>();
            ghost.DebuffTier(3);
            ghost.ConversionChance(8);
            ghost.RenderLayer(GhostOutlineTarget.IDs.BloodRed);
        }
    }
}