using Aequus.Content.Necromancy;
using Aequus.Content.Necromancy.Renderer;
using System;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Buffs.Necro {
    public class ConversionRedSprite : ModBuff
    {
        public override string Texture => Aequus.PlaceholderDebuff;

        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            var zombie = npc.GetGlobalNPC<NecromancyNPC>();
            zombie.DebuffTier(3);
            zombie.ConversionChance(4);
            zombie.ghostDamage = Math.Max(zombie.ghostDamage, 60);
            zombie.RenderLayer(ColorTargetID.BloodRed);
        }
    }
}