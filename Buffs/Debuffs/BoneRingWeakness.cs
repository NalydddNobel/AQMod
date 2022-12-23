using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Buffs.Debuffs
{
    public class BoneRingWeakness : ModBuff
    {
        public override string Texture => Aequus.VanillaTexture + "Buff_" + BuffID.Weak;

        public static SoundStyle InflictDebuffSound => Aequus.GetSound("inflictweakness");

        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;

            AequusBuff.AddStaticImmunity(NPCID.WallofFleshEye, false, Type);
            foreach (var n in ContentSamples.NpcsByNetId)
            {
                if (n.Value.boss || (NPCID.Sets.DebuffImmunitySets.TryGetValue(n.Key, out var buff) && buff != null && buff.SpecificallyImmuneTo != null && buff.SpecificallyImmuneTo.ContainsAny(BuffID.Weak)))
                {
                    AequusBuff.AddStaticImmunity(n.Key, false, Type);
                }
            }
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.StatSpeed() *= 0.75f;
            npc.Aequus().statAttackDamage *= 0.9f;
        }
    }
}