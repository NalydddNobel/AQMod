using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Buffs.Debuffs
{
    public class Weakness : ModBuff
    {
        public override string Texture => Aequus.VanillaTexture + "Buff_" + BuffID.Weak;

        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;

            AequusBuff.AddStaticImmunity(NPCID.WallofFleshEye, false, Type);
            foreach (var n in ContentSamples.NpcsByNetId)
            {
                if (n.Value.boss || (NPCID.Sets.DebuffImmunitySets.TryGetValue(n.Key, out var buff) && buff.SpecificallyImmuneTo.ContainsAny(BuffID.Weak)))
                {
                    AequusBuff.AddStaticImmunity(n.Key, false, Type);
                }
            }
        }
    }
}